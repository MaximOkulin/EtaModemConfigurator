using EtaModemConfigurator.Commands;
using System.Net.Sockets;
using System.Windows.Input;

namespace EtaModemConfigurator.ViewModels
{
    public class ClientViewModel : CommonTransportViewModel
    {
        private string _address;
        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                Settings.ClientAddress = value;
                Settings.SaveSettings();
                OnPropertyChanged("Address");
            }
        }

        private int _port;
        public int Port
        {
            get => _port;
            set
            {
                _port = value;
                Settings.ClientPort = value;
                Settings.SaveSettings();
                OnPropertyChanged("Port");
            }
        }

        public ClientViewModel()
        {
            IsAirConnected = true;
            CommonTransport = new Transport.CommonTransport(Types.TransportTypes.TCP);

            Address = Settings.ClientAddress;
            Port = Settings.ClientPort;
            IsEnabledConnectBtn = true;
        }

        private RelayCommand _connectCommand;

        public ICommand ConnectCommand
        {
            get
            {
                if (_connectCommand == null)
                {
                    _connectCommand = new RelayCommand(param => Connect());
                }
                return _connectCommand;
            }
        }

        private void Connect()
        {
            IsEnabledConnectBtn = false;
            IsNeedToCloseConnection = true;
            StartSession(_address, _port);
        }
    }
}
