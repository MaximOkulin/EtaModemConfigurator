using EtaModemConfigurator.Types;
namespace EtaModemConfigurator.API
{
    public partial class Commands
    {
        /// <summary>
        /// Запись нового идентификатора модема
        /// </summary>
        public Command SetIdentifier = new Command { CommandName = "SetIdentifier", CommandType = CommandType.Write, Code = 8, ModbusFunctionCode = ModbusFunction.PresetMultipleRegisters, ResponseLengthType = LengthType.Fixed, ResponseLength = 8 };
        /// <summary>
        /// Запись второй части идентификатора модема
        /// </summary>
        public Command SetIdentifierPart2 = new Command { CommandName = "SetIdentifierPart2", CommandType = CommandType.Write, Code = 48, ModbusFunctionCode = ModbusFunction.PresetMultipleRegisters, ResponseLengthType = LengthType.Fixed, ResponseLength = 8 };
        /// <summary>
        /// Запись флага отладочной информации
        /// </summary>
        public Command SetDebugModeId = new Command { CommandName = "SetDebugModeId", CommandType = CommandType.Write, Code = 2, ModbusFunctionCode = ModbusFunction.PresetMultipleRegisters, ResponseLengthType = LengthType.Fixed, ResponseLength = 8 };
        /// <summary>
        /// Запись режима работы модема
        /// </summary>
        public Command SetModemModeId = new Command { CommandName = "SetModemModeId", CommandType = CommandType.Write, Code = 4, ModbusFunctionCode = ModbusFunction.PresetMultipleRegisters, ResponseLengthType = LengthType.Fixed, ResponseLength = 8 };
        /// <summary>
        /// Запись Modbus-адреса модема по умолчанию
        /// </summary>
        public Command SetNetworkAddress = new Command { CommandName = "SetNetworkAddress", CommandType = CommandType.Write, Code = 6, ModbusFunctionCode = ModbusFunction.PresetMultipleRegisters, ResponseLengthType = LengthType.Fixed, ResponseLength = 8 };
        /// <summary>
        /// Запись пин-кода симкарты
        /// </summary>
        public Command SetPin = new Command { CommandName = "SetPin", CommandType = CommandType.Write, Code = 0x0048, ModbusFunctionCode = ModbusFunction.PresetMultipleRegisters, ResponseLengthType = LengthType.Fixed, ResponseLength = 8 };
        /// <summary>
        /// Запись APN точки доступа оператора связи
        /// </summary>
        public Command SetApn = new Command { CommandName = "SetApn", CommandType = CommandType.Write, Code = 0x004A, ModbusFunctionCode = ModbusFunction.PresetMultipleRegisters, ResponseLengthType = LengthType.Fixed, ResponseLength = 8 };
        /// <summary>
        /// Запись логина пользователя
        /// </summary>
        public Command SetLogin = new Command { CommandName = "SetLogin", CommandType = CommandType.Write, Code = 0x006A, ModbusFunctionCode = ModbusFunction.PresetMultipleRegisters, ResponseLengthType = LengthType.Fixed, ResponseLength = 8 };
        /// <summary>
        /// Запись пароля пользователя
        /// </summary>
        public Command SetPassword = new Command { CommandName = "SetPassword", CommandType = CommandType.Write, Code = 0x007E, ModbusFunctionCode = ModbusFunction.PresetMultipleRegisters, ResponseLengthType = LengthType.Fixed, ResponseLength = 8 };
        /// <summary>
        /// Запись порта, прослушиваемого в режиме сервера
        /// </summary>
        public Command SetListenPort = new Command { CommandName = "SetListenPort", CommandType = CommandType.Write, Code = 0x0092, ModbusFunctionCode = ModbusFunction.PresetMultipleRegisters, ResponseLengthType = LengthType.Fixed, ResponseLength = 8 };
        /// <summary>
        /// Количество серверов для подключения
        /// </summary>
        public Command SetServCount = new Command { CommandName = "SetServCount", CommandType = CommandType.Write, Code = 0x0094, ModbusFunctionCode = ModbusFunction.PresetMultipleRegisters, ResponseLengthType = LengthType.Fixed, ResponseLength = 8 };
        /// <summary>
        /// Запись IP-адреса сервера 1
        /// </summary>
        public Command SetAddr_1 = new Command { CommandName = "SetAddr_1", CommandType = CommandType.Write, Code = 0x0096, ModbusFunctionCode = ModbusFunction.PresetMultipleRegisters, ResponseLengthType = LengthType.Fixed, ResponseLength = 8 };
        /// <summary>
        /// Запись порта сервера 1
        /// </summary>
        public Command SetPort_1 = new Command { CommandName = "SetPort_1", CommandType = CommandType.Write, Code = 0x00A6, ModbusFunctionCode = ModbusFunction.PresetMultipleRegisters, ResponseLengthType = LengthType.Fixed, ResponseLength = 8 };

        /// <summary>
        /// Запись IP-адреса сервера 2
        /// </summary>
        public Command SetAddr_2 = new Command { CommandName = "SetAddr_2", CommandType = CommandType.Write, Code = 0x00A8, ModbusFunctionCode = ModbusFunction.PresetMultipleRegisters, ResponseLengthType = LengthType.Fixed, ResponseLength = 8 };
        /// <summary>
        /// Запись порта сервера 2
        /// </summary>
        public Command SetPort_2 = new Command { CommandName = "SetPort_2", CommandType = CommandType.Write, Code = 0x00B8, ModbusFunctionCode = ModbusFunction.PresetMultipleRegisters, ResponseLengthType = LengthType.Fixed, ResponseLength = 8 };

        /// <summary>
        /// Запись IP-адреса сервера 3
        /// </summary>
        public Command SetAddr_3 = new Command { CommandName = "SetAddr_3", CommandType = CommandType.Write, Code = 0x00BA, ModbusFunctionCode = ModbusFunction.PresetMultipleRegisters, ResponseLengthType = LengthType.Fixed, ResponseLength = 8 };
        /// <summary>
        /// Запись порта сервера 3
        /// </summary>
        public Command SetPort_3 = new Command { CommandName = "SetPort_3", CommandType = CommandType.Write, Code = 0x00CA, ModbusFunctionCode = ModbusFunction.PresetMultipleRegisters, ResponseLengthType = LengthType.Fixed, ResponseLength = 8 };
        /// <summary>
        /// Запись выбранного последовательного интерфейса
        /// </summary>
        public Command SetSelectSerial = new Command { CommandName = "SetSelectSerial", CommandType = CommandType.Write, Code = 0x00CC, ModbusFunctionCode = ModbusFunction.PresetMultipleRegisters, ResponseLengthType = LengthType.Fixed, ResponseLength = 8 };
        /// <summary>
        /// Запись скорости порта
        /// </summary>
        public Command SetBaudRate = new Command { CommandName = "SetBaudRate", CommandType = CommandType.Write, Code = 0x00CE, ModbusFunctionCode = ModbusFunction.PresetMultipleRegisters, ResponseLengthType = LengthType.Fixed, ResponseLength = 8 };
        /// <summary>
        /// Запись формата кадра данных последовательного порта
        /// </summary>
        public Command SetDataFormat = new Command { CommandName = "SetDataFormat", CommandType = CommandType.Write, Code = 0x00D2, ModbusFunctionCode = ModbusFunction.PresetMultipleRegisters, ResponseLengthType = LengthType.Fixed, ResponseLength = 8 };
        /// <summary>
        /// Запись количества повторов доставки IP пакетов
        /// </summary>
        public Command SetNmRetry = new Command { CommandName = "SetNmRetry", CommandType = CommandType.Write, Code = 0x00D4, ModbusFunctionCode = ModbusFunction.PresetMultipleRegisters, ResponseLengthType = LengthType.Fixed, ResponseLength = 8 };
        /// <summary>
        /// Запись количества ожиданий перед отправкой IP пакета
        /// </summary>
        public Command SetWaitTm = new Command { CommandName = "SetWaitTm", CommandType = CommandType.Write, Code = 0x00D6, ModbusFunctionCode = ModbusFunction.PresetMultipleRegisters, ResponseLengthType = LengthType.Fixed, ResponseLength = 8 };
        /// <summary>
        /// Запись размера блока данных, принимаемых с последовательного порта перед отправкой пакета
        /// </summary>
        public Command SetSendSz = new Command { CommandName = "SetSendSz", CommandType = CommandType.Write, Code = 0x00D8, ModbusFunctionCode = ModbusFunction.PresetMultipleRegisters, ResponseLengthType = LengthType.Fixed, ResponseLength = 8 };
        /// <summary>
        /// Запись использования задержки перед отправкой в последовательный порт
        /// </summary>
        public Command SetRxMode = new Command { CommandName = "SetRxMode", CommandType = CommandType.Write, Code = 0x00DA, ModbusFunctionCode = ModbusFunction.PresetMultipleRegisters, ResponseLengthType = LengthType.Fixed, ResponseLength = 8 };
        /// <summary>
        /// Запись размера исходящих данных каждого пакета
        /// </summary>
        public Command SetRxSize = new Command { CommandName = "SetRxSize", CommandType = CommandType.Write, Code = 0x00DC, ModbusFunctionCode = ModbusFunction.PresetMultipleRegisters, ResponseLengthType = LengthType.Fixed, ResponseLength = 8 };
        /// <summary>
        /// Запись задержки принимаемых данных с посл. порта
        /// </summary>
        public Command SetRxTimer = new Command { CommandName = "SetRxTimer", CommandType = CommandType.Write, Code = 0x00DE, ModbusFunctionCode = ModbusFunction.PresetMultipleRegisters, ResponseLengthType = LengthType.Fixed, ResponseLength = 8 };
        /// <summary>
        /// Запись времени для проверки наличия соединения
        /// </summary>
        public Command SetCheckPeriod = new Command { CommandName = "SetCheckPeriod", CommandType = CommandType.Write, Code = 0x00E0, ModbusFunctionCode = ModbusFunction.PresetMultipleRegisters, ResponseLengthType = LengthType.Fixed, ResponseLength = 8 };
        /// <summary>
        /// Запись задержки перед реконнектом
        /// </summary>
        public Command SetTimeForReconnect = new Command { CommandName = "SetTimeForReconnect", CommandType = CommandType.Write, Code = 0x00E2, ModbusFunctionCode = ModbusFunction.PresetMultipleRegisters, ResponseLengthType = LengthType.Fixed, ResponseLength = 8 };
        /// <summary>
        /// Запись таймера перезагрузки
        /// </summary>
        public Command SetRebootTime = new Command { CommandName = "SetRebootTime", CommandType = CommandType.Write, Code = 0x00E4, ModbusFunctionCode = ModbusFunction.PresetMultipleRegisters, ResponseLengthType = LengthType.Fixed, ResponseLength = 8 };

        /// <summary>
        /// Сохранение настроек
        /// </summary>
        public Command SaveSettings = new Command { CommandName = "SaveSettings", CommandType = CommandType.Write, Code = 0, ModbusFunctionCode = ModbusFunction.PresetMultipleRegisters, ResponseLengthType = LengthType.Fixed, ResponseLength = 8 };
        /// <summary>
        /// Перезагрузка модема
        /// </summary>
        public Command Reboot = new Command { CommandName = "Reboot", CommandType = CommandType.Write, Code = 0, ModbusFunctionCode = ModbusFunction.PresetSingleRegister, RegistersCount = 1, ResponseLengthType = LengthType.Fixed, ResponseLength = 8 };

        /// <summary>
        /// Разрыв текущего соединения
        /// </summary>
        public Command ResetConnection = new Command { CommandName = "ResetConnection", CommandType = CommandType.Write, Code = 0, ModbusFunctionCode = ModbusFunction.PresetSingleRegister, RegistersCount = 1, ResponseLengthType = LengthType.Fixed, ResponseLength = 8, IsWriteWithoutRead = true };

        /// <summary>
        /// Запись времени включения/выключения реле
        /// </summary>
        public Command SetRelayCalendar = new Command { CommandName = "SetRelayCalendar", CommandType = CommandType.Write, ModbusFunctionCode = ModbusFunction.PresetMultipleRegisters, RegistersCount = 2, ResponseLengthType = LengthType.Fixed, ResponseLength = 8 };
    }
}
