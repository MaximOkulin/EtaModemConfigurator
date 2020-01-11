using EtaModemConfigurator.Modbus;
using System;

namespace EtaModemConfigurator.API
{
    public partial class Functions
    {
        private int _deviceAddress;
        private readonly Commands _commands;
        private readonly ModbusPackageHelperBase _modbusPackageHelper;

        public int DeviceAddress
        {
            set
            {
                _deviceAddress = value;
            }
        }

        public Functions(int deviceAddress)
        {
            _deviceAddress = deviceAddress;
            _commands = new Commands();
            _modbusPackageHelper = new ModbusPackageHelperBase();
        }

        public byte[] ReadConfiguration1()
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.ReadConfiguration1);
        }

        public byte[] SetControlMode()
        {
            return new byte[] { 0x43, 0x4d, 0x44 };
        }

        public byte[] ReadIsSetting()
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.ReadIsSetting);
        }

        public byte[] ReadIdentifierPart1()
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.ReadIdentifierPart1);
        }

        public byte[] ReadIdentifierPart2()
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.ReadIdentifierPart2);
        }

        public byte[] ReadIdentifierPart3()
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.ReadIdentifierPart3);
        }

        public byte[] ReadIdentifierPart4()
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.ReadIdentifierPart4);
        }

        public byte[] ReadIdentifierPart5()
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.ReadIdentifierPart5);
        }

        public byte[] ReadPin()
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.ReadPin);
        }

        public byte[] ReadApnPart1()
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.ReadApnPart1);
        }

        public byte[] ReadApnPart2()
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.ReadApnPart2);
        }

        public byte[] ReadLogin()
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.ReadLogin);
        }

        public byte[] ReadPassword()
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.ReadPassword);
        }

        public byte[] ReadListenPortServCount()
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.ReadListenPortServCount);
        }

        public byte[] ReadAddr_1()
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.ReadAddr_1);
        }

        public byte[] ReadPort_1()
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.ReadPort_1);
        }

        public byte[] ReadAddr_2()
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.ReadAddr_2);
        }

        public byte[] ReadPort_2()
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.ReadPort_2);
        }

        public byte[] ReadAddr_3()
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.ReadAddr_3);
        }

        public byte[] ReadPort_3()
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.ReadPort_3);
        }

        public byte[] ReadCommonSettings()
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.ReadCommonSettings);
        }

        public byte[] ReadCheckRebootSettings()
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.ReadCheckRebootSettings);
        }

        public byte[] ReadSignalQuality()
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.ReadSignalQuality);
        }

        public byte[] ReadSoftwareVersion()
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.ReadSoftware);
        }

        public byte[] ReadCalendar(DateTime startDate, DateTime endDate, int startDayNumber)
        {
            var cmd = _commands.ReadCalendar;
            cmd.RegistersCount = ((endDate - startDate).Days + 1) * 2;
            cmd.Code = 244 + startDayNumber * 4;

            return _modbusPackageHelper.GetCommand(_deviceAddress, cmd);
        }
    }
}
