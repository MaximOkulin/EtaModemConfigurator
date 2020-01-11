using EtaModemConfigurator.Types;
using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace EtaModemConfigurator.Transport
{
    public partial class Transport
    {
        /// <summary>
        /// Инициализирует подключение к прибору через последовательный порт
        /// </summary>
        /// <returns>True - успех; False - ошибка</returns>
        private bool SetComPortSettings()
        {
            _port = new SerialPort()
            {
                PortName = _connectionInfo.SerialPortName,
                BaudRate = _connectionInfo.BaudRate,
                Parity = _connectionInfo.Parity,
                StopBits = _connectionInfo.StopBits,
                DataBits = _connectionInfo.DataBits
            };
            _port.DataReceived += _port_DataReceived;
            try
            {
                _port.ReadTimeout = 500;
                _port.WriteTimeout = 20;
                _port.Open();
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Закрывает последовательный порт, если используется прямое подключение
        /// </summary>
        public void CloseSerialPort()
        {
            if ((TransportType == TransportTypes.Direct) && _port != null)
            {
                if (_port.IsOpen)
                {
                    // отключаем обработчик события приема данных
                    _port.DataReceived -= _port_DataReceived;
                    _port.Close();
                    _port.Dispose();
#if DEBUG
                    Console.WriteLine("COM port closed");
#endif
                }
            }
        }

        private int _serialPortReceiveDelay = 500;

        public int SerialPortReceiveDelay
        {
            set
            {
                _serialPortReceiveDelay = value;
            }
        }

        /// <summary>
        /// Обрабатывает прием данных из последовательного порта
        /// </summary>
        private void _port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (!_isLostConnectionRaised)
            {
                StopWaitTimer();

                Thread.Sleep(_serialPortReceiveDelay);

                bool successReceive = true;

                if (!_port.IsOpen)
                {
                    RaiseLostConnection(false);
                }
                else
                {
                    try
                    {
                        // циклом читаем данные из порта
                        do
                        {
                            if (_port.IsOpen)
                            {
                                State.BytesReaded = 0;
                                State.ComPortBuffer.Clear();

                                var bytesToRead = _port.BytesToRead;

#if DEBUG
                                Debug.WriteLine("Bytes to read: " + bytesToRead);
#endif
                                if (bytesToRead > 0)
                                {
                                    var _byte = new byte[bytesToRead];
                                    _port.Read(_byte, 0, bytesToRead);

                                    for (var i = 0; i < bytesToRead; i++)
                                    {
                                        State.BytesReaded++;
                                        State.ComPortBuffer.Add(_byte[i]);
                                    }
                                }

                                if (CurrentCommand.CommandName.Equals("StartSession", StringComparison.Ordinal))
                                {
                                    var responseText = Encoding.ASCII.GetString(State.ComPortBuffer.ToArray());
                                    State.BytesReaded = 0;

#if DEBUG
                                    if (responseText.Length > 1)
                                    {
                                        Debug.WriteLine(responseText);
                                    }
#endif

                                    if (responseText.Contains("READY") && responseText.Length == 7)
                                    {
                                        State.BytesReaded = 5;
                                        break;
                                    }
                                    else
                                    {
                                        Thread.Sleep(1000);
                                    }
                                }

                                if (!CurrentCommand.CommandName.Equals("StartSession", StringComparison.Ordinal) &&
                                    State.IsFirstIteration && State.BytesReaded >= 3)
                                {
                                    FirstIteration(State.ComPortBuffer.ToArray());
                                }
                            }
                            else
                            {
                                RaiseLostConnection(false);
                                successReceive = false;
                                break;
                            }

                        } while ((_port.BytesToRead > 0 || State.BytesReaded < State.TotalBytes || State.BytesReaded < 3) 
                                  && CurrentErrorCode == ErrorCode.None);
                    }
                    catch
                    {
                        RaiseLostConnection(false);
                        successReceive = false;
                    }

                    

                    if (!CurrentCommand.CommandName.Equals("StartSession", StringComparison.Ordinal) && !VerifyCheckSum())
                    {
                        RaiseLostConnection(false);
                        successReceive = false;
                    }

                    if (!CurrentCommand.CommandName.Equals("StartSession", StringComparison.Ordinal) && !CheckNetworkAddress())
                    {
                        RaiseLostConnection(false);
                        successReceive = false;
                    }


                    if (successReceive)
                    {
                        StopWaitTimer();
                        RaiseReceiveDataCompleteEvent();
                        AutoEvent.Set();
                    }
                }
            }
        }
    }
}
