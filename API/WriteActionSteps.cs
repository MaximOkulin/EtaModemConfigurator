using EtaModemConfigurator.Models;
using EtaModemConfigurator.Types;

namespace EtaModemConfigurator.API
{
    public partial class ActionSteps
    {
        [SetMethod("идентификатор")]
        public void SetIdentifier(string value)
        {
            Transport.CurrentCommand = _commands.SetIdentifier;

            if (value.Length < 40)
            {
                Transport.Send(_functions.SetIdentifier(value, 1, true), true);
                Wait();
            }
            else if (value.Length == 40)
            {
                Transport.Send(_functions.SetIdentifier(value, 1, false), true);
                Wait();
                Transport.Send(_functions.SetIdentifier(string.Empty, 2, true), true);
                Wait();
            }
            else if (value.Length > 40)
            {
                Transport.Send(_functions.SetIdentifier(value.Substring(0, 40), 1, false), true);
                Wait();
                Transport.Send(_functions.SetIdentifier(value.Substring(40, value.Length - 40), 2, true), true);
                Wait();
            }
        }

        public void SaveSettings(int networkAddress)
        {
            Transport.CurrentCommand = _commands.SaveSettings;
            _functions.DeviceAddress = networkAddress;
            Transport.Send(_functions.SaveSettings(networkAddress), true);
            Wait();
        }

        [SetMethod("флаг отладки")]
        public void SetDebugModeId(short debugModeId)
        {
            Transport.CurrentCommand = _commands.SetDebugModeId;
            Transport.Send(_functions.SetDebugModeId(debugModeId), true);
            Wait();
        }

        [SetMethod("режим работы")]
        public void SetModemModeId(short modemModeId)
        {
            Transport.CurrentCommand = _commands.SetModemModeId;
            Transport.Send(_functions.SetModemModeId(modemModeId), true);
            Wait();
        }

        [SetMethod("Modbus-адрес")]
        public void SetNetworkAddress(short networkAddress)
        {
            Transport.CurrentCommand = _commands.SetNetworkAddress;
            Transport.Send(_functions.SetNetworkAddress(networkAddress), true);
            Wait();
        }

        [SetMethod("пин-код симкарты")]
        public void SetPin(short pin)
        {
            Transport.CurrentCommand = _commands.SetPin;
            Transport.Send(_functions.SetPin(pin), true);
            Wait();
        }

        [SetMethod("APN точки доступа")]
        public void SetApn(string value)
        {
            Transport.CurrentCommand = _commands.SetApn;
            Transport.Send(_functions.SetApn(value), true);
            Wait();
        }

        [SetMethod("логин пользователя")]
        public void SetLogin(string value)
        {
            Transport.CurrentCommand = _commands.SetLogin;
            Transport.Send(_functions.SetLogin(value), true);
            Wait();
        }

        [SetMethod("пароль пользователя")]
        public void SetPassword(string value)
        {
            Transport.CurrentCommand = _commands.SetPassword;
            Transport.Send(_functions.SetPassword(value), true);
            Wait();
        }

        [SetMethod("порт прослушки")]
        public void SetListenPort(short port)
        {
            Transport.CurrentCommand = _commands.SetListenPort;
            Transport.Send(_functions.SetListenPort(port), true);
            Wait();
        }

        [SetMethod("кол-во серверов")]
        public void SetServCount(short servCount)
        {
            Transport.CurrentCommand = _commands.SetServCount;
            Transport.Send(_functions.SetServCount(servCount), true);
            Wait();
        }

        [SetMethod("IP-адрес сервера 1")]
        public void SetAddr_1(string value)
        {
            Transport.CurrentCommand = _commands.SetAddr_1;
            Transport.Send(_functions.SetAddr_1(value), true);
            Wait();
        }

        [SetMethod("IP-адрес сервера 2")]
        public void SetAddr_2(string value)
        {
            Transport.CurrentCommand = _commands.SetAddr_2;
            Transport.Send(_functions.SetAddr_2(value), true);
            Wait();
        }

        [SetMethod("IP-адрес сервера 3")]
        public void SetAddr_3(string value)
        {
            Transport.CurrentCommand = _commands.SetAddr_3;
            Transport.Send(_functions.SetAddr_3(value), true);
            Wait();
        }

        [SetMethod("порт сервера 1")]
        public void SetPort_1(ushort port)
        {
            Transport.CurrentCommand = _commands.SetPort_1;
            Transport.Send(_functions.SetPort_1(port), true);
            Wait();
        }

        [SetMethod("порт сервера 2")]
        public void SetPort_2(ushort port)
        {
            Transport.CurrentCommand = _commands.SetPort_2;
            Transport.Send(_functions.SetPort_2(port), true);
            Wait();
        }

        [SetMethod("порт сервера 3")]
        public void SetPort_3(ushort port)
        {
            Transport.CurrentCommand = _commands.SetPort_3;
            Transport.Send(_functions.SetPort_3(port), true);
            Wait();
        }

        [SetMethod("посл. интерфейс")]
        public void SetSelectSerial(short selectSerial)
        {
            Transport.CurrentCommand = _commands.SetSelectSerial;
            Transport.Send(_functions.SetSelectSerial(selectSerial), true);
            Wait();
        }

        [SetMethod("скорость порта")]
        public void SetBaudRate(uint baudRate)
        {
            Transport.CurrentCommand = _commands.SetBaudRate;
            Transport.Send(_functions.SetBaudRate(baudRate), true);
            Wait();
        }

        [SetMethod("формат кадра")]
        public void SetDataFormat(short dataFormat)
        {
            Transport.CurrentCommand = _commands.SetDataFormat;
            Transport.Send(_functions.SetDataFormat(dataFormat), true);
            Wait();
        }

        [SetMethod("повторы доставки")]
        public void SetNmRetry(short nmRetry)
        {
            Transport.CurrentCommand = _commands.SetNmRetry;
            Transport.Send(_functions.SetNmRetry(nmRetry), true);
            Wait();
        }

        [SetMethod("количество ожиданий")]
        public void SetWaitTm(short waitTm)
        {
            Transport.CurrentCommand = _commands.SetWaitTm;
            Transport.Send(_functions.SetWaitTm(waitTm), true);
            Wait();
        }

        [SetMethod("размер блока данных")]
        public void SetSendSz(short sendSz)
        {
            Transport.CurrentCommand = _commands.SetSendSz;
            Transport.Send(_functions.SetSendSz(sendSz), true);
            Wait();
        }

        [SetMethod("задержка перед отправкой")]
        public void SetRxMode(short rxMode)
        {
            Transport.CurrentCommand = _commands.SetRxMode;
            Transport.Send(_functions.SetRxMode(rxMode), true);
            Wait();
        }

        [SetMethod("размер пакета")]
        public void SetRxSize(short rxSize)
        {
            Transport.CurrentCommand = _commands.SetRxSize;
            Transport.Send(_functions.SetRxSize(rxSize), true);
            Wait();
        }

        [SetMethod("задержка приёма")]
        public void SetRxTimer(short rxTimer)
        {
            Transport.CurrentCommand = _commands.SetRxTimer;
            Transport.Send(_functions.SetRxTimer(rxTimer), true);
            Wait();
        }

        [SetMethod("время проверки")]
        public void SetCheckPeriod(ushort checkPeriod)
        {
            Transport.CurrentCommand = _commands.SetCheckPeriod;
            Transport.Send(_functions.SetCheckPeriod(checkPeriod), true);
            Wait();
        }

        [SetMethod("задержка реконнекта")]
        public void SetTimeForReconnect(ushort timeForReconnect)
        {
            Transport.CurrentCommand = _commands.SetTimeForReconnect;
            Transport.Send(_functions.SetTimeForReconnect(timeForReconnect), true);
            Wait();
        }

        [SetMethod("таймер перезагрузки")]
        public void SetRebootTime(ushort rebootTime)
        {
            Transport.CurrentCommand = _commands.SetRebootTime;
            Transport.Send(_functions.SetRebootTime(rebootTime), true);
            Wait();
        }

        /// <summary>
        /// Перезагрузка модема
        /// </summary>
        public void Reboot()
        {
            Transport.CurrentCommand = _commands.Reboot;
            Transport.Send(_functions.Reboot(), true);
        }

        /// <summary>
        /// Сброс текущего соединения
        /// </summary>
        public void ResetConnection()
        {
            Transport.CurrentCommand = _commands.ResetConnection;
            Transport.Send(_functions.ResetConnection());
        }

        /// <summary>
        /// Запись новых значений времени включения/отключения реле
        /// </summary>
        /// <param name="dayNumber">Номер дня</param>
        /// <param name="hourOn">Час включения</param>
        /// <param name="minuteOn">Минута включения</param>
        /// <param name="hourOff">Час отключения</param>
        /// <param name="minuteOff">Минута отключения</param>
        public void SetRelayCalendar(RelayTime relayTime)
        {
            Transport.CurrentCommand = _commands.SetRelayCalendar;
            Transport.Send(_functions.SetRelayCalendar(relayTime));
            Wait();
        }
    }
}
