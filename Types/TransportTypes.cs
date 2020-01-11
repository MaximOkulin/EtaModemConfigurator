using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EtaModemConfigurator.Types
{
    public enum TransportTypes
    {
        Direct = 1, // прямое подключение через COM-порт
        TCP = 2 // через сеть TCP/IP
    }
}
