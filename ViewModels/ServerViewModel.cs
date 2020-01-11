using EtaModemConfigurator.Commands;
using EtaModemConfigurator.Transport;
using EtaModemConfigurator.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using EtaModemConfigurator.Views;
using DevExpress.Xpf.WindowsUI;
using System.Windows.Controls;

namespace EtaModemConfigurator.ViewModels
{
    public class TcpListenerEx : TcpListener
    {
        public TcpListenerEx(IPEndPoint ipEndPoint) : base(ipEndPoint)
        {

        }

        public bool Active
        {
            get
            {
                return base.Active;
            }
        }
    }
    public class ServerViewModel : CommonTransportViewModel
    {
        private string[] _localIpAddressList;

        public string[] LocalIpAddressList
        {
            get
            {
                if (_localIpAddressList == null)
                {
                    var host = Dns.GetHostEntry(Dns.GetHostName());
                    var lst = new List<string>();
                    foreach (var ip in host.AddressList)
                    {
                        if (ip.AddressFamily == AddressFamily.InterNetwork)
                        {
                            lst.Add(ip.ToString());
                        }
                    }

                    _localIpAddressList = lst.ToArray();

                }
                return _localIpAddressList;
            }
        }

        public string LocalListenAddress
        {
            get
            {
                return CommonTransport.LocalListenAddress;
            }
            set
            {
                CommonTransport.LocalListenAddress = value;
                Settings.LocalListenAddress = value;
                Settings.SaveSettings();
                OnPropertyChanged("LocalListenAddress");
            }
        }

        public int LocalListenPort
        {
            get
            {
                return CommonTransport.LocalListenPort;
            }
            set
            {
                CommonTransport.LocalListenPort = value;
                Settings.LocalListenPort = value;
                Settings.SaveSettings();
                OnPropertyChanged("LocalListenPort");
            }
        }

        private string _identifier;

        public string Identifier
        {
            get => _identifier;
            set
            {
                _identifier = value;
                Settings.Identifier = value;
                Settings.SaveSettings();
                OnPropertyChanged("Identifier");
            }
        }

        private bool _isEnabledStartServerBtn;

        public bool IsEnabledStartServerBtn
        {
            get => _isEnabledStartServerBtn;
            set
            {
                _isEnabledStartServerBtn = value;
                OnPropertyChanged("IsEnabledStartServerBtn");
            }
        }



        public ServerViewModel()
        {
            IsAirConnected = true;
            CommonTransport = new CommonTransport(TransportTypes.TCP);

            LocalListenAddress = Settings.LocalListenAddress;
            LocalListenPort = Settings.LocalListenPort;
            NetworkAddress = Settings.NetworkAddress;
            Identifier = Settings.Identifier;
            IsEnabledStartServerBtn = true;
        }

        private RelayCommand _startServerCommand;

        public ICommand StartServerCommand
        {
            get
            {
                if (_startServerCommand == null)
                {
                    _startServerCommand = new RelayCommand(param => StartListenServer(), null);
                }
                return _startServerCommand;
            }
        }

        private TcpListenerEx _tcpListener;
        private void StartListenServer()
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                var allConnectionGridView = new AllConnectionGridView();
                allConnectionGridView.DataContext = new AllConnectionGridViewModel(LocalListenAddress, LocalListenPort);
                ((Application.Current.MainWindow.Content as Grid).Children[0] as NavigationFrame).Navigate(allConnectionGridView);
            }));
        }

        private async void AcceptClients()
        {
            while (true && _tcpListener.Active)
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

                tcpClient.GetStream()
                    .BeginRead(stateObject.TcpBuffer, 0, stateObject.TcpBuffer.Length, ReceiveIdentifierCallback,
                        stateObject);
            }
            catch (IOException)
            {
                tcpClient.GetStream().Close();
                tcpClient.Close();
            }
        }

        private void ReceiveIdentifierCallback(IAsyncResult ar)
        {
            try
            {
                var state = (StateObject)ar.AsyncState;
                var bytesReaded = state.TcpClient.GetStream().EndRead(ar);

                if (bytesReaded > 0)
                {
                    string barsIdentity = Encoding.GetEncoding(1251).GetString(state.TcpBuffer, 0, bytesReaded);
                    var identityParts = barsIdentity.Split('\r', '\n');

                    if (identityParts[0].Equals(_identifier, StringComparison.Ordinal))
                    {
                        _tcpListener.Stop();
                        StartSession(state.TcpClient);
                    }
                    else
                    {
                        state.TcpClient.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
