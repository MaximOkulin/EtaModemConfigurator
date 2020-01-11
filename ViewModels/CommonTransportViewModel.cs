using DevExpress.Xpf.WindowsUI;
using EtaModemConfigurator.Models;
using EtaModemConfigurator.Transport;
using EtaModemConfigurator.Types;
using EtaModemConfigurator.Views;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EtaModemConfigurator.ViewModels
{
    public class CommonTransportViewModel : ViewModelBase
    {
        public CommonTransport CommonTransport;
        protected readonly LocalSettings Settings;
        protected bool IsNeedToCloseConnection;

        public CommonTransportViewModel()
        {
            Settings = new LocalSettings();
            Settings.Init();
        }

        protected void StartSession()
        {
            CommonTransport.InitConnection();

            if (CommonTransport.StartSession())
            {
                new Task(() => SetControlMode()).Start();
            }
            else
            {
                LostConnection();
            }
        }

        protected virtual void LostConnection()
        {
            CurrentOperationName = CommonTransport.ErrorMessage;
            Thread.Sleep(5000);
            WaitIndicatorState = false;
            CommonTransport.IsLostConnectionRaised = false;
            IsEnabledConnectBtn = true;            
        }

        protected void ReadCalendar(TcpClient tcpClient)
        {
            CommonTransport.InitConnection(tcpClient);

            new Task(() =>
                {
                    CommonTransport.ReadMonthCalendarComplete += CommonTransport_ReadMonthCalendarComplete;
                    CommonTransport.ReadCalendar();
                    CommonTransport.ReadMonthCalendarComplete -= CommonTransport_ReadMonthCalendarComplete;

                    Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        var relayCalendarView = new RelayCalendarView();
                        relayCalendarView.DataContext = new RelayCalendarViewModel(CommonTransport.Calendar, CommonTransport.Transport, CommonTransport.ActionSteps, this as AllConnectionGridViewModel);
                        ((Application.Current.MainWindow.Content as Grid).Children[0] as NavigationFrame).Navigate(relayCalendarView);
                    }));
            }).Start();
        }

        private void CommonTransport_ReadMonthCalendarComplete(object sender, EventArgs.ReadCalendarEventArgs e)
        {
            CurrentOperationName = "чтение календаря на " + e.Month;
        }

        private bool _isEnabledConnectBtn;
        public bool IsEnabledConnectBtn
        {
            get => _isEnabledConnectBtn;
            set
            {
                _isEnabledConnectBtn = value;
                OnPropertyChanged("IsEnabledConnectBtn");
            }
        }

        protected void StartSession(string address, int port)
        {
            WaitIndicatorState = true;
            CurrentOperationName = "установка соединения...";
            new Task(() =>
            {
                try
                {
                    var tcpClient = new TcpClient(address, port);
                    StartSession(tcpClient);
                }
                catch
                {
                    CurrentOperationName = "соединение не установлено!";
                    Thread.Sleep(5000);
                    WaitIndicatorState = false;
                    IsEnabledConnectBtn = true;
                }
            }).Start();
        }


        protected void StartSession(TcpClient tcpClient)
        {
            CommonTransport.InitConnection(tcpClient);
            new Task(() => ReadIsSettingWithoutDelay()).Start();
        }

        private void SetControlMode()
        {
            Thread.Sleep(1000);
            CommonTransport.SetControlMode();

            if (CommonTransport.IsLostConnectionRaised)
            {
                LostConnection();
            }
            else
            {
                CurrentOperationName = "команда READY получена";
                new Task(() => ReadIsSetting()).Start();
            }
        }

        private void ReadIsSetting()
        {
            Thread.Sleep(8000);

            CommonTransport.ReadIsSetting();

            if (CommonTransport.IsLostConnectionRaised)
            {
                LostConnection();
            }
            else
            {
                new Task(() => ReadConfiguration1()).Start();
            }
        }

        protected void ReadIsSettingWithoutDelay()
        {
            CurrentOperationName = "чтение флага настройки";
            CommonTransport.ReadIsSetting();

            if (CommonTransport.IsLostConnectionRaised)
            {
                LostConnection();
            }
            else
            {
                new Task(() => ReadConfiguration1()).Start();
            }
        }

        private void ReadConfiguration1()
        {
            CurrentOperationName = "чтение режима работы";
            CommonTransport.ReadConfiguration1();

            if (CommonTransport.IsLostConnectionRaised)
            {
                LostConnection();
            }
            else
            {
                new Task(() => ReadIdentifierPart1()).Start();
            }
        }

        private void ReadIdentifierPart1()
        {
            if (CommonTransport.ReadIdentifierPart1())
            {
                if (CommonTransport.IsLostConnectionRaised)
                {
                    LostConnection();
                }
                else
                {
                    new Task(() => ReadPin()).Start();
                }
            }
            else
            {
                if (CommonTransport.IsLostConnectionRaised)
                {
                    LostConnection();
                }
                else
                {
                    new Task(() => ReadIdentifierPart2()).Start();
                }
            }
        }

        private void ReadIdentifierPart2()
        {
            if (CommonTransport.ReadIdentifierPart2())
            {
                if (CommonTransport.IsLostConnectionRaised)
                {
                    LostConnection();
                }
                else
                {
                    new Task(() => ReadPin()).Start();
                }
            }
            else
            {
                if (CommonTransport.IsLostConnectionRaised)
                {
                    LostConnection();
                }
                else
                {
                    new Task(() => ReadIdentifierPart3()).Start();
                }
            }
        }

        private void ReadIdentifierPart3()
        {
            if (CommonTransport.ReadIdentifierPart3())
            {
                if (CommonTransport.IsLostConnectionRaised)
                {
                    LostConnection();
                }
                else
                {
                    new Task(() => ReadPin()).Start();
                }
            }
            else
            {
                if (CommonTransport.IsLostConnectionRaised)
                {
                    LostConnection();
                }
                else
                {
                    new Task(() => ReadIdentifierPart4()).Start();
                }
            }
        }

        private void ReadIdentifierPart4()
        {
            if (CommonTransport.ReadIdentifierPart4())
            {
                if (CommonTransport.IsLostConnectionRaised)
                {
                    LostConnection();
                }
                else
                {
                    new Task(() => ReadPin()).Start();
                }
            }
            else
            {
                if (CommonTransport.IsLostConnectionRaised)
                {
                    LostConnection();
                }
                else
                {
                    new Task(() => ReadIdentifierPart5()).Start();
                }
            }
        }

        private void ReadIdentifierPart5()
        {
            CommonTransport.ReadIdentifierPart5();

            if (CommonTransport.IsLostConnectionRaised)
            {
                LostConnection();
            }
            else
            {
                new Task(() => ReadPin()).Start();
            }
        }


        private void ReadPin()
        {
            CurrentOperationName = "чтение PIN-кода";
            CommonTransport.ReadPin();

            if (CommonTransport.IsLostConnectionRaised)
            {
                LostConnection();
            }
            else
            {
                new Task(() => ReadApnPart1()).Start();
            }
        }

        private void ReadApnPart1()
        {
            CurrentOperationName = "чтение APN";
            if (!CommonTransport.ReadApnPart1())
            {
                if (CommonTransport.IsLostConnectionRaised)
                {
                    LostConnection();
                }
                else
                {
                    new Task(() => ReadApnPart2()).Start();
                }
            }
            else
            {
                if (CommonTransport.IsLostConnectionRaised)
                {
                    LostConnection();
                }
                else
                {
                    new Task(() => ReadLogin()).Start();
                }
            }
        }

        private void ReadApnPart2()
        {
            CommonTransport.ReadApnPart2();

            if (CommonTransport.IsLostConnectionRaised)
            {
                LostConnection();
            }
            else
            {
                new Task(() => ReadLogin()).Start();
            }
        }

        private void ReadLogin()
        {
            CurrentOperationName = "чтение логина";
            CommonTransport.ReadLogin();

            if (CommonTransport.IsLostConnectionRaised)
            {
                LostConnection();
            }
            else
            {
                new Task(() => ReadPassword()).Start();
            }
        }

        private void ReadPassword()
        {
            CurrentOperationName = "чтение пароля";
            CommonTransport.ReadPassword();

            if (CommonTransport.IsLostConnectionRaised)
            {
                LostConnection();
            }
            else
            {
                new Task(() => ReadListenPortServCount()).Start();
            }
        }

        private void ReadListenPortServCount()
        {
            CurrentOperationName = "чтение порта сервера";
            CommonTransport.ReadListenPortServCount();

            if (CommonTransport.IsLostConnectionRaised)
            {
                LostConnection();
            }
            else
            {
                new Task(() => ReadAddr_1()).Start();
            }
        }

        private void ReadAddr_1()
        {
            CurrentOperationName = "чтение IP-адрес сервера 1";
            CommonTransport.ReadAddr_1();

            if (CommonTransport.IsLostConnectionRaised)
            {
                LostConnection();
            }
            else
            {
                new Task(() => ReadPort_1()).Start();
            }
        }

        private void ReadPort_1()
        {
            CurrentOperationName = "чтение порта сервера 1";
            CommonTransport.ReadPort_1();

            if (CommonTransport.IsLostConnectionRaised)
            {
                LostConnection();
            }
            else
            {
                new Task(() => ReadAddr_2()).Start();
            }
        }

        private void ReadAddr_2()
        {
            CurrentOperationName = "чтение IP-адрес сервера 2";
            CommonTransport.ReadAddr_2();

            if (CommonTransport.IsLostConnectionRaised)
            {
                LostConnection();
            }
            else
            {
                new Task(() => ReadPort_2()).Start();
            }
        }

        private void ReadPort_2()
        {
            CurrentOperationName = "чтение порта сервера 2";
            CommonTransport.ReadPort_2();
            if (CommonTransport.IsLostConnectionRaised)
            {
                LostConnection();
            }
            else
            {
                new Task(() => ReadAddr_3()).Start();
            }
        }

        private void ReadAddr_3()
        {
            CurrentOperationName = "чтение IP-адрес сервера 3";
            CommonTransport.ReadAddr_3();

            if (CommonTransport.IsLostConnectionRaised)
            {
                LostConnection();
            }
            else
            {
                new Task(() => ReadPort_3()).Start();
            }
        }

        private void ReadPort_3()
        {
            CurrentOperationName = "чтение порта сервера 3";
            CommonTransport.ReadPort_3();

            if (CommonTransport.IsLostConnectionRaised)
            {
                LostConnection();
            }
            else
            {
                new Task(() => ReadSoftwareVersion()).Start();
            }
        }

        private void ReadSoftwareVersion()
        {
            CurrentOperationName = "чтение версии прошивки";
            CommonTransport.ReadSoftwareVersion();

            if (CommonTransport.IsLostConnectionRaised)
            {
                LostConnection();
            }
            else
            {
                new Task(() => ReadCommonSettings()).Start();
            }
        }

        private void ReadCommonSettings()
        {
            CurrentOperationName = "чтение настроек COM-портов";
            CommonTransport.ReadCommonSettings();

            if (CommonTransport.IsLostConnectionRaised)
            {
                LostConnection();
            }
            else
            {
                new Task(() => ReadCheckRebootSettings()).Start();
            }
        }

        public bool IsAirConnected { get; set; }
        private void ReadCheckRebootSettings()
        {
            CurrentOperationName = "чтение таймера перезагрузки";
            CommonTransport.ReadCheckRebootSettings();

            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                var dashBoardView = new DashboardView();
                var dashboardViewModel  = new DashboardViewModel(CommonTransport.ModemSettings, CommonTransport.Transport, CommonTransport.ActionSteps, CommonTransport.ConnectionInfo, Settings, this as AllConnectionGridViewModel) { IsAirConnected = this.IsAirConnected};
                dashboardViewModel.IsNeedToCloseConnection = IsNeedToCloseConnection;
                dashBoardView.DataContext = dashboardViewModel;
                ((Application.Current.MainWindow.Content as Grid).Children[0] as NavigationFrame).Navigate(dashBoardView);
            }));
        }

        public int NetworkAddress
        {
            get
            {
                return CommonTransport.NetworkAddress;
            }
            set
            {
                CommonTransport.NetworkAddress = value;
                Settings.NetworkAddress = value;
                Settings.SaveSettings();
                OnPropertyChanged("NetworkAddress");
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

        private string _operationName;

        public string OperationName
        {
            get
            {
                return _operationName;
            }
            set
            {
                _operationName = value;
                OnPropertyChanged("OperationName");
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
    }
}
