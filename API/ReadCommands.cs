using EtaModemConfigurator.Types;

namespace EtaModemConfigurator.API
{
    public partial class Commands
    {
        public Command StartSession = new Command { CommandName = "StartSession", IsReadWithoutWrite = true, ResponseLengthType = LengthType.Fixed, ResponseLength = 5 };

        public Command SetControlMode = new Command { CommandName = "SetControlMode", IsWriteWithoutRead = true };

        /// <summary>
        /// Чтение флага настройки контроллера
        /// </summary>
        public Command ReadIsSetting = new Command { CommandName = "ReadIsSetting", Code = 0, CommandType = CommandType.Read, RegistersCount = 1, ModbusFunctionCode = ModbusFunction.ReadHoldingRegisters, ResponseLengthType = LengthType.Calculated };

        /// <summary>
        /// Чтение первой секции конфигурации (Debug, ModemMode, NetworkAddress)
        /// </summary>
        public Command ReadConfiguration1 = new Command { CommandName = "ReadConfiguration1", Code = 2, CommandType = CommandType.Read, RegistersCount = 3, ModbusFunctionCode = ModbusFunction.ReadHoldingRegisters, ResponseLengthType = LengthType.Calculated };
        /// <summary>
        /// Чтение первой части идентификатора модема (14 символов [1-14])
        /// </summary>
        public Command ReadIdentifierPart1 = new Command { CommandName = "ReadIdentifierPart1", Code = 8, CommandType = CommandType.Read, RegistersCount = 7, ModbusFunctionCode = ModbusFunction.ReadHoldingRegisters, ResponseLengthType = LengthType.Calculated };
        /// <summary>
        /// Чтение второй части идентификатора модема (14 символов [15-28])
        /// </summary>
        public Command ReadIdentifierPart2 = new Command { CommandName = "ReadIdentifierPart2", Code = 22, CommandType = CommandType.Read, RegistersCount = 7, ModbusFunctionCode = ModbusFunction.ReadHoldingRegisters, ResponseLengthType = LengthType.Calculated };
        /// <summary>
        /// Чтение третьей части идентифкатора модема (14 символов [29-42])
        /// </summary>
        public Command ReadIdentifierPart3 = new Command { CommandName = "ReadIdentifierPart3", Code = 36, CommandType = CommandType.Read, RegistersCount = 7, ModbusFunctionCode = ModbusFunction.ReadHoldingRegisters, ResponseLengthType = LengthType.Calculated };
        /// <summary>
        /// Чтение четвертой части идентифкатора модема (14 символов [43-56])
        /// </summary>
        public Command ReadIdentifierPart4 = new Command { CommandName = "ReadIdentifierPart4", Code = 50, CommandType = CommandType.Read, RegistersCount = 7, ModbusFunctionCode = ModbusFunction.ReadHoldingRegisters, ResponseLengthType = LengthType.Calculated };
        /// <summary>
        /// Чтение пятой части идентифкатора модема (8 символов [57-64])
        /// </summary>
        public Command ReadIdentifierPart5 = new Command { CommandName = "ReadIdentifierPart5", Code = 64, CommandType = CommandType.Read, RegistersCount = 4, ModbusFunctionCode = ModbusFunction.ReadHoldingRegisters, ResponseLengthType = LengthType.Calculated };
        /// <summary>
        /// Чтение пин-кода симкарты
        /// </summary>
        public Command ReadPin = new Command { CommandName = "ReadPin", Code = 72, CommandType = CommandType.Read, RegistersCount = 1, ModbusFunctionCode = ModbusFunction.ReadHoldingRegisters, ResponseLengthType = LengthType.Calculated };
        /// <summary>
        /// Чтение APN точки доступа оператора связи часть 1 (первые 16 символов)
        /// </summary>
        public Command ReadApnPart1 = new Command { CommandName = "ReadApnPart1", Code = 74, CommandType = CommandType.Read, RegistersCount = 8, ModbusFunctionCode = ModbusFunction.ReadHoldingRegisters, ResponseLengthType = LengthType.Calculated };
        /// <summary>
        /// Чтение APN точки доступа оператора связи часть 2 (остальные 16 символов)
        /// </summary>
        public Command ReadApnPart2 = new Command { CommandName = "ReadApnPart2", Code = 90, CommandType = CommandType.Read, RegistersCount = 8, ModbusFunctionCode = ModbusFunction.ReadHoldingRegisters, ResponseLengthType = LengthType.Calculated };
        /// <summary>
        /// Чтение логина пользователя
        /// </summary>
        public Command ReadLogin = new Command { CommandName = "ReadLogin", Code = 106, CommandType = CommandType.Read, RegistersCount = 10, ModbusFunctionCode = ModbusFunction.ReadHoldingRegisters, ResponseLengthType = LengthType.Calculated };
        /// <summary>
        /// Чтение пароля пользователя
        /// </summary>
        public Command ReadPassword = new Command { CommandName = "ReadPassword", Code = 126, CommandType = CommandType.Read, RegistersCount = 10, ModbusFunctionCode = ModbusFunction.ReadHoldingRegisters, ResponseLengthType = LengthType.Calculated };
        /// <summary>
        /// Чтение порта и количества серверов в режиме клиента
        /// </summary>
        public Command ReadListenPortServCount = new Command { CommandName = "ReadListenPortServCount", Code = 146, CommandType = CommandType.Read, RegistersCount = 2, ModbusFunctionCode = ModbusFunction.ReadHoldingRegisters, ResponseLengthType = LengthType.Calculated };
        /// <summary>
        /// Чтение IP-адреса сервера 1
        /// </summary>
        public Command ReadAddr_1 = new Command { CommandName = "ReadAddr_1", Code = 150, CommandType = CommandType.Read, RegistersCount = 8, ModbusFunctionCode = ModbusFunction.ReadHoldingRegisters, ResponseLengthType = LengthType.Calculated };
        /// <summary>
        /// Чтение порта сервера 1
        /// </summary>
        public Command ReadPort_1 = new Command { CommandName = "ReadPort_1", Code = 166, CommandType = CommandType.Read, RegistersCount = 1, ModbusFunctionCode = ModbusFunction.ReadHoldingRegisters, ResponseLengthType = LengthType.Calculated };
        /// <summary>
        /// Чтение IP-адреса сервера 2
        /// </summary>
        public Command ReadAddr_2 = new Command { CommandName = "ReadAddr_2", Code = 168, CommandType = CommandType.Read, RegistersCount = 8, ModbusFunctionCode = ModbusFunction.ReadHoldingRegisters, ResponseLengthType = LengthType.Calculated };
        /// <summary>
        /// Чтение порта сервера 2
        /// </summary>
        public Command ReadPort_2 = new Command { CommandName = "ReadPort_2", Code = 184, CommandType = CommandType.Read, RegistersCount = 1, ModbusFunctionCode = ModbusFunction.ReadHoldingRegisters, ResponseLengthType = LengthType.Calculated };
        /// <summary>
        /// Чтение IP-адреса сервера 3
        /// </summary>
        public Command ReadAddr_3 = new Command { CommandName = "ReadAddr_3", Code = 186, CommandType = CommandType.Read, RegistersCount = 8, ModbusFunctionCode = ModbusFunction.ReadHoldingRegisters, ResponseLengthType = LengthType.Calculated };
        /// <summary>
        /// Чтение порта сервера 3
        /// </summary>
        public Command ReadPort_3 = new Command { CommandName = "ReadPort_3", Code = 202, CommandType = CommandType.Read, RegistersCount = 1, ModbusFunctionCode = ModbusFunction.ReadHoldingRegisters, ResponseLengthType = LengthType.Calculated };
        /// <summary>
        /// Чтение основного массива настроек
        /// </summary>
        public Command ReadCommonSettings = new Command { CommandName = "ReadCommonSettings", Code = 204, CommandType = CommandType.Read, RegistersCount = 10, ModbusFunctionCode = ModbusFunction.ReadHoldingRegisters, ResponseLengthType = LengthType.Calculated };
        /// <summary>
        /// Чтение настроек проверки и задержки
        /// </summary>
        public Command ReadCheckRebootSettings = new Command { CommandName = "ReadCheckRebootSettings", Code = 224, CommandType = CommandType.Read, RegistersCount = 3, ModbusFunctionCode = ModbusFunction.ReadHoldingRegisters, ResponseLengthType = LengthType.Calculated };
        /// <summary>
        /// Чтение текущего уровня сигнала
        /// </summary>
        public Command ReadSignalQuality = new Command { CommandName = "ReadSignalQuality", Code = 18, CommandType = CommandType.Read, RegistersCount = 3, ModbusFunctionCode = ModbusFunction.ReadInputRegisters, ResponseLengthType = LengthType.Calculated };

        /// <summary>
        /// Чтение текущей версии прошивки
        /// </summary>
        public Command ReadSoftware = new Command { CommandName = "ReadSoftware", Code = 24, CommandType = CommandType.Read, RegistersCount = 1, ModbusFunctionCode = ModbusFunction.ReadInputRegisters, ResponseLengthType = LengthType.Fixed, ResponseLength = 7 };
        /// <summary>
        /// Чтение календаря включения/отключения реле
        /// </summary>
        public Command ReadCalendar = new Command { CommandName = "ReadCalendar", CommandType = CommandType.Read, ModbusFunctionCode = ModbusFunction.ReadHoldingRegisters, ResponseLengthType = LengthType.Calculated };
    }
}
