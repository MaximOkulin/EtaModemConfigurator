using System.IO.Ports;
using System.Net.Sockets;

namespace EtaModemConfigurator.Types
{
    public class ConnectionInfo
    {
        public int AmountAttempt;
        public int ReceiveTimeOut;
        public string NetAddress;
        public string NetPhone;
        public int Port;
        public string SerialPortName;
        public int BaudRate;
        public Parity Parity;
        public int DataBits;
        public StopBits StopBits;
        public TcpClient PreparedTcpClient;
    }
}
