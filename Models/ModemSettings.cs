using System;

namespace EtaModemConfigurator.Models
{
    public class ModemSettings : ICloneable
    {
        public short IsSetting { get; set; }
        public short DebugModeId { get; set; }
        public short ModemModeId { get; set; }
        public short NetworkAddress { get; set; }
        public string Identifier { get; set; }
        public ushort Pin { get; set; }
        public string Apn { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public ushort ListenPort { get; set; }
        public short ServCount { get; set; }
        public string Addr_1 { get; set; }
        public ushort Port_1 { get; set; }
        public string Addr_2 { get; set; }
        public ushort Port_2 { get; set; }
        public string Addr_3 { get; set; }
        public ushort Port_3 { get; set; }
        public short SelectSerial { get; set; }
        public uint BaudRate { get; set; }
        public short DataFormat { get; set; }
        public short NmRetry { get; set; }
        public short WaitTm { get; set; }
        public short SendSz { get; set; }
        public short RxMode { get; set; }
        public short RxSize { get; set; }
        public short RxTimer { get; set; }
        public ushort CheckPeriod { get; set; }
        public ushort TimeForReconnect { get; set; }
        public ushort RebootTime { get; set; }
        public ushort SoftwareVersion { get; set; }

        public object Clone()
        {
            return new ModemSettings
            {
                DebugModeId = DebugModeId,
                ModemModeId = ModemModeId,
                NetworkAddress = NetworkAddress,
                Identifier = Identifier,
                Pin = Pin,
                Apn = Apn,
                Login = Login,
                Password = Password,
                ListenPort = ListenPort,
                ServCount = ServCount,
                Addr_1 = Addr_1,
                Port_1 = Port_1,
                Addr_2 = Addr_2,
                Port_2 = Port_2,
                Addr_3 = Addr_3,
                Port_3 = Port_3,
                SelectSerial = SelectSerial,
                BaudRate = BaudRate,
                DataFormat = DataFormat,
                NmRetry = NmRetry,
                WaitTm = WaitTm,
                SendSz = SendSz,
                RxMode = RxMode,
                RxSize = RxSize,
                RxTimer = RxTimer,
                CheckPeriod = CheckPeriod,
                TimeForReconnect = TimeForReconnect,
                RebootTime = RebootTime,
                SoftwareVersion = SoftwareVersion
            };
        }
    }
}
