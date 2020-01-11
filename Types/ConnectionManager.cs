using EtaModemConfigurator.Modbus;
using EtaModemConfigurator.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace EtaModemConfigurator.Types
{
    public class ConnectionManager
    {
        private IPEndPoint _ipEndPoint;
        private TcpListenerEx _tcpListener;

        private ItemsChangeObservableCollection<ModemInfo> _modemInfos;
        private static object _modemInfoLocker = new object();
        private Timer _maintainConnectionsTimer;

        public ItemsChangeObservableCollection<ModemInfo> ModemInfos
        {
            get => _modemInfos;
        }

        public ConnectionManager(string listenAddress, int port)
        {
            _ipEndPoint = new IPEndPoint(IPAddress.Parse(listenAddress), port);
            _modemInfos = new ItemsChangeObservableCollection<ModemInfo>();
        }

        public void StartListener()
        {
            if(_tcpListener == null)
            {
                _tcpListener = new TcpListenerEx(_ipEndPoint);
            }

            _tcpListener.Start();

            if (_maintainConnectionsTimer == null)
            {
                _maintainConnectionsTimer = new Timer(MaintainConnections, null, 10000, 11000);
            }

            Task.Run(() => AcceptClients());
        }

        public void StopListener()
        {
            if (_tcpListener != null)
            {
                _tcpListener.Stop();

                if (_maintainConnectionsTimer != null)
                {
                    _maintainConnectionsTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    _maintainConnectionsTimer = null;
                }
                lock (_modemInfoLocker)
                {
                    _modemInfos.ToList().ForEach(p =>
                    {
#if DEBUG
                        //Debug.WriteLine("Перезагрузка модема: " + p.Identifier);
#endif
                        try
                        {
                            var networkStream = p.TcpClient.GetStream();
                            p.TcpClient.SendTimeout = 7000;
                            networkStream.Write(p.ResetConnectionPackage, 0, 8);
                        }
                        catch
                        {

                        }
                    });
                }
            }
        }

        private async void AcceptClients()
        {
            while (_tcpListener.Active)
            {
                try
                {
                    var context = await _tcpListener.AcceptTcpClientAsync();
                    ProcessTcpClientAsync(context);
                }
                catch (Exception ex)
                {
                }
            }
        }

        private void ProcessTcpClientAsync(TcpClient tcpClient)
        {
            var stateObject = new StateObject();
            try
            {
                stateObject.TcpClient = tcpClient;
                var stream = tcpClient.GetStream();
                int bytesReaded = stream.Read(stateObject.TcpBuffer, 0, stateObject.TcpBuffer.Length);
                ReceiveIdentifierCallback(stateObject, bytesReaded);
            }
            catch (IOException)
            {
                tcpClient.GetStream().Close();
                tcpClient.Close();
            }
        }

        private void ReceiveIdentifierCallback(StateObject stateObject, int bytesReaded)
        {
            try
            {
                var state = stateObject;

                if (bytesReaded > 0)
                {
                    string modemIdentity = Encoding.GetEncoding(1251).GetString(state.TcpBuffer, 0, bytesReaded);
                    var identityParts = modemIdentity.Split('\r', '\n');

                    var match = Regex.Match(identityParts[2], @"CSQ: (\d{2}),");

                    int signalLevel = -1;
                    if (match.Groups.Count == 2)
                    {
                        signalLevel = Convert.ToInt16(match.Groups[1].Value);
                    }

                    int networkAddress = 254;

                    if (identityParts.Count() == 7)
                    {
                        networkAddress = Convert.ToInt32(identityParts[4]);
                    }

                    ModemInfo modemInfo = null;
                    lock (_modemInfoLocker)
                    {
                        modemInfo = _modemInfos.FirstOrDefault(p => p.Identifier.Equals(identityParts[0], StringComparison.Ordinal));
                    }

                    if (modemInfo == null)
                    {
                        // формируем команду запрос текущей версии прошивки
                        var packageHelper = new ModbusPackageHelperBase();
                        var getSoftwarePackage = packageHelper.GetCommand(networkAddress, new API.Commands().ReadSoftware);

                        var networkStream = state.TcpClient.GetStream();
                        state.TcpBuffer = new byte[1024];
                        state.TcpClient.ReceiveTimeout = 7000;
                        state.TcpClient.SendTimeout = 7000;

                        networkStream.Write(getSoftwarePackage, 0, 8);

                        ushort softwareVersion = 0;
                        try
                        {
                            networkStream.Read(state.TcpBuffer, 0, state.TcpBuffer.Length);
                            softwareVersion = BitConverter.ToUInt16(new byte[] { state.TcpBuffer[4], state.TcpBuffer[3] }, 0);
                        }
                        catch
                        {

                        }

                        lock (_modemInfoLocker)
                        {
                            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                _modemInfos.Add(new ModemInfo(networkAddress)
                                {
                                    Identifier = identityParts[0],
                                    SignalLevel = signalLevel.GetSignalLevelInDb(),
                                    IsBusy = false,
                                    TcpClient = state.TcpClient,
                                    UpdateDate = DateTime.Now,
                                    SoftwareVersion = softwareVersion,
                                    Model = softwareVersion == 100 ? "модем" :
                                            softwareVersion == 200 ? "модем+реле" : string.Empty
                                });
                            }));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var k = ex;
            }
        }

        private void MaintainConnections(object state)
        {
            _maintainConnectionsTimer.Change(Timeout.Infinite, Timeout.Infinite);
            List<ModemInfo> _freeModems = null;

            try
            {
                lock (_modemInfoLocker)
                {
                    _freeModems = _modemInfos.Where(p => !p.IsBusy).ToList();
                }
            }
            catch
            {
                _freeModems = null;
            }

            if (_freeModems != null)
            {
                _freeModems.ForEach(p =>
               {
                   if (!p.IsBusy)
                   {
#if DEBUG
                       //Debug.WriteLine("поддержка связи с " + p.Identifier);
#endif
                       var responseBuf = new byte[11];
                       var networkStream = p.TcpClient.GetStream();
                       p.TcpClient.ReceiveTimeout = 7000;
                       p.TcpClient.SendTimeout = 7000;

                       try
                       {
                           networkStream.Write(p.GetSignalLevelPackage, 0, 8);

                           for (int offset = 0; offset < 11; offset += networkStream.Read(responseBuf, offset, 11 - offset), Thread.Sleep(250)) ;

                           var buf = responseBuf.GetPackageBody();
                           var signalQuality = buf.GetNullTerminatedString(Encoding.GetEncoding(1251)).Value;

                           Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                           {
                               int sQ = int.MinValue;
                               try
                               {
                                   sQ = Convert.ToInt32(signalQuality.Split(',')[0]);
                               }
                               catch { }

                               if (sQ != int.MinValue)
                               {
                                   p.SignalLevel = Convert.ToInt32(sQ).GetSignalLevelInDb();
                                   p.UpdateDate = DateTime.Now;
                               }
                           }));
                       }
                       catch (Exception ex)
                       {
                           var e = ex;
                           lock (_modemInfoLocker)
                           {
                               if (_modemInfos.Contains(p))
                               {
                                   Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                                   {
                                       _modemInfos.Remove(p);
                                   }));
                               }
                           }
                       }
                   }
                   else
                   {
#if DEBUG
                      // Debug.WriteLine("модем {0} занят", p.Identifier);
#endif
                   }
               });
            }

            if (_maintainConnectionsTimer != null)
            {
#if DEBUG
               //Debug.WriteLine("Запуск таймера");
#endif
                _maintainConnectionsTimer.Change(7000, 11000);
            }
        }

        public void SetModemBusy(string identifier, bool state)
        {
            lock (_modemInfoLocker)
            {
                var modem = _modemInfos.FirstOrDefault(p => p.Identifier.Equals(identifier));
                if (modem != null)
                {
                    modem.IsBusy = state;
                }
            }
        }

        public void RemoveModemFromList(string identifier)
        {
            lock(_modemInfoLocker)
            {
                var modem = _modemInfos.FirstOrDefault(p => p.Identifier.Equals(identifier));
                 _modemInfos.Remove(modem);
            }
        }
    }
}
