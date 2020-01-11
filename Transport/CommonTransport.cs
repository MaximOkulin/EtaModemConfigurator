using EtaModemConfigurator.API;
using EtaModemConfigurator.EventArgs;
using EtaModemConfigurator.Exceptions;
using EtaModemConfigurator.Models;
using EtaModemConfigurator.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace EtaModemConfigurator.Transport
{
    public class CommonTransport
    {
        public string ComPortName { get; set; }
        public int NetworkAddress { get; set; }

        public string LocalListenAddress { get; set; }
        public int LocalListenPort { get; set; }
        public ModemSettings ModemSettings;
        public List<RelayTime> Calendar;

        private TransportTypes _transportType;

        private bool _isLostConnectionRaised = false;
        public bool IsLostConnectionRaised
        {
            get => _isLostConnectionRaised;
            set
            {
                _isLostConnectionRaised = value;
            }
        }

        private string _errorMessage = "возникла ошибка!";
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
            }
        }



        public CommonTransport(TransportTypes transportType)
        {
            _transportType = transportType;
            _isLostConnectionRaised = false;

            ComPortName = "COM3";
            NetworkAddress = 254;
            ModemSettings = new ModemSettings();
            InitCalendar();
            InitTransport();
        }

        private void InitCalendar()
        {
            Calendar = new List<RelayTime>();
            var currentYear = DateTime.Now.Year;
            var firstDate = new DateTime(currentYear, 1, 1);
            var lastDate = new DateTime(currentYear, 12, 31);
            var countForRelay1 = (ushort)0;
            var countForRelay2 = (ushort)366;

            for(var dt = firstDate; dt <= lastDate; dt = dt.AddDays(1))
            {
                // Реле 1
                Calendar.Add(new RelayTime
                {
                    DayNumber = countForRelay1,
                    Date = dt,
                    RelayNumber = 1
                });

                // Реле 2
                Calendar.Add(new RelayTime
                {
                    DayNumber = countForRelay2,
                    Date = dt,
                    RelayNumber = 2
                });
                countForRelay1++;
                countForRelay2++;
            }
        }

        private Transport _transport;

        public Transport Transport
        {
            get
            {
                return _transport;
            }
        }

        private ActionSteps _actionSteps;

        public ActionSteps ActionSteps
        {
            get
            {
                return _actionSteps;
            }
        }

        private ManualResetEvent _autoEvent;

        private void InitTransport()
        {
            if (_autoEvent == null) _autoEvent = new ManualResetEvent(false);
            _transport = new Transport();
        }

        private ConnectionInfo _connectionInfo;

        public ConnectionInfo ConnectionInfo
        {
            get
            {
                return _connectionInfo;
            }
        }

        public void InitConnection(TcpClient tcpClient = null)
        {
            if (_transport != null)
            {
                ModemSettings = new ModemSettings();
                _autoEvent = new ManualResetEvent(false);

                _transport.SetNewAutoEvent(_autoEvent);

                if (_transportType == TransportTypes.Direct)
                {
                    _transport.CloseSerialPort();
                    _transport.NetworkAddress = NetworkAddress;
                    _connectionInfo = new ConnectionInfo
                    {
                        BaudRate = 9600,
                        DataBits = 8,
                        Parity = System.IO.Ports.Parity.None,
                        SerialPortName = ComPortName,
                        StopBits = System.IO.Ports.StopBits.One,
                        ReceiveTimeOut = 600000
                    };

                    _transport.InitConnection((int)TransportTypes.Direct, _connectionInfo);
                }
                else if (_transportType == TransportTypes.TCP)
                {
                    _transport.NetworkAddress = NetworkAddress;
                    _connectionInfo = new ConnectionInfo
                    {
                        PreparedTcpClient = tcpClient,
                        ReceiveTimeOut = 12000
                    };
                    _transport.InitConnection((int)TransportTypes.TCP, _connectionInfo);
                }
            };
            _actionSteps = new ActionSteps(_transport, _autoEvent, NetworkAddress);
        }

        public delegate void ReadCalendarEventHandler(object sender, ReadCalendarEventArgs e);

        // событие "Приём данных завершен"
        public event ReadCalendarEventHandler ReadMonthCalendarComplete;

        private void RaiseReadMonthCalendarComplete(string month)
        {
            if (ReadMonthCalendarComplete != null)
            {
                ReadMonthCalendarComplete(this, new ReadCalendarEventArgs { Month = month });
            }
        }

        public void ReadCalendar()
        {
            var currentYear = DateTime.Now.Year;
            var firstDate = new DateTime(currentYear, 1, 1);
            var lastDate = new DateTime(currentYear, 12, 31);

            var currentMonth = 0;

            for (var dt = firstDate; dt <= lastDate;)
            {
                var startDate = dt;
                var endDate = dt.AddDays(9);

                if (currentMonth != dt.Month)
                {
                    currentMonth = dt.Month;
                    RaiseReadMonthCalendarComplete(dt.ToString("MMMM", new CultureInfo("ru-RU")));
                }

                endDate = endDate > lastDate ? lastDate : endDate;

#if DEBUG
                Debug.WriteLine("стартовая дата: " + startDate + "; конечная дата: " + endDate);
#endif

                var startDayNumberRelay1 = Calendar.First(p => p.Date == startDate && p.RelayNumber == 1).DayNumber;
                var startDayNumberRelay2 = Calendar.First(p => p.Date == startDate && p.RelayNumber == 2).DayNumber;

                _actionSteps.ReadCalendar(startDate, endDate, startDayNumberRelay1);
                ParseCalendar(startDate, endDate, 1);

                _actionSteps.ReadCalendar(startDate, endDate, startDayNumberRelay2);
                ParseCalendar(startDate, endDate, 2);

                dt = endDate.AddDays(1);
            }
        }

        private void ParseCalendar(DateTime startDate, DateTime endDate, int relayNumber)
        {
            var buf = _transport.Buffer;
            var position = 0;
            for (var dt = startDate; dt <= endDate; dt = dt.AddDays(1))
            {
                var offset = 4 * position;
                var relayTime = Calendar.First(p => p.Date == dt && p.RelayNumber == relayNumber);
                relayTime.HourOn = buf[3 + offset];
                relayTime.MinuteOn = buf[4 + offset];
                relayTime.HourOff = buf[5 + offset];
                relayTime.MinuteOff = buf[6 + offset];

                position++;
            }
        }

        public bool StartSession()
        {
            var result = false;
            try
            {
                _actionSteps.StartSession();
                result = Encoding.ASCII.GetString(_transport.Buffer).Contains("READY");
            }
            catch (LostConnectionException)
            {
                ErrorMessage = "потеря соединения";
                IsLostConnectionRaised = true;
            }
            catch
            {
                ErrorMessage = "получен некорректный ответ";
                IsLostConnectionRaised = true;
            }

            return result;
        }

        public void SetControlMode()
        {
            try
            {
                _actionSteps.SetControlMode();
            }
            catch(LostConnectionException)
            {
                ErrorMessage = "потеря соединения";
                IsLostConnectionRaised = true;
            }
            catch
            {
                ErrorMessage = "получен некорректный ответ";
                IsLostConnectionRaised = true;
            }
        }

        public void ReadIsSetting()
        {
            try
            {
                _actionSteps.ReadIsSetting();

                var buf = _transport.Buffer;

                ModemSettings.IsSetting = BitConverter.ToInt16(new byte[] { buf[4], buf[3] }, 0);
            }
            catch (LostConnectionException)
            {
                ErrorMessage = "потеря соединения";
                IsLostConnectionRaised = true;
            }
            catch
            {
                ErrorMessage = "получен некорректный ответ";
                IsLostConnectionRaised = true;
            }
        }

        public void ReadConfiguration1()
        {
            try
            {
                _actionSteps.ReadConfiguration1();

                var buf = _transport.Buffer;

                ModemSettings.DebugModeId = BitConverter.ToInt16(new byte[] { buf[4], buf[3] }, 0);
                ModemSettings.ModemModeId = BitConverter.ToInt16(new byte[] { buf[6], buf[5] }, 0);
                ModemSettings.NetworkAddress = BitConverter.ToInt16(new byte[] { buf[8], buf[7] }, 0);
            }
            catch (LostConnectionException)
            {
                ErrorMessage = "потеря соединения";
                IsLostConnectionRaised = true;
            }
            catch
            {
                ErrorMessage = "получен некорректный ответ";
                IsLostConnectionRaised = true;
            }
        }

        public bool ReadIdentifierPart1()
        {
            NullTerminatedString result = new NullTerminatedString();
            try
            {
                _actionSteps.ReadIdentifierPart1();

                var buf = _transport.Buffer.GetPackageBody();

                result = buf.GetNullTerminatedString(Encoding.GetEncoding(1251));

                if (!string.IsNullOrEmpty(result.Value))
                {
                    ModemSettings.Identifier += result.Value;
                }
            }
            catch (LostConnectionException)
            {
                ErrorMessage = "потеря соединения";
                IsLostConnectionRaised = true;
            }
            catch
            {
                ErrorMessage = "получен некорректный ответ";
                IsLostConnectionRaised = true;
            }

            return result.IsNullTerminated;
        }

        public bool ReadIdentifierPart2()
        {
            NullTerminatedString result = new NullTerminatedString();
            try
            {
                _actionSteps.ReadIdentifierPart2();

                var buf = _transport.Buffer.GetPackageBody();

                result = buf.GetNullTerminatedString(Encoding.GetEncoding(1251));

                if (!string.IsNullOrEmpty(result.Value))
                {
                    ModemSettings.Identifier += result.Value;
                }
            }
            catch (LostConnectionException)
            {
                ErrorMessage = "потеря соединения";
                IsLostConnectionRaised = true;
            }
            catch
            {
                ErrorMessage = "получен некорректный ответ";
                IsLostConnectionRaised = true;
            }

            return result.IsNullTerminated;
        }

        public bool ReadIdentifierPart3()
        {
            NullTerminatedString result = new NullTerminatedString();

            try
            {
                _actionSteps.ReadIdentifierPart3();

                var buf = _transport.Buffer.GetPackageBody();

                result = buf.GetNullTerminatedString(Encoding.GetEncoding(1251));

                if (!string.IsNullOrEmpty(result.Value))
                {
                    ModemSettings.Identifier += result.Value;
                }
            }
            catch (LostConnectionException)
            {
                ErrorMessage = "потеря соединения";
                IsLostConnectionRaised = true;
            }
            catch
            {
                ErrorMessage = "получен некорректный ответ";
                IsLostConnectionRaised = true;
            }

            return result.IsNullTerminated;
        }

        public bool ReadIdentifierPart4()
        {
            NullTerminatedString result = new NullTerminatedString();

            try
            {
                _actionSteps.ReadIdentifierPart4();

                var buf = _transport.Buffer.GetPackageBody();

                result = buf.GetNullTerminatedString(Encoding.GetEncoding(1251));

                if (!string.IsNullOrEmpty(result.Value))
                {
                    ModemSettings.Identifier += result.Value;
                }
            }
            catch (LostConnectionException)
            {
                ErrorMessage = "потеря соединения";
                IsLostConnectionRaised = true;
            }
            catch
            {
                ErrorMessage = "получен некорректный ответ";
                IsLostConnectionRaised = true;
            }

            return result.IsNullTerminated;
        }

        public void ReadIdentifierPart5()
        {
            try
            {
                _actionSteps.ReadIdentifierPart5();

                var buf = _transport.Buffer.GetPackageBody();

                var result = buf.GetNullTerminatedString(Encoding.GetEncoding(1251));

                if (!string.IsNullOrEmpty(result.Value))
                {
                    ModemSettings.Identifier += result.Value;
                }
            }
            catch (LostConnectionException)
            {
                ErrorMessage = "потеря соединения";
                IsLostConnectionRaised = true;
            }
            catch
            {
                ErrorMessage = "получен некорректный ответ";
                IsLostConnectionRaised = true;
            }
        }


        public void ReadPin()
        {
            try
            {
                _actionSteps.ReadPin();

                var buf = _transport.Buffer;
                ModemSettings.Pin = BitConverter.ToUInt16(new byte[] { buf[4], buf[3] }, 0);
            }
            catch (LostConnectionException)
            {
                ErrorMessage = "потеря соединения";
                IsLostConnectionRaised = true;
            }
            catch
            {
                ErrorMessage = "получен некорректный ответ";
                IsLostConnectionRaised = true;
            }
        }

        public bool ReadApnPart1()
        {
            var result = new NullTerminatedString();
            try
            {
                _actionSteps.ReadApnPart1();

                var buf = _transport.Buffer.GetPackageBody();

                result = buf.GetNullTerminatedString(Encoding.ASCII);

                if (!string.IsNullOrEmpty(result.Value))
                {
                    ModemSettings.Apn += result.Value;
                }
            }
            catch (LostConnectionException)
            {
                ErrorMessage = "потеря соединения";
                IsLostConnectionRaised = true;
            }
            catch
            {
                ErrorMessage = "получен некорректный ответ";
                IsLostConnectionRaised = true;
            }

            return result.IsNullTerminated;
        }

        public void ReadApnPart2()
        {
            try
            {
                _actionSteps.ReadApnPart2();

                var buf = _transport.Buffer.GetPackageBody();

                var result = buf.GetNullTerminatedString(Encoding.ASCII);

                if (!string.IsNullOrEmpty(result.Value))
                {
                    ModemSettings.Apn += result.Value;
                }
            }
            catch (LostConnectionException)
            {
                ErrorMessage = "потеря соединения";
                IsLostConnectionRaised = true;
            }
            catch
            {
                ErrorMessage = "получен некорректный ответ";
                IsLostConnectionRaised = true;
            }
        }

        public void ReadLogin()
        {
            try
            {
                _actionSteps.ReadLogin();
                var buf = _transport.Buffer.GetPackageBody();

                var result = buf.GetNullTerminatedString(Encoding.ASCII);

                if (!string.IsNullOrEmpty(result.Value))
                {
                    ModemSettings.Login = result.Value;
                }
            }
            catch (LostConnectionException)
            {
                ErrorMessage = "потеря соединения";
                IsLostConnectionRaised = true;
            }
            catch
            {
                ErrorMessage = "получен некорректный ответ";
                IsLostConnectionRaised = true;
            }
        }

        public void ReadPassword()
        {
            try
            {
                _actionSteps.ReadPassword();
                var buf = _transport.Buffer.GetPackageBody();

                var result = buf.GetNullTerminatedString(Encoding.ASCII);

                if (!string.IsNullOrEmpty(result.Value))
                {
                    ModemSettings.Password = result.Value;
                }
            }
            catch (LostConnectionException)
            {
                ErrorMessage = "потеря соединения";
                IsLostConnectionRaised = true;
            }
            catch
            {
                ErrorMessage = "получен некорректный ответ";
                IsLostConnectionRaised = true;
            }
        }

        public void ReadListenPortServCount()
        {
            try
            {
                _actionSteps.ReadListenPortServCount();

                var buf = _transport.Buffer;

                ModemSettings.ListenPort = BitConverter.ToUInt16(new byte[] { buf[4], buf[3] }, 0);
                ModemSettings.ServCount = BitConverter.ToInt16(new byte[] { buf[6], buf[5] }, 0);
            }
            catch (LostConnectionException)
            {
                ErrorMessage = "потеря соединения";
                IsLostConnectionRaised = true;
            }
            catch
            {
                ErrorMessage = "получен некорректный ответ";
                IsLostConnectionRaised = true;
            }
        }

        public void ReadAddr_1()
        {
            try
            {
                _actionSteps.ReadAddr_1();
                var buf = _transport.Buffer.GetPackageBody();

                var result = buf.GetNullTerminatedString(Encoding.ASCII);

                if (!string.IsNullOrEmpty(result.Value))
                {
                    ModemSettings.Addr_1 = result.Value;
                }
            }
            catch (LostConnectionException)
            {
                ErrorMessage = "потеря соединения";
                IsLostConnectionRaised = true;
            }
            catch
            {
                ErrorMessage = "получен некорректный ответ";
                IsLostConnectionRaised = true;
            }
        }

        public void ReadPort_1()
        {
            try
            {
                _actionSteps.ReadPort_1();
                var buf = _transport.Buffer;

                ModemSettings.Port_1 = BitConverter.ToUInt16(new byte[] { buf[4], buf[3] }, 0);
            }
            catch (LostConnectionException)
            {
                ErrorMessage = "потеря соединения";
                IsLostConnectionRaised = true;
            }
            catch
            {
                ErrorMessage = "получен некорректный ответ";
                IsLostConnectionRaised = true;
            }
        }

        public void ReadAddr_2()
        {
            try
            {
                _actionSteps.ReadAddr_2();
                var buf = _transport.Buffer.GetPackageBody();

                var result = buf.GetNullTerminatedString(Encoding.ASCII);

                if (!string.IsNullOrEmpty(result.Value))
                {
                    ModemSettings.Addr_2 = result.Value;
                }
            }
            catch (LostConnectionException)
            {
                ErrorMessage = "потеря соединения";
                IsLostConnectionRaised = true;
            }
            catch
            {
                ErrorMessage = "получен некорректный ответ";
                IsLostConnectionRaised = true;
            }
        }

        public void ReadPort_2()
        {
            try
            {
                _actionSteps.ReadPort_2();
                var buf = _transport.Buffer;

                ModemSettings.Port_2 = BitConverter.ToUInt16(new byte[] { buf[4], buf[3] }, 0);
            }
            catch (LostConnectionException)
            {
                ErrorMessage = "потеря соединения";
                IsLostConnectionRaised = true;
            }
            catch
            {
                ErrorMessage = "получен некорректный ответ";
                IsLostConnectionRaised = true;
            }
        }

        public void ReadAddr_3()
        {
            try
            {
                _actionSteps.ReadAddr_3();
                var buf = _transport.Buffer.GetPackageBody();

                var result = buf.GetNullTerminatedString(Encoding.ASCII);

                if (!string.IsNullOrEmpty(result.Value))
                {
                    ModemSettings.Addr_3 = result.Value;
                }
            }
            catch (LostConnectionException)
            {
                ErrorMessage = "потеря соединения";
                IsLostConnectionRaised = true;
            }
            catch
            {
                ErrorMessage = "получен некорректный ответ";
                IsLostConnectionRaised = true;
            }
        }

        public void ReadPort_3()
        {
            try
            {
                _actionSteps.ReadPort_3();
                var buf = _transport.Buffer;

                ModemSettings.Port_3 = BitConverter.ToUInt16(new byte[] { buf[4], buf[3] }, 0);
            }
            catch (LostConnectionException)
            {
                ErrorMessage = "потеря соединения";
                IsLostConnectionRaised = true;
            }
            catch
            {
                ErrorMessage = "получен некорректный ответ";
                IsLostConnectionRaised = true;
            }
        }

        public void ReadSoftwareVersion()
        {
            try
            {
                _actionSteps.ReadSoftwareVersion();
                var buf = _transport.Buffer;

                ModemSettings.SoftwareVersion = BitConverter.ToUInt16(new byte[] { buf[4], buf[3] }, 0);
            }
            catch (LostConnectionException)
            {
                ErrorMessage = "потеря соединения";
                IsLostConnectionRaised = true;
            }
            catch
            {
                ErrorMessage = "получен некорректный ответ";
                IsLostConnectionRaised = true;
            }
        }


        public void ReadCommonSettings()
        {
            try
            {
                _actionSteps.ReadCommonSettings();
                var buf = _transport.Buffer;

                ModemSettings.SelectSerial = BitConverter.ToInt16(new byte[] { buf[4], buf[3] }, 0);
                ModemSettings.BaudRate = BitConverter.ToUInt32(new byte[] { buf[6], buf[5], buf[8], buf[7] }, 0);
                ModemSettings.DataFormat = BitConverter.ToInt16(new byte[] { buf[10], buf[9] }, 0);
                ModemSettings.NmRetry = BitConverter.ToInt16(new byte[] { buf[12], buf[11] }, 0);
                ModemSettings.WaitTm = BitConverter.ToInt16(new byte[] { buf[14], buf[13] }, 0);
                ModemSettings.SendSz = BitConverter.ToInt16(new byte[] { buf[16], buf[15] }, 0);
                ModemSettings.RxMode = BitConverter.ToInt16(new byte[] { buf[18], buf[17] }, 0);
                ModemSettings.RxSize = BitConverter.ToInt16(new byte[] { buf[20], buf[19] }, 0);
                ModemSettings.RxTimer = BitConverter.ToInt16(new byte[] { buf[22], buf[21] }, 0);
            }
            catch (LostConnectionException)
            {
                ErrorMessage = "потеря соединения";
                IsLostConnectionRaised = true;
            }
            catch
            {
                ErrorMessage = "получен некорректный ответ";
                IsLostConnectionRaised = true;
            }
        }

        public void ReadCheckRebootSettings()
        {
            try
            {
                _actionSteps.ReadCheckRebootSettings();
                var buf = _transport.Buffer;

                ModemSettings.CheckPeriod = BitConverter.ToUInt16(new byte[] { buf[4], buf[3] }, 0);
                ModemSettings.TimeForReconnect = BitConverter.ToUInt16(new byte[] { buf[6], buf[5] }, 0);
                ModemSettings.RebootTime = BitConverter.ToUInt16(new byte[] { buf[8], buf[7] }, 0);
            }
            catch (LostConnectionException)
            {
                ErrorMessage = "потеря соединения";
                IsLostConnectionRaised = true;
            }
            catch
            {
                ErrorMessage = "получен некорректный ответ";
                IsLostConnectionRaised = true;
            }
        }
    }
}
