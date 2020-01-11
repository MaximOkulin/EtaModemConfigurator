using DevExpress.Mvvm;
using DevExpress.Xpf.WindowsUI;
using EtaModemConfigurator.API;
using EtaModemConfigurator.Commands;
using EtaModemConfigurator.Models;
using EtaModemConfigurator.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EtaModemConfigurator.ViewModels
{
    public class RelayCalendarViewModel : ViewModelBase
    {
        private List<RelayTime> _calendar;

        public List<RelayTime> Calendar
        {
            get => _calendar;
        }

        private Transport.Transport _transport;
        private ActionSteps _actionSteps;
        private readonly object _readSignalQualityLocker;
        private Timer _signalQualityTimer;
        private readonly AllConnectionGridViewModel _allConnectionGridViewModel;

        public RelayCalendarViewModel(List<RelayTime> calendar, Transport.Transport transport, ActionSteps actionSteps, AllConnectionGridViewModel allConnectionGridViewModel = null)
        {
            _transport = transport;
            _actionSteps = actionSteps;
            _allConnectionGridViewModel = allConnectionGridViewModel;

            _calendar = calendar;
            CheckCalendarForEdit();

            foreach(var relayTime in calendar)
            {
                relayTime.RelayTimeChanged += RelayTime_RelayTimeChanged;
            }

            _readSignalQualityLocker = new object();
            _signalQualityTimer = new Timer(ReadSignalQuality, null, 5000, 15000);            
        }

        private void ReadSignalQuality(object state)
        {
            lock (_readSignalQualityLocker)
            {
                try
                {
#if DEBUG
                    Debug.WriteLine("поддерживаем связь в фоновом режиме");
#endif
                    _actionSteps.ReadSignalQuality();
                }
                catch { }
            }
        }

        private void RelayTime_RelayTimeChanged(object sender, System.EventArgs e)
        {
            CheckCalendarForEdit();
        }

        private RelayCommand _saveCommand;

        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(param => SaveCalendar(), null);
                }
                return _saveCommand;
            }
        }

        private void SaveCalendar()
        {
            WaitIndicatorState = true;
            IsBtnEditEnabled = false;
            CurrentOperationName = "запись календаря реле...";

            new Task(() =>
            {
                var editedRelayTimes = _calendar.Where(p => p.IsEdited).ToList();

                lock (_readSignalQualityLocker)
                {
                    foreach (var relayTime in editedRelayTimes)
                    {
                        CurrentOperationName = string.Format("запись даты: {0}.{1}.{2} реле{3}", relayTime.Date.Day, relayTime.Date.Month,
                            relayTime.Date.Year, relayTime.RelayNumber);

                        _actionSteps.SetRelayCalendar(relayTime);


                        if (_transport.Buffer[1] == 0x10)
                        {
                            CurrentOperationName = "запись успешна";
                            relayTime.SetSuccessfullEdit();
                        }
                    }
                    WaitIndicatorState = false;
                }
            }).Start();
        }

        

        private void CheckCalendarForEdit()
        {
            IsBtnEditEnabled = _calendar.Count(p => p.IsEdited) > 0;
        }

        private bool _isBtnEditEnabled = false;
        public bool IsBtnEditEnabled
        {
            get => _isBtnEditEnabled;
            set
            {
                _isBtnEditEnabled = value;
                OnPropertyChanged("IsBtnEditEnabled");
            }
        }

        private bool _waitIndicatorState = false;

        public bool WaitIndicatorState
        {
            get
            {
                return _waitIndicatorState;
            }
            set
            {
                _waitIndicatorState = value;
                OnPropertyChanged("WaitIndicatorState");
            }
        }

        private string _currentOperationName;

        public string CurrentOperationName
        {
            get
            {
                return _currentOperationName;
            }
            set
            {
                _currentOperationName = value;
                OnPropertyChanged("CurrentOperationName");
            }
        }


        private RelayCommand _backCommand;
        public ICommand BackCommand
        {
            get
            {
                if (_backCommand == null)
                {
                    _backCommand = new RelayCommand(param =>
                    {
                        if (_transport != null)
                        {
                            if (_transport.TransportType == TransportTypes.Direct)
                            {
                                _actionSteps.Reboot();
                                _transport.CloseSerialPort();
                            }
                            else if (_transport.TransportType == TransportTypes.TCP)
                            {
#if DEBUG
                                Debug.WriteLine("отцепляем событие изменения коллекции RelayTime");
#endif
                                foreach (var relayTime in _calendar)
                                {
                                    relayTime.RelayTimeChanged -= RelayTime_RelayTimeChanged;
                                }

                                // отключаем таймер чтения уровня сигнала
                                _signalQualityTimer.Change(Timeout.Infinite, Timeout.Infinite);

                                if (_allConnectionGridViewModel != null)
                                {
                                    _allConnectionGridViewModel.FreeModem();
                                }
                            }

                            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                            {
                                ((Application.Current.MainWindow.Content as Grid).Children[0] as NavigationFrame).GoBack();
                            }));
                        }
                    }, null);
                }
                return _backCommand;
            }
        }
    }
}
