using EtaModemConfigurator.Modbus;
using EtaModemConfigurator.Models;
using EtaModemConfigurator.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EtaModemConfigurator.API
{
    public partial class Functions
    {
        /// <summary>
        /// Возвращает пакет байтов для записи нового идентификатора модема
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] SetIdentifier(string value, int partNumber, bool addNullTerminate)
        {
            var command = partNumber == 1 ? _commands.SetIdentifier : _commands.SetIdentifierPart2;
            return _modbusPackageHelper.GetCommand(_deviceAddress, command,
                data =>
                {
                    List<byte> windows1251 = Encoding.GetEncoding(1251).GetBytes(value).ToList();

                    if (addNullTerminate)
                    {
                        if (windows1251.Count % 2 == 0)
                        {
                            windows1251.Add(0x00);
                            windows1251.Add(0x00);
                        }
                        else
                        {
                            windows1251.Add(0x00);
                        }
                    }
                    var pairReverse = GetPairReverseByteArray(windows1251);
                    data.Data = pairReverse;
                    data.RegistersCount = pairReverse.Length / 2;
                });
        }

        private byte[] GetPairReverseByteArray(List<byte> bytes)
        {
            var byteArray = bytes.ToArray();
            List<byte> pairReverse = new List<byte>();

            for(var i = 0; i < byteArray.Length; i = i + 2)
            {
                pairReverse.Add(byteArray[i + 1]);
                pairReverse.Add(byteArray[i]);
            }

            return pairReverse.ToArray();
        }

        public Action<ModbusFunctionData> GetModbusFunctionData(string value)
        {
            return data =>
            {
                List<byte> ascii = Encoding.ASCII.GetBytes(value).ToList();

                if (ascii.Count % 2 == 0)
                {
                    ascii.Add(0x00);
                    ascii.Add(0x00);
                }
                else
                {
                    ascii.Add(0x00);
                }

                var pairReverse = GetPairReverseByteArray(ascii);
                data.Data = pairReverse;
                data.RegistersCount = pairReverse.Length / 2;
            };
        }

        /// <summary>
        /// Возвращает пакет байтов для записи APN точки доступа оператора связи
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] SetApn(string value)
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.SetApn, GetModbusFunctionData(value));
        }

        /// <summary>
        /// Возвращает пакет байтов для записи логина пользователя
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] SetLogin(string value)
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.SetLogin, GetModbusFunctionData(value));
        }

        /// <summary>
        /// Возвращает пакет байтов для записи пароля пользователя
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] SetPassword(string value)
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.SetPassword, GetModbusFunctionData(value));
        }

        /// <summary>
        /// Возвращает пакет байтов для записи IP-адреса сервера 1
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] SetAddr_1(string value)
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.SetAddr_1, GetModbusFunctionData(value));
        }

        /// <summary>
        /// Возвращает пакет байтов для записи IP-адреса сервера 2
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] SetAddr_2(string value)
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.SetAddr_2, GetModbusFunctionData(value));
        }

        /// <summary>
        /// Возвращает пакет байтов для записи IP-адреса сервера 3
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] SetAddr_3(string value)
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.SetAddr_3, GetModbusFunctionData(value));
        }


        /// <summary>
        /// Возвращает пакет байтов для записи настроек в модем
        /// </summary>
        /// <returns></returns>
        public byte[] SaveSettings(int networkAddress)
        {
            return _modbusPackageHelper.GetCommand(networkAddress, _commands.SaveSettings,
                data =>
                {
                    data.Data = new byte[] { 0x00, 0x00 };
                    data.RegistersCount = 1;
                });
        }

        /// <summary>
        /// Возвращает пакет байтов для записи флага отладочной информации
        /// </summary>
        /// <param name="debugModeId"></param>
        /// <returns></returns>
        public byte[] SetDebugModeId(short debugModeId)
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.SetDebugModeId,
                data =>
                {
                    data.Data = BitConverter.GetBytes(debugModeId).Reverse().ToArray();
                    data.RegistersCount = 1;
                });
        }

        /// <summary>
        /// Возвращает пакет байтов для записи режима работы модема
        /// </summary>
        /// <param name="modemModeId"></param>
        /// <returns></returns>
        public byte[] SetModemModeId(short modemModeId)
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.SetModemModeId,
                data =>
                {
                    data.Data = BitConverter.GetBytes(modemModeId).Reverse().ToArray();
                    data.RegistersCount = 1;
                });
        }

        /// <summary>
        /// Возвращает пакет байтов для записи Modbus-адреса модема
        /// </summary>
        /// <param name="networkAddress"></param>
        /// <returns></returns>
        public byte[] SetNetworkAddress(short networkAddress)
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.SetNetworkAddress,
                data =>
                {
                    data.Data = BitConverter.GetBytes(networkAddress).Reverse().ToArray();
                    data.RegistersCount = 1;
                });
        }

        /// <summary>
        /// Возвращает пакет байтов для записи пин-кода симкарты
        /// </summary>
        /// <param name="pin"></param>
        /// <returns></returns>
        public byte[] SetPin(short pin)
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.SetPin,
                data =>
                {
                    data.Data = BitConverter.GetBytes(pin).Reverse().ToArray();
                    data.RegistersCount = 1;
                });
        }

        /// <summary>
        /// Возвращает пакет байтов для записи порта, прослушиваемого в режиме сервера
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public byte[] SetListenPort(short port)
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.SetListenPort,
                data =>
                {
                    data.Data = BitConverter.GetBytes(port).Reverse().ToArray();
                    data.RegistersCount = 1;
                });
        }

        /// <summary>
        /// Возвращает пакет байтов для записи количества серверов, к которым модем поочередно подключается в режиме клиента
        /// </summary>
        /// <param name="servCount"></param>
        /// <returns></returns>
        public byte[] SetServCount(short servCount)
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.SetServCount,
                data =>
                {
                    data.Data = BitConverter.GetBytes(servCount).Reverse().ToArray();
                    data.RegistersCount = 1;
                });
        }

        /// <summary>
        /// Возвращает пакет байтов для записи порта сервера 1, к которому подключается модем
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public byte[] SetPort_1(ushort port)
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.SetPort_1,
                data =>
                {
                    data.Data = BitConverter.GetBytes(port).Reverse().ToArray();
                    data.RegistersCount = 1;
                });
        }

        /// <summary>
        /// Возвращает пакет байтов для записи порта сервера 2, к которому подключается модем
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public byte[] SetPort_2(ushort port)
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.SetPort_2,
                data =>
                {
                    data.Data = BitConverter.GetBytes(port).Reverse().ToArray();
                    data.RegistersCount = 1;
                });
        }

        /// <summary>
        /// Возвращает пакет байтов для записи порта сервера 3, к которому подключается модем
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public byte[] SetPort_3(ushort port)
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.SetPort_3,
                data =>
                {
                    data.Data = BitConverter.GetBytes(port).Reverse().ToArray();
                    data.RegistersCount = 1;
                });
        }

        /// <summary>
        /// Возвращает пакет байтов для записи выбранного последовательного интерфейса
        /// </summary>
        /// <param name="selectSerial"></param>
        /// <returns></returns>
        public byte[] SetSelectSerial(short selectSerial)
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.SetSelectSerial,
                data =>
                {
                    data.Data = BitConverter.GetBytes(selectSerial).Reverse().ToArray();
                    data.RegistersCount = 1;
                });
        }

        /// <summary>
        /// Возвращает пакет байтов для записи скорости последовательного порта
        /// </summary>
        /// <param name="baudRate"></param>
        /// <returns></returns>
        public byte[] SetBaudRate(uint baudRate)
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.SetBaudRate,
                data =>
                {
                    var bytes = BitConverter.GetBytes(baudRate);
                    data.Data = new byte[] { bytes[1], bytes[0], bytes[3], bytes[2] };
                    data.RegistersCount = 2;
                });
        }

        /// <summary>
        /// Возвращает пакет байтов для записи формата кадра данных последовательного порта
        /// </summary>
        /// <param name="dataFormat"></param>
        /// <returns></returns>
        public byte[] SetDataFormat(short dataFormat)
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.SetDataFormat,
                data =>
                {
                    data.Data = BitConverter.GetBytes(dataFormat).Reverse().ToArray();
                    data.RegistersCount = 1;
                });
        }

        /// <summary>
        /// Возвращает пакет байтов для записи количества повторов доставки IP пакетов
        /// </summary>
        /// <param name="nmRetry"></param>
        /// <returns></returns>
        public byte[] SetNmRetry(short nmRetry)
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.SetNmRetry,
                data =>
                {
                    data.Data = BitConverter.GetBytes(nmRetry).Reverse().ToArray();
                    data.RegistersCount = 1;
                });
        }

        /// <summary>
        /// Возвращает пакет байтов для записи количества ожиданий перед отправкой IP пакета
        /// </summary>
        /// <param name="waitTm"></param>
        /// <returns></returns>
        public byte[] SetWaitTm(short waitTm)
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.SetWaitTm,
                data =>
                {
                    data.Data = BitConverter.GetBytes(waitTm).Reverse().ToArray();
                    data.RegistersCount = 1;
                });
        }

        /// <summary>
        /// Возвращает пакет байтов для записи размера блоках данных, принимаемых с последовательного порта перед отправкой пакета
        /// </summary>
        /// <param name="sendSz"></param>
        /// <returns></returns>
        public byte[] SetSendSz(short sendSz)
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.SetSendSz,
                data =>
                {
                    data.Data = BitConverter.GetBytes(sendSz).Reverse().ToArray();
                    data.RegistersCount = 1;
                });
        }

        /// <summary>
        /// Возвращает пакет байтов для записи использования задержки перед отправкой в последовательный порт
        /// </summary>
        /// <param name="rxMode"></param>
        /// <returns></returns>
        public byte[] SetRxMode(short rxMode)
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.SetRxMode,
                data =>
                {
                    data.Data = BitConverter.GetBytes(rxMode).Reverse().ToArray();
                    data.RegistersCount = 1;
                });
        }

        /// <summary>
        /// Возвращает пакет байтов для записи размера исходящих данных каждого пакета
        /// </summary>
        /// <param name="rxSize"></param>
        /// <returns></returns>
        public byte[] SetRxSize(short rxSize)
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.SetRxSize,
                data =>
                {
                    data.Data = BitConverter.GetBytes(rxSize).Reverse().ToArray();
                    data.RegistersCount = 1;
                });
        }

        /// <summary>
        /// Возвращает пакет байтов для записи задержки принимаемых данных с посл. порта
        /// </summary>
        /// <param name="rxTimer"></param>
        /// <returns></returns>
        public byte[] SetRxTimer(short rxTimer)
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.SetRxTimer,
                data =>
                {
                    data.Data = BitConverter.GetBytes(rxTimer).Reverse().ToArray();
                    data.RegistersCount = 1;
                });
        }

        /// <summary>
        /// Возвращает пакет байтов для записи времени для проверки наличия соединения с удаленной стороной
        /// </summary>
        /// <param name="checkPeriod"></param>
        /// <returns></returns>
        public byte[] SetCheckPeriod(ushort checkPeriod)
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.SetCheckPeriod,
                data =>
                {
                    data.Data = BitConverter.GetBytes(checkPeriod).Reverse().ToArray();
                    data.RegistersCount = 1;
                });
        }

        /// <summary>
        /// Возвращает пакет байтов для записи задержки перед реконнектом
        /// </summary>
        /// <param name="timeForReconnect"></param>
        /// <returns></returns>
        public byte[] SetTimeForReconnect(ushort timeForReconnect)
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.SetTimeForReconnect,
                data =>
                {
                    data.Data = BitConverter.GetBytes(timeForReconnect).Reverse().ToArray();
                    data.RegistersCount = 1;
                });
        }

        /// <summary>
        /// Возвращает пакет байтов для записи таймера перезагрузки
        /// </summary>
        /// <param name="rebootTime"></param>
        /// <returns></returns>
        public byte[] SetRebootTime(ushort rebootTime)
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.SetRebootTime,
                data =>
                {
                    data.Data = BitConverter.GetBytes(rebootTime).Reverse().ToArray();
                    data.RegistersCount = 1;
                });
        }

        public byte[] SetRelayCalendar(RelayTime relayTime)
        {
            var cmd = _commands.SetRelayCalendar;
            cmd.Code = 244 + relayTime.DayNumber * 4;
            return _modbusPackageHelper.GetCommand(_deviceAddress, cmd,
                data =>
                {
                    data.Data = new byte[] { relayTime.HourOn, relayTime.MinuteOn, relayTime.HourOff, relayTime.MinuteOff };
                });
        }

        /// <summary>
        /// Возвращает пакет байтов для выполнения перезагрузки модема
        /// </summary>
        /// <returns></returns>
        public byte[] Reboot()
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.Reboot,
                data =>
                {
                    data.Data = new byte[] { 0x00, 0x01 };
                });
        }

        /// <summary>
        /// Возвращает пакет байтов для выполнения разрыва текущего соединения
        /// </summary>
        /// <returns></returns>
        public byte[] ResetConnection()
        {
            return _modbusPackageHelper.GetCommand(_deviceAddress, _commands.Reboot,
                data =>
                {
                    data.Data = new byte[] { 0x00, 0x02 };
                });
        }
    }
}
