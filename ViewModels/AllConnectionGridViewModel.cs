using DevExpress.Mvvm;
using DevExpress.Xpf.WindowsUI;
using EtaModemConfigurator.Commands;
using EtaModemConfigurator.Transport;
using EtaModemConfigurator.Types;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EtaModemConfigurator.ViewModels
{
    public class AllConnectionGridViewModel: CommonTransportViewModel
    {
        public ItemsChangeObservableCollection<ModemInfo> ModemInfos { get; private set; }
        private ConnectionManager _connectionManager;

        public AllConnectionGridViewModel(string listenAddress, int listenPort)
        {
            IsAirConnected = true;
            _connectionManager = new ConnectionManager(listenAddress, listenPort);
            _connectionManager.StartListener();
            ModemInfos = _connectionManager.ModemInfos;
            CommonTransport = new CommonTransport(TransportTypes.TCP);
        }

        private ModemInfo _selectedItem;
        public ModemInfo SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");

                if (value != null)
                {
                    IsBtnEditEnabled = true;
                    IsGridEnabled = true;

                    if (value.SoftwareVersion == 200)
                    {
                        EditRelayCalendarVisibility = Visibility.Visible;
                        IsBtnEditRelayCalendarEnabled = true;
                    }
                    else
                    {
                        EditRelayCalendarVisibility = Visibility.Hidden;
                    }
                }
                else
                {
                    IsBtnEditEnabled = false;
                }
            }
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

        private RelayCommand _editCommand;

        public ICommand EditCommand
        {
            get
            {
                if (_editCommand == null)
                {
                    _editCommand = new RelayCommand(param => GoToDashboard(), null);
                }
                return _editCommand;
            }
        }

        private RelayCommand _editRelayCalendarCommand;

        public ICommand EditRelayCalendarCommand
        {
            get
            {
                if (_editRelayCalendarCommand == null)
                {
                    _editRelayCalendarCommand = new RelayCommand(param => GoToEditCalendar(), null);
                }
                return _editRelayCalendarCommand;
            }
        }

        private void GoToEditCalendar()
        {
            if (SelectedItem != null)
            {
                IsBtnEditEnabled = false;
                IsGridEnabled = false;
                IsBtnEditRelayCalendarEnabled = false;
                WaitIndicatorState = true;
                OperationName = "Чтение календаря реле...";
                CurrentOperationName = "чтение календаря реле...";

                _connectionManager.SetModemBusy(SelectedItem.Identifier, true);
#if DEBUG
                Debug.WriteLine("Начинаю чтения календаря");
#endif
                ReadCalendar(SelectedItem.TcpClient);
            }
        }

        private bool _isBtnEditRelayCalendarEnabled = false;
        public bool IsBtnEditRelayCalendarEnabled
        {
            get => _isBtnEditRelayCalendarEnabled;
            set
            {
                _isBtnEditRelayCalendarEnabled = value;
                OnPropertyChanged("IsBtnEditRelayCalendarEnabled");
            }
        }

        private Visibility _editRelayCalendarVisibility = Visibility.Hidden;
        
        public Visibility EditRelayCalendarVisibility
        {
            get => _editRelayCalendarVisibility;
            set
            {
                _editRelayCalendarVisibility = value;
                OnPropertyChanged("EditRelayCalendarVisibility");
            }
        }

        private void GoToDashboard()
        {
            if (SelectedItem != null)
            {
                IsBtnEditEnabled = false;
                IsGridEnabled = false;
                IsBtnEditRelayCalendarEnabled = false;
                WaitIndicatorState = true;
                OperationName = "Чтение настроек модема...";
                CurrentOperationName = string.Empty;

                _connectionManager.SetModemBusy(SelectedItem.Identifier, true);
#if DEBUG
                Debug.WriteLine("Выставил занятость модема " + SelectedItem.Identifier + " Ожидаю таймут");
                Debug.WriteLine("Начинаю опрос модема");
#endif
                StartSession(SelectedItem.TcpClient);
            }
        }

        /// <summary>
        /// Освобождает занятость точки доступа
        /// </summary>
        public void FreeModem()
        {
            IsBtnEditEnabled = true;
            IsGridEnabled = true;
            IsBtnEditRelayCalendarEnabled = true;
            WaitIndicatorState = false;
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                _connectionManager.SetModemBusy(SelectedItem.Identifier, false);
            }));
        }

        protected override void LostConnection()
        {
            base.LostConnection();
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                _connectionManager.RemoveModemFromList(SelectedItem.Identifier);
            }));

            FreeModem();
        }

        private bool _isGridEnabled = false;

        public bool IsGridEnabled
        {
            get => _isGridEnabled;
            set
            {
                _isGridEnabled = value;
                OnPropertyChanged("IsGridEnabled");
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
                        _connectionManager.StopListener();
                        Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            ((Application.Current.MainWindow.Content as Grid).Children[0] as NavigationFrame).GoBack();
                        }));
                    }, null);
                }
                return _backCommand;
            }
        }
    }

}
