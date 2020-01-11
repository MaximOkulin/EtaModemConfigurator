using EtaModemConfigurator.Models;
using System.Reflection;
using System.Collections.Generic;
using System;
using EtaModemConfigurator.API;
using System.Linq;
using System.Windows;
using EtaModemConfigurator.Commands;
using System.Windows.Input;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using EtaModemConfigurator.Types;
using System.IO.Ports;
using EtaModemConfigurator.Views;
using System.Windows.Controls;
using DevExpress.Xpf.WindowsUI;
using System.Diagnostics;
using System.Net;
using GSMSignalLevelLibrary;
using System.IO;
using Newtonsoft.Json;

namespace EtaModemConfigurator.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        private bool _isGoBack = false;
        private bool _isNeedToCloseConnection;
        public bool IsNeedToCloseConnection
        {
            get => _isNeedToCloseConnection;
            set
            {
                _isNeedToCloseConnection = value;
            }
        }

        private ModemSettings _originalModemSettings;
        private List<string> _changedProperties;
        private ConnectionInfo _connectionInfo;

        private ModemSettings _modemSettings;
        private Transport.Transport _transport;
        private ActionSteps _actionSteps;

        private ICommand _importButtonPressed;
        public ICommand ImportButtonPressed
        {
            get => _importButtonPressed = _importButtonPressed == null ? new RelayCommand(p =>
            {
                using (var openFileDialog = new System.Windows.Forms.OpenFileDialog())
                {
                    openFileDialog.Filter = "Файлы конфигурации (*.cfg)|*.cfg|Все файлы (*.*)|*.*";
                    openFileDialog.DefaultExt = "cfg";
                    try
                    {
                        string modemSettingsString = openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK ? File.ReadAllText(openFileDialog.FileName) : null;
                        if (modemSettingsString != null)
                        {
                            var importedModemSettings = JsonConvert.DeserializeObject<ModemSettings>(modemSettingsString);

                            var propertyInfo = typeof(ModemSettings).GetProperties();

                            for (int i = 0; i < propertyInfo.Length; i++)
                            {
                                var property = typeof(DashboardViewModel).GetProperty(propertyInfo[i].Name);
                                if (property != null)
                                {
                                    property.SetValue(this, propertyInfo[i].GetValue(importedModemSettings));
                                }
                            }
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Не удалось открыть файл!", "Конфигуратор gprs-модемов 'ЭнергоТехАудит'", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }) : _importButtonPressed;
        }


        private ICommand _exportButtonPressed;
        public ICommand ExportButtonPressed
        {
            get => _exportButtonPressed = _exportButtonPressed == null ? new RelayCommand(p =>
            {
                using (var saveFileDialog = new System.Windows.Forms.SaveFileDialog())
                {
                    saveFileDialog.Filter = "Файлы конфигурации (*.cfg)|*.cfg|Все файлы (*.*)|*.*";
                    saveFileDialog.DefaultExt = "cfg";
                    string modemSettingsString = JsonConvert.SerializeObject(_modemSettings);
                    try
                    {
                        if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            File.WriteAllText(saveFileDialog.FileName, modemSettingsString);
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Не удалось сохранить файл!", "Конфигуратор gprs-модемов 'ЭнергоТехАудит'", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }) : _exportButtonPressed;
        }


        private RelayCommand _saveCommand;

        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(param => this.SaveState(), null);
                }
                return _saveCommand;
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
                        _isGoBack = true;
                        if (_transport != null)
                        {
                            if (_transport.TransportType == TransportTypes.Direct)
                            {
                                _actionSteps.Reboot();
                                _transport.CloseSerialPort();
                            }
                            else if (_transport.TransportType == TransportTypes.TCP)
                            {
                                // отключаем таймер чтения уровня сигнала
                                _signalQualityTimer.Change(Timeout.Infinite, Timeout.Infinite);

                                if (_allConnectionGridViewModel != null)
                                {
                                    _allConnectionGridViewModel.FreeModem();
                                }

                                if (_isNeedToCloseConnection)
                                {
                                    _transport.CloseTcpClient();
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

        private RelayCommand _goHome;

        public ICommand GoHome
        {
            get
            {
                if (_goHome == null)
                {
                    _goHome = new RelayCommand(param => this.NavigateToStart(), null);
                }

                return _goHome;
            }
        }

        public bool SignalQualityVisibility
        {
            get
            {
                return _transport.TransportType == TransportTypes.TCP;
            }
        }

        private string _signalQuality;

        public string SignalQuality
        {
            get
            {
                if (_transport.TransportType == TransportTypes.TCP)
                {
                    if (string.IsNullOrEmpty(_signalQuality))
                    {
                        ReadSignal();
                    }
                }

                return _signalQuality;
            }
            set
            {
#if DEBUG
                Debug.WriteLine("Новое значение уровня сигнала: " + value);
#endif
                _signalQuality = value;
                OnPropertyChanged("SignalQuality");
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


        public short ServCount
        {
            get
            {
                return _modemSettings.ServCount;
            }
            set
            {
                _modemSettings.ServCount = value;

                IpAddress2Enabled = value >= 2;
                Port2Enabled = value >= 2;

                IpAddress3Enabled = value >= 3;
                Port3Enabled = value >= 3;

                OnPropertyChanged("ServCount");
            }
        }

        public string Addr_1
        {
            get
            {
                return _modemSettings.Addr_1;
            }
            set
            {
                _modemSettings.Addr_1 = value;
                OnPropertyChanged("Addr_1");
            }
        }

        public string Addr_2
        {
            get
            {
                return _modemSettings.Addr_2;
            }
            set
            {
                _modemSettings.Addr_2 = value;
                OnPropertyChanged("Addr_2");
            }
        }

        public string Addr_3
        {
            get
            {
                return _modemSettings.Addr_3;
            }
            set
            {
                _modemSettings.Addr_3 = value;
                OnPropertyChanged("Addr_3");
            }
        }

        public ushort Port_1
        {
            get
            {
                return _modemSettings.Port_1;
            }
            set
            {
                _modemSettings.Port_1 = value;
                OnPropertyChanged("Port_1");
            }
        }

        public ushort Port_2
        {
            get
            {
                return _modemSettings.Port_2;
            }
            set
            {
                _modemSettings.Port_2 = value;
                OnPropertyChanged("Port_2");
            }
        }

        public ushort Port_3
        {
            get
            {
                return _modemSettings.Port_3;
            }
            set
            {
                _modemSettings.Port_3 = value;
                OnPropertyChanged("Port_3");
            }
        }

        public ushort Pin
        {
            get
            {
                return _modemSettings.Pin;
            }
            set
            {
                _modemSettings.Pin = value;
                OnPropertyChanged("Pin");
            }
        }

        public string Apn
        {
            get
            {
                return _modemSettings.Apn;
            }
            set
            {
                _modemSettings.Apn = value;
                OnPropertyChanged("Apn");
            }
        }

        public string Login
        {
            get
            {
                return _modemSettings.Login;
            }
            set
            {
                _modemSettings.Login = value;
                OnPropertyChanged("Login");
            }
        }

        public string Password
        {
            get
            {
                return _modemSettings.Password;
            }
            set
            {
                _modemSettings.Password = value;
                OnPropertyChanged("Password");
            }
        }

        private Dictionary<short, string> _modemModeList;

        public Dictionary<short, string> ModemModeList
        {
            get
            {
                if (_modemModeList == null)
                {
                    _modemModeList = new Dictionary<short, string>
                    {
                        { 0, "Клиент" },
                        { 1, "Сервер" }
                    };
                }
                return _modemModeList;
            }
        }

        public short ModemModeId
        {
            get
            {
                return _modemSettings.ModemModeId;
            }
            set
            {
                _modemSettings.ModemModeId = value;
                OnPropertyChanged("ModemModeId");
                OnPropertyChanged("IsClientTabEnabled");
                OnPropertyChanged("IsServerTabEnabled");
            }
        }

        private Dictionary<short, string> _serialList;

        public Dictionary<short, string> SerialList
        {
            get
            {
                if (_serialList == null)
                {
                    _serialList = new Dictionary<short, string>
                    {
                        { 0, "RS-232" },
                        { 1, "RS-485" },
                        { 2, "RS-232 + RS-485" }
                    };
                }
                return _serialList;
            }
        }

        public short SelectSerial
        {
            get
            {
                return _modemSettings.SelectSerial;
            }
            set
            {
                _modemSettings.SelectSerial = value;
                OnPropertyChanged("SelectSerial");
            }
        }

        public bool IsClientTabEnabled
        {
            get
            {
                return _modemSettings.ModemModeId == 0;
            }
        }

        public bool IsServerTabEnabled
        {
            get
            {
                return _modemSettings.ModemModeId == 1;
            }
        }

        public ushort ListenPort
        {
            get
            {
                return _modemSettings.ListenPort;
            }
            set
            {
                _modemSettings.ListenPort = value;
                OnPropertyChanged("ListenPort");
            }
        }

        public short DebugModeId
        {
            get
            {
                return _modemSettings.DebugModeId;
            }
            set
            {
                _modemSettings.DebugModeId = value;
                OnPropertyChanged("DebugModeId");
            }
        }

        public short NetworkAddress
        {
            get
            {
                return _modemSettings.NetworkAddress;
            }
            set
            {
                _modemSettings.NetworkAddress = value;
                OnPropertyChanged("NetworkAddress");
            }
        }

        public string Identifier
        {
            get
            {
                return _modemSettings.Identifier;
            }
            set
            {
                _modemSettings.Identifier = value;
                OnPropertyChanged("Identifier");
            }
        }

        private uint[] _baudRateList;

        public uint[] BaudRateList
        {
            get
            {
                if (_baudRateList == null)
                {
                    _baudRateList = new uint[]
                        { 1200, 2400, 4800, 9600, 14400, 19200, 28800, 38400, 57600, 115200 };
                };
                return _baudRateList;
            }
        }

        public uint BaudRate
        {
            get
            {
                return _modemSettings.BaudRate;
            }
            set
            {
                _modemSettings.BaudRate = value;
                OnPropertyChanged("BaudRate");
            }
        }

        private Dictionary<short, string> _dataFormatList;

        public int TransportTypeId
        {
            get
            {
                return (int)_transport.TransportType;
            }
        }

        public Dictionary<short, string> DataFormatList
        {
            get
            {
                if (_dataFormatList == null)
                {
                        _dataFormatList = new Dictionary<short, string>
                        {
                        { 0, "5N1" },
                        { 2, "6N1" },
                        { 4, "7N1" },
                        { 6, "8N1" },
                        { 8, "5N2" },
                        { 10, "6N2" },
                        { 12, "7N2" },
                        { 14, "8N2" },
                        { 32, "5E1" },
                        { 34, "6E1" },
                        { 36, "7E1" },
                        { 38, "8E1" },
                        { 40, "5E2" },
                        { 42, "6E2" },
                        { 44, "7E2" },
                        { 46, "8E2" },
                        { 48, "5O1" },
                        { 50, "6O1" },
                        { 52, "7O1" },
                        { 54, "8O1" },
                        { 56, "5O2" },
                        { 58, "6O2" },
                        { 60, "7O2" },
                        { 62, "8O2" }
                       };
                }
                return _dataFormatList;
            }
        }

        public short DataFormat
        {
            get
            {
                return _modemSettings.DataFormat;
            }
            set
            {
                _modemSettings.DataFormat = value;
                OnPropertyChanged("DataFormat");
            }
        }

        public short NmRetry
        {
            get
            {
                return _modemSettings.NmRetry;
            }
            set
            {
                _modemSettings.NmRetry = value;
                OnPropertyChanged("NmRetry");
            }
        }

        public short WaitTm
        {
            get
            {
                return _modemSettings.WaitTm;
            }
            set
            {
                _modemSettings.WaitTm = value;
                OnPropertyChanged("WaitTm");
            }
        }

        public short SendSz
        {
            get
            {
                return _modemSettings.SendSz;
            }
            set
            {
                _modemSettings.SendSz = value;
                OnPropertyChanged("SendSz");
            }
        }

        public short RxMode
        {
            get
            {
                return _modemSettings.RxMode;
            }
            set
            {
                _modemSettings.RxMode = value;
                OnPropertyChanged("RxMode");
            }
        }

        public short RxSize
        {
            get
            {
                return _modemSettings.RxSize;
            }
            set
            {
                _modemSettings.RxSize = value;
                OnPropertyChanged("RxSize");
            }
        }

        public short RxTimer
        {
            get
            {
                return _modemSettings.RxTimer;
            }
            set
            {
                _modemSettings.RxTimer = value;
                OnPropertyChanged("RxTimer");
            }
        }

        public ushort CheckPeriod
        {
            get
            {
                return _modemSettings.CheckPeriod;
            }
            set
            {
                _modemSettings.CheckPeriod = value;
                OnPropertyChanged("CheckPeriod");
            }
        }

        public ushort TimeForReconnect
        {
            get
            {
                return _modemSettings.TimeForReconnect;
            }
            set
            {
                _modemSettings.TimeForReconnect = value;
                OnPropertyChanged("TimeForReconnect");
            }
        }

        public ushort RebootTime
        {
            get
            {
                return _modemSettings.RebootTime;
            }
            set
            {
                _modemSettings.RebootTime = value;
                OnPropertyChanged("RebootTime");
            }
        }

        private readonly LocalSettings _settings;
        private readonly object _readSignalQualityLocker;
        private readonly AllConnectionGridViewModel _allConnectionGridViewModel;

        public DashboardViewModel(ModemSettings modemSettings, Transport.Transport transport, ActionSteps actionSteps, 
            ConnectionInfo connectionInfo, LocalSettings settings, AllConnectionGridViewModel allConnectionGridViewModel = null)
        {
            _modemSettings = modemSettings;
            _transport = transport;
            _actionSteps = actionSteps;
            _originalModemSettings = (ModemSettings)modemSettings.Clone();
            _changedProperties = new List<string>();
            _connectionInfo = connectionInfo;
            _settings = settings;
            _readSignalQualityLocker = new object();
            _allConnectionGridViewModel = allConnectionGridViewModel;

            if (_transport.TransportType == TransportTypes.TCP)
            {
                _signalQualityTimer = new Timer(ReadSignalQuality, null, 8000, 10000);
            }
        }

        private void ReadSignalQuality(object state)
        {
            lock (_readSignalQualityLocker)
            {
                _signalQualityTimer.Change(Timeout.Infinite, Timeout.Infinite);
                try
                {
#if DEBUG
                    Debug.WriteLine("Первая попытка запроса уровня сигнала");
#endif
                    ReadSignal();
                }
                catch
                {
                    try
                    {
#if DEBUG
                        Debug.WriteLine("Вторая попытка запроса уровня сигнала");
#endif
                        ReadSignal();

                    }
                    catch
                    {
                        if (!_isGoBack)
                        {
                            SignalQuality = "-1";
                            ConnectionState = ConnectionState.Failed;
                            Thread.Sleep(1500);
                            CurrentOperationName = "восстановление соединения...";

                            var endPoint = _transport.TcpClient.Client.RemoteEndPoint;
                            var address = ((IPEndPoint)endPoint).Address.ToString();
                            var port = ((IPEndPoint)endPoint).Port;

                            while (true && !_isGoBack)
                            {
                                try
                                {
                                    ConnectionState = ConnectionState.Reestablish;
                                    var tcpClient = new System.Net.Sockets.TcpClient(address, port);
                                    _actionSteps.Transport.Stream = tcpClient.GetStream();
                                    break;
                                }
                                catch (Exception ex)
                                {

                                }
                                Thread.Sleep(500);
                            }
                        }
                    }
                }

                if (!_isGoBack)
                {
                    _signalQualityTimer.Change(8000, 10000);
                }
            }
        }

        private void ReadSignal()
        {
            var package = _actionSteps.Functions.ReadSignalQuality();
#if DEBUG
            Debug.WriteLine("Выполнение команды чтения уровня сигнала");
            Debug.WriteLine(BitConverter.ToString(package));
#endif
            _actionSteps.Transport.Stream.Write(package, 0, 8);
           
            var buffer = new byte[11];
            _actionSteps.Transport.Stream.Read(buffer, 0, 11);

            if (buffer.Length == 11 && buffer[1] == 0x04 && buffer[2] == 0x06)
            {
                var buf = buffer.GetPackageBody();
                SignalQuality = buf.GetNullTerminatedString(Encoding.GetEncoding(1251)).Value.Split(',')[0];
            }           
#if DEBUG
            Debug.WriteLine("Уровень сигнала " + SignalQuality);            
#endif
        }

        private int _signalLevel;

        public int SignalLevel
        {
            get
            {
                return _signalLevel;
            }
            set
            {
                _signalLevel = value;
                OnPropertyChanged("SignalLevel");
            }
        }

        private Timer _signalQualityTimer;        

        protected override void CheckProperty(string propertyName)
        {
            PropertyInfo propertyInfo = _modemSettings.GetType().GetProperty(propertyName);

            if (propertyInfo != null)
            {
                var newValue = propertyInfo.GetValue(_modemSettings, null);
                var oldValue = propertyInfo.GetValue(_originalModemSettings, null);

                if ((newValue is ValueType) && !(newValue as ValueType).Equals(oldValue))
                {
                    if (!_changedProperties.Contains(propertyName))
                    {
                        _changedProperties.Add(propertyName);
                        //SaveButtonVisibility = Visibility.Visible;
                        SaveButtonIsEnabled = true;
                    }
                }
                else if (newValue is string)
                {
                    if (!Convert.ToString(newValue).Equals(Convert.ToString(oldValue), StringComparison.Ordinal) && !_changedProperties.Contains(propertyName))
                    {
                        _changedProperties.Add(propertyName);
                        //SaveButtonVisibility = Visibility.Visible;
                        SaveButtonIsEnabled = true;
                    }
                    if (Convert.ToString(newValue).Equals(Convert.ToString(oldValue), StringComparison.Ordinal) && _changedProperties.Contains(propertyName))
                    {
                        _changedProperties.Remove(propertyName);
                        //SaveButtonVisibility = Visibility.Hidden;
                        SaveButtonIsEnabled = false;
                        //WaitIndicatorState = false;
                    }
                }
                else
                {
                    if (_changedProperties.Contains(propertyName))
                    {
                        _changedProperties.Remove(propertyName);
                        //SaveButtonVisibility = Visibility.Hidden;
                        SaveButtonIsEnabled = false;
                    }
                }
            }
        }

        private Visibility _saveButtonVisibility = Visibility.Hidden;
        public Visibility SaveButtonVisibility
        {
            get
            {
                return _saveButtonVisibility;
            }
            set
            {
                if (_changedProperties.Count > 0)
                {
                    _saveButtonVisibility = Visibility.Visible;
                }
                else
                {
                    _saveButtonVisibility = Visibility.Hidden;
                }
                OnPropertyChanged("SaveButtonVisibility");
            }
        }

        private bool _saveButtonIsEnabled = false;
        public bool SaveButtonIsEnabled
        {
            get
            {
                return _saveButtonIsEnabled;
            }
            set
            {
                if (_changedProperties.Count > 0)
                {
                    _saveButtonIsEnabled = true;
                }
                else
                {
                    _saveButtonIsEnabled = false;
                }
                OnPropertyChanged("SaveButtonIsEnabled");
            }

        }

        private bool _ipAddress2Enabled;

        public bool IpAddress2Enabled
        {
            get
            {
                return _ipAddress2Enabled;
            }
            set
            {
                _ipAddress2Enabled = value;
                OnPropertyChanged("IpAddress2Enabled");
            }
        }

        private bool _ipAddress3Enabled;

        public bool IpAddress3Enabled
        {
            get
            {
                return _ipAddress3Enabled;
            }
            set
            {
                _ipAddress3Enabled = value;
                OnPropertyChanged("IpAddress3Enabled");
            }
        }


        private bool _port2Enabled;

        public bool Port2Enabled
        {
            get
            {
                return _port2Enabled;
            }
            set
            {
                _port2Enabled = value;
                OnPropertyChanged("Port2Enabled");
            }
        }

        private bool _port3Enabled;

        public bool Port3Enabled
        {
            get
            {
                return _port3Enabled;
            }
            set
            {
                _port3Enabled = value;
                OnPropertyChanged("Port3Enabled");
            }
        }

        private ConnectionState _connectionState;
        public ConnectionState ConnectionState
        {
            get => _connectionState;
            set
            {
                _connectionState = value;
                OnPropertyChanged("ConnectionState");
            }
        }

        private void NavigateToStart()
        {
            _transport.CloseSerialPort();
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                var mainView = new MainView();
                ((Application.Current.MainWindow.Content as Grid).Children[0] as NavigationFrame).Navigate(mainView);
            }));
        }

        public bool IsAirConnected { get; set; } = true;
        private void SaveState()
        {
            if (IsAirConnected)
            {
                if (MessageBox.Show("Соединение с модемом осуществлено по беспроводной сети. В случае неккоректных данных связь с модемом будет потеряна!\nВсё равно сохранить?", "Конфигуратор gprs-модемов 'ЭнергоТехАудит'", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    SaveStateNoWarning();
                }
            }
            else
            {
                SaveStateNoWarning();
            }
        }

        private void SaveStateNoWarning()
        {
            if (_changedProperties.Any())
            {
                WaitIndicatorState = true;
            }

            new Task(() =>
            {
                var localChangedProperties = _changedProperties.Except(new string[] { "NetworkAddress", "BaudRate", "DataFormat" }).ToArray();

                if (_changedProperties.Contains("NetworkAddress"))
                {
                    SetValue("NetworkAddress");
                    _transport.NetworkAddress = _originalModemSettings.NetworkAddress;

                    lock (_readSignalQualityLocker)
                    {
                        _actionSteps.SaveSettings(_originalModemSettings.NetworkAddress);
                    }

                    _settings.NetworkAddress = _originalModemSettings.NetworkAddress;
                    _settings.SaveSettings();
                }

                if (_changedProperties.Contains("BaudRate"))
                {
                    SetValue("BaudRate");
                    SetNewDataFormat(_originalModemSettings.DataFormat);
                }


                if (_changedProperties.Contains("DataFormat"))
                {
                    SetValue("DataFormat");
                    SetNewDataFormat(_originalModemSettings.DataFormat);
                }

                foreach (var propertyName in localChangedProperties)
                {
                    SetValue(propertyName);
                }

                Thread.Sleep(1000);
               
                WaitIndicatorState = false;
            }).Start();
        }

        private void SetNewDataFormat(short dataFormat)
        {
            switch(dataFormat)
            {
                case 0x06: // 8N1
                    _connectionInfo.Parity = Parity.None;
                    _connectionInfo.DataBits = 8;
                    _connectionInfo.StopBits = StopBits.One;
                    break;
                case 0x0E: // 8N2
                    _connectionInfo.Parity = Parity.None;
                    _connectionInfo.DataBits = 8;
                    _connectionInfo.StopBits = StopBits.Two;
                    break;
                case 0x26: // 8E1
                    _connectionInfo.Parity = Parity.Even;
                    _connectionInfo.DataBits = 8;
                    _connectionInfo.StopBits = StopBits.One;
                    break;
                case 0x2E: // 8E2
                    _connectionInfo.Parity = Parity.Even;
                    _connectionInfo.DataBits = 8;
                    _connectionInfo.StopBits = StopBits.Two;
                    break;
                case 0x36: // 8O1
                    _connectionInfo.Parity = Parity.Odd;
                    _connectionInfo.DataBits = 8;
                    _connectionInfo.StopBits = StopBits.One;
                    break;
                case 0x3E: // 8O2
                    _connectionInfo.Parity = Parity.Odd;
                    _connectionInfo.DataBits = 8;
                    _connectionInfo.StopBits = StopBits.Two;
                    break;
            }
        }

        // TODO: Debug 
        public void ReadIsSetting()
        {
            _actionSteps.ReadIsSetting();

            var buf = _transport.Buffer;

            var k  = BitConverter.ToInt16(new byte[] { buf[4], buf[3] }, 0);

            Thread.Sleep(10000);

            _actionSteps.ReadIsSetting();

            buf = _transport.Buffer;

            var t = BitConverter.ToInt16(new byte[] { buf[4], buf[3] }, 0);
        }

        private MethodInfo GetSetMethod(string propertyName)
        {
            return typeof(ActionSteps).GetMethods().FirstOrDefault(mi => mi.Name.Equals(string.Format("Set{0}", propertyName)));
        }

        private void SetValue(string propertyName)
        {
            lock (_readSignalQualityLocker)
            {
                MethodInfo setMethod = GetSetMethod(propertyName);

                var attr = setMethod.GetCustomAttributesData().FirstOrDefault();
                if (attr != null)
                {
                    CurrentOperationName = (string)attr.ConstructorArguments[0].Value;
                }

                if (setMethod != null)
                {
                    PropertyInfo propertyInfo = _modemSettings.GetType().GetProperty(propertyName);
                    var newVal = propertyInfo.GetValue(_modemSettings, null);

                    ParameterInfo argParameter = setMethod.GetParameters().FirstOrDefault();

                    if (argParameter != null)
                    {
                        var parType = argParameter.ParameterType;

                        var newValue = Convert.ChangeType(newVal, parType);

                        try
                        {
                            setMethod.Invoke(_actionSteps, new[] { newValue });

                            if (_actionSteps.Transport.CurrentErrorCode == Types.ErrorCode.None)
                            {
                                propertyInfo.SetValue(_originalModemSettings, newVal, null);
                                CheckProperty(propertyName);
                            }
                        }
                        catch
                        {

                        }
                    }
                }

                WaitIndicatorState = true;
                _actionSteps.SaveSettings(_originalModemSettings.NetworkAddress);

                int secondsRemain = 15;
                while (secondsRemain > 0)
                {
                    CurrentOperationName = string.Format("ожидание {0} секунд...", secondsRemain);
                    Thread.Sleep(1000);
                    secondsRemain--;
                }                
            }
        }
    }
}
