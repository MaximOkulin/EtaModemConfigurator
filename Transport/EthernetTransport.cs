using EtaModemConfigurator.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace EtaModemConfigurator.Transport
{
    public partial class Transport
    {
        private TcpClient _tcpClient;
        public TcpClient TcpClient
        {
            get => _tcpClient;
            set
            {
                _tcpClient = value;
            }
        }

        /// <summary>
        /// Инициализирует TCP-подключение к прибору
        /// </summary>
        /// <param name="preparedTcpClient">Готовый Tcp-клиент</param>
        /// <returns>True - успех; False - ошибка</returns>
        private bool SetEthernetSettings(TcpClient preparedTcpClient = null)
        {
            bool connectionResult = false;

            // попытка подключения
            try
            {
                if (preparedTcpClient != null)
                {
                    _tcpClient = preparedTcpClient;
                }
                else
                {
                    _tcpClient = new TcpClient(_connectionInfo.NetAddress, _connectionInfo.Port);
                }

                _tcpClient.SendTimeout = 11000;
                _tcpClient.ReceiveTimeout = 11000;

                _stream = _tcpClient.GetStream();
                
                connectionResult = true;
            }
            catch(ArgumentNullException)
            {
                connectionResult = false;
            }
            catch (SocketException)
            {
                connectionResult = false;
            }
            
            if (_stream != null)
            {
                
            }
            return connectionResult;
        }

        private void ReadData()
        {
            try
            {
#if FAKEPOLLING
                State.BytesReaded = CurrentCommand.FakeResponse.Length;
                State.TcpBuffer = CurrentCommand.FakeResponse;
#endif
#if !FAKEPOLLING
                var byteList = new List<byte>();

                do
                {
                    var currentBytesRead = _stream.Read(State.TcpBuffer, 0, State.TcpBuffer.Length);
                    State.BytesReaded += currentBytesRead;
                    byteList.AddRange(State.TcpBuffer.Take(currentBytesRead));

                    if (CurrentCommand.ResponseLengthType == LengthType.Fixed || CurrentCommand.ResponseLengthType == LengthType.Calculated)
                    {
                        if (State.IsFirstIteration)
                        {
                            FirstIteration(byteList.ToArray());
                        }

                        if (!State.IsFirstIteration)
                        {
                            if (State.BytesReaded >= State.TotalBytes)
                            {
                                break;
                            }
                            else
                            {
                                Thread.Sleep(1000);
                            }
                        }
                        else
                        {
                            Thread.Sleep(1000);
                        }
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }

                } while (_stream.DataAvailable);

                State.TcpBuffer = byteList.ToArray();
#endif

#if DEBUG
                Debug.WriteLine("Ответ:");
                Debug.WriteLine(BitConverter.ToString(State.TcpBuffer));
#endif


                if (CurrentCommand.ResponseLengthType == LengthType.Format)
                {
                    FirstIteration(State.TcpBuffer);
                }


                if (State.BytesReaded >= State.TotalBytes)
                {
                    // все байты пришли нулевые
                    if (State.TcpBuffer.All(p => p == 0x00))
                    {
#if DEBUG
                        Debug.WriteLine("пришли нулевые байты");
#endif
                        RaiseLostConnection(false);
                    }
                    else if (!VerifyCheckSum())
                    {
                        WrongCheckSumCount++;
                        RaiseLostConnection(false);
                    }
                    else if (!CheckNetworkAddress())
                    {
                        RaiseLostConnection(false);
                    }
                    else
                    {
                        TotalBytesRead += State.BytesReaded;
                        RaiseReceiveDataCompleteEvent();
                    }
                }
                else
                {
                    RaiseLostConnection(false);
                }
            }
            catch (IOException ex)
            {
                var socketException = ex.InnerException as SocketException;
                if (socketException != null && socketException.SocketErrorCode == SocketError.TimedOut)
                {
                    RaiseLostConnection(false);
                }
                else
                {
                    RaiseLostConnection(true);
                }
            }
        }

        public void CloseTcpClient()
        {
            if (TransportType == TransportTypes.TCP)
            {
                if (_stream != null)
                {
                    _stream.Close();
                    _stream.Dispose();
                }
                if (_tcpClient != null)
                {
                    _tcpClient.Close();
                }
            }
        }
    }
}
