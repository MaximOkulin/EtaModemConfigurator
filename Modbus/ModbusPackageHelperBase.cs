using EtaModemConfigurator.Types;
using System;

namespace EtaModemConfigurator.Modbus
{
    /// <summary>
    /// Базовый класс для формирования пакета байтов согласно протоколу Modbus
    /// </summary>
    public class ModbusPackageHelperBase
    {
        private readonly ModbusMode _modbusMode;

        public ModbusPackageHelperBase()
        {
            _modbusMode = ModbusMode.RTU;
        }

        public ModbusPackageHelperBase(ModbusMode modbusMode)
        {
            _modbusMode = modbusMode;
        }

        /// <summary>
        /// Возвращает пакет в формате Modbus
        /// </summary>
        /// <param name="networkAddress">Сетевой адрес устройства</param>
        /// <param name="cmd">Команда (чтение/запись)</param>
        /// <param name="action">Действие по заполнению сегмента данных пакета</param>
        public byte[] GetCommand(int networkAddress, Command cmd, Action<ModbusFunctionData> action = null)
        {
            if (cmd.CommandType == CommandType.Read)
            {
                return ReadCommand(networkAddress, cmd, cmd.RegistersCount);
            }
            if (cmd.CommandType == CommandType.Write)
            {
                return WriteCommand(networkAddress, cmd, action);
            }
            return null;
        }

        /// <summary>
        /// Возвращает пакет на чтение данных
        /// (должен быть переопределен в наследнике класса)
        /// </summary>
        /// <param name="networkAddress">Сетевой адрес устройства</param>
        /// <param name="cmd">Команда</param>
        /// <param name="registersCount">Количество регистров</param>
        protected virtual byte[] ReadCommand(int networkAddress, Command cmd, int registersCount = 0)
        {
            var command = ModbusProtocol.GetReadRequest(new ModbusFunctionData
            {
                DeviceAddress = networkAddress,
                Function = cmd.ModbusFunctionCode,
                StartingAddress = cmd.Code,
                RegistersCount = registersCount
            }, _modbusMode);

            return command;
        }

        /// <summary>
        /// Возвращает пакет на запись данных (стандартный)
        /// (должен быть переопределен в наследнике класса, если прибор реализует неправильный Modbus)
        /// </summary>
        /// <param name="networkAddress">Сетевой адрес устройства</param>
        /// <param name="cmd">Команда</param>
        /// <param name="action">Действие по заполнению данных пакета</param>
        protected virtual byte[] WriteCommand(int networkAddress, Command cmd, Action<ModbusFunctionData> action = null)
        {
            var functionData = new ModbusFunctionData
            {
                DeviceAddress = networkAddress,
                Function = cmd.ModbusFunctionCode,
                StartingAddress = cmd.Code,
                RegistersCount = cmd.RegistersCount
            };

            if (action != null)
            {
                action(functionData);
            }

            var command = ModbusProtocol.GetWriteRequest(functionData, _modbusMode);

            return command;
        }
    }
}
