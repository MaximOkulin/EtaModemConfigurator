using EtaModemConfigurator.Exceptions;
using EtaModemConfigurator.Types;
using System.Threading;


namespace EtaModemConfigurator.Base
{
    public class ActionStepsBase
    {
        public Transport.Transport Transport;
        protected ManualResetEvent AutoEvent;


        public ActionStepsBase(Transport.Transport transport, ManualResetEvent autoEvent)
        {
            Transport = transport;
            AutoEvent = autoEvent;
        }


        protected bool Wait()
        {
            //
            if (Transport.TransportType == TransportTypes.Direct)
            {
                AutoEvent.WaitOne();
                AutoEvent.Reset();
            }

            if (Transport.IsLostConnectionRaised)
            {
                if (Transport.CurrentCommand.CanBeSkip == CanBeSkip.Yes)
                {
                    return false;
                }

                throw new LostConnectionException(Transport.CurrentCommand.ErrorMessage);
            }
            if (Transport.CurrentCommand.IsGenerateErrorResponseException)
            {
                if (Transport.CurrentErrorCode != ErrorCode.None)
                {
                
                }
            }

            return true;
        }
    }
}
