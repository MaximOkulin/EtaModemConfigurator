using EtaModemConfigurator.Modbus;
using EtaModemConfigurator.ViewModels;
using System;
using System.ComponentModel;
using System.Net.Sockets;

namespace EtaModemConfigurator.Types
{
    public class ModemInfo : ViewModelBase
    {
        private API.Commands _commands;

        private byte[] _getSignalLevelPackage;
        public byte[] GetSignalLevelPackage
        {
            get => _getSignalLevelPackage;
        }

        private byte[] _resetConnectionPackage;
        public byte[] ResetConnectionPackage
        {
            get => _resetConnectionPackage;
        }

        public ModemInfo(int networkAddress)
        {
            NetworkAddress = networkAddress;
            _commands = new API.Commands();

            var packageHelper = new ModbusPackageHelperBase();
            _getSignalLevelPackage = packageHelper.GetCommand(networkAddress, _commands.ReadSignalQuality);
            _resetConnectionPackage = packageHelper.GetCommand(networkAddress, _commands.Reboot,
                data =>
                {
                    data.Data = new byte[] { 0x00, 0x02 };
                });
        }

        private string _identifier;
        public string Identifier {
            get => _identifier;
            set
            {
                _identifier = value;
                OnPropertyChanged("Identifier");
            }
        }

        private int _signalLevel;
        public int SignalLevel {
            get => _signalLevel;
            set
            {
                _signalLevel = value;
                OnPropertyChanged("SignalLevel");
            }
        }

        private DateTime _updateDate;
        public DateTime UpdateDate {
            get => _updateDate;
            set
            {
                _updateDate = value;
                OnPropertyChanged("UpdateDate");
            }
        }

        private volatile TcpClient _tcpClient;
        public TcpClient TcpClient {
            get => _tcpClient;
            set
            {
                _tcpClient = value;
                OnPropertyChanged("TcpClient");
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged("IsBusy");
            }
        }

        private int _softwareVersion;
        public int SoftwareVersion
        {
            get => _softwareVersion;
            set
            {
                _softwareVersion = value;
                OnPropertyChanged("SoftwareVersion");
            }
        }

        private string _model;

        public string Model
        {
            get => _model;
            set
            {
                _model = value;
                OnPropertyChanged("Model");
            }
        }

        private int _networkAddress;
        public int NetworkAddress
        {
            get => _networkAddress;
            set
            {
                _networkAddress = value;
                OnPropertyChanged("NetworkAddress");
            }
        }
    }
}
