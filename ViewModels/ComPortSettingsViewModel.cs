using System.IO.Ports;
using EtaModemConfigurator.Commands;
using System.Windows.Input;
using System.Threading.Tasks;
using EtaModemConfigurator.Transport;

namespace EtaModemConfigurator.ViewModels
{
    public class ComPortSettingsViewModel : CommonTransportViewModel
    {
        private RelayCommand _connectCommand;

        public ICommand ConnectCommand
        {
            get
            {
                if (_connectCommand == null)
                {
                    _connectCommand = new RelayCommand(param => this.Connect(), null);
                }
                return _connectCommand;
            }
        }

        private void Connect()
        {
            WaitIndicatorState = true;
            OperationName = "Поиск модема";
            CurrentOperationName = "ожидание команды READY...";

            new Task((() => StartSession())).Start();
        }

        public ComPortSettingsViewModel()
        {
            IsAirConnected = false;
            CommonTransport = new CommonTransport(Types.TransportTypes.Direct);
           
            ComPortName = Settings.LocalComPortName;
            NetworkAddress = Settings.NetworkAddress;            
        }

        private string[] _comPortList;

        public string[] ComPortList
        {
            get
            {
                if (_comPortList == null)
                {
                    _comPortList = SerialPort.GetPortNames();
                }
                return _comPortList;
            }
        }
        

        public string ComPortName
        {
            get
            {
                return CommonTransport.ComPortName;
            }
            set
            {
                CommonTransport.ComPortName = value;
                Settings.LocalComPortName = value;
                Settings.SaveSettings();
                OnPropertyChanged("ComPortName");
            }
        }
    }
}
