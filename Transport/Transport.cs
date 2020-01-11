using System;
using System.IO;
using System.IO.Ports;
using System.Net.Sockets;
using System.Threading;
using System.Linq;
using Timer = System.Timers.Timer;
using EtaModemConfigurator.Types;
using System.Diagnostics;

namespace EtaModemConfigurator.Transport
{
    /// <summary>
    /// Базовый класс, реализующий транспортную функцию до устройств
    /// </summary>
    public partial class Transport
    {
        public TransportTypes TransportType { get; set; }
        private SerialPort _port;
        private NetworkStream _stream;
        public NetworkStream Stream
        {
            get => _stream;
            set => _stream = value;
        }
        

        public ManualResetEvent AutoEvent;
        protected StateObject State;
        public bool IsCsdCommandMode;

        public int TotalBytesWrite { get; set; }
        public int TotalBytesRead { get; set; }       

        /// <summary>
        /// Флаг инициации ручного сброса опроса
        /// </summary>
        public bool IsManualReset = false;

        private ConnectionInfo _connectionInfo;
        public Command CurrentCommand { get; set; }
        
        // флаг - был ли произведен сброс соединения
        private bool _isLostConnectionRaised;
        
        protected byte[] CurrentRequest;
        private const int MaxRequestAttempts = 5;

        /// <summary>
        /// Таймер, ожидающий приём данных от прибора
        /// </summary>
        public Timer ResponseWaitTimer;

        private int _currentRequestAttemptsCount = 1;
        private bool _useAttemptsInFail;

        public int GoodResponseCount = 0;
        public int BadResponseCount = 0;
        public int WrongCheckSumCount = 0;

        public ErrorCode CurrentErrorCode { get; set; }

        public delegate void ReceiveDataCompleteHandler(object sender, System.EventArgs e);

        // событие "Приём данных завершен"
        public event ReceiveDataCompleteHandler ReceiveDataComplete;

        /// <summary>
        /// Возбуждает событие завершения приема данных
        /// </summary>
        protected virtual void RaiseReceiveDataCompleteEvent()
        {
            GoodResponseCount++;
            if (ReceiveDataComplete != null)
            {
                ReceiveDataComplete(this, null);
            }
        }

        /// <summary>
        /// Строковое представление текущего запроса
        /// </summary>
        public string CurrentRequestDump
        {
            get
            {
                return string.Format("0x{0}", BitConverter.ToString(CurrentRequest));
            }
        }

        public string ErrorText { get; set; }
        

        public bool IsLostConnectionRaised
        {
            get { return _isLostConnectionRaised; }
            set { _isLostConnectionRaised = value; }
        }

        /// <summary>
        /// Буфер данных, полученных от прибора 
        /// </summary>
        public byte[] Buffer
        {
            get
            {
                if (TransportType == TransportTypes.Direct)
                {
                    return State.ComPortBuffer.Take(State.BytesReaded).ToArray();
                }
                if (TransportType == TransportTypes.TCP)
                {
                    return State.TcpBuffer.Take(State.BytesReaded).ToArray();
                }
                return null;
            }
        }


        public Transport()
        {
        }

        public void SetNewAutoEvent(ManualResetEvent autoEvent)
        {
            AutoEvent = autoEvent;
        }


        /// <summary>
        /// Возбуждает событие потери связи
        /// </summary>
        protected virtual void RaiseLostConnection(bool isFatal = false, string errorMessage = null)
        {
            _isLostConnectionRaised = true;

            if (ResponseWaitTimer != null)
            {
                ResponseWaitTimer.Stop();

                BadResponseCount++;

                if ((_useAttemptsInFail && _currentRequestAttemptsCount < MaxRequestAttempts && !isFatal))
                {
#if DEBUG
                    Debug.WriteLine("повторная отправка команды: " + CurrentCommand.CommandName);
#endif
                    ReSendCommand();
                }
                else
                {
                    CurrentErrorCode = CurrentErrorCode == ErrorCode.None ? ErrorCode.LossConnection : CurrentErrorCode;

                    if (CurrentCommand.ErrorMessageCreator != null)
                    {
                        CurrentCommand.ErrorMessage = CurrentCommand.ErrorMessageCreator(CurrentCommand);
                    }

                    if (!string.IsNullOrEmpty(CurrentCommand.ErrorMessage))
                    {
                        //CurrentCommand.ErrorMessage = string.Format("{0}. {1}", errorMessage ?? DeviceMessages.FailConnectionStatus, CurrentCommand.ErrorMessage);
                    }

                    if (TransportType == TransportTypes.Direct)
                    {
                        AutoEvent.Set();
                    }
                }
            }
        }
    

        /// <summary>
        /// Стандартный таймаут ожидания ответа от прибора
        /// </summary>
        private int _timeOut;
        /// <summary>
        /// Инициализирует соединение с удалённым прибором
        /// </summary>
        /// <returns>true - успех; false - неудача</returns>
        public bool InitConnection(int transportType, ConnectionInfo connectionInfo)
        {
            _connectionInfo = connectionInfo;

            if (connectionInfo.ReceiveTimeOut > 0)
            {
                _timeOut = connectionInfo.ReceiveTimeOut;
            }
            else
            {
                _timeOut = 35000;
            }
            

            ResponseWaitTimer = new Timer
            {
                Interval = _timeOut
            };
            ResponseWaitTimer.Elapsed += _responseWaitTimer_Elapsed;

            switch (transportType)
            {
                case (int) TransportTypes.TCP:
                    TransportType = TransportTypes.TCP;
                    return SetEthernetSettings(connectionInfo.PreparedTcpClient);
                case (int) TransportTypes.Direct:
                    TransportType = TransportTypes.Direct;
                    return SetComPortSettings();              
            }

            return false;
        }

        public virtual bool InitConnection()
        {
            return false;
        }
        /// <summary>
        /// Изменяет интервал ожидания
        /// </summary>
        /// <param name="newTimeOut">Новое значение таймаута</param>
        private void ChangeWaitTimeOut(int? newTimeOut)
        {
            if (TransportType == TransportTypes.TCP)
            {
                if (newTimeOut != null)
                {
                    _stream.ReadTimeout = newTimeOut.Value;
                }
                else
                {
                    _stream.ReadTimeout = _timeOut;
                }
            }

            if (TransportType == TransportTypes.Direct)
            {
                if (ResponseWaitTimer != null)
                {
                    if (newTimeOut != null)
                    {
                        ResponseWaitTimer.Interval = newTimeOut.Value;
                        _timeOut = newTimeOut.Value;
                    }
                    else
                    {
                        ResponseWaitTimer.Interval = _timeOut;
                    }
                }
            }
        }

        public void Send(object state, bool useAttemptsInFail = false, int? timeOut = null)
        {
            ChangeWaitTimeOut(timeOut);

            _currentRequestAttemptsCount = 1;
            _useAttemptsInFail = useAttemptsInFail;

            var data = (byte[])state;

            CurrentRequest = data;

            if (State == null)
            {
                State = new StateObject();
            }

            SendCommand();
        }

        private void ReSendCommand()
        {
            if (State == null)
            {
                State = new StateObject();
            }
            // увеличиваем счетчик попыток отправки команды в прибор
            _currentRequestAttemptsCount++;

            SendCommand();
        }

        private void SendCommand()
        {
#if DEBUG
            Debug.WriteLine("Выполнение команды: " + CurrentCommand.CommandName);
            Debug.WriteLine(BitConverter.ToString(CurrentRequest));
#endif
            CurrentErrorCode = ErrorCode.None;

            // если необходимо сделать задержку перед посылкой пакета
            if (CurrentCommand.TimeOutBeforeSend > 0)
            {
                Thread.Sleep(CurrentCommand.TimeOutBeforeSend * 1000);
            }

            // обнуляем объект состояния приема перед отправкой команды
            State.BytesReaded = 0;
            State.IsFirstIteration = true;
            State.TotalBytes = 0;

            _isLostConnectionRaised = false;


            try
            {
                if (TransportType == TransportTypes.Direct)
                {
                    StartWaitTimer();
                    State.ClearComPortBuffer();
                    //_port.DiscardInBuffer();
                    //_port.DiscardOutBuffer();

                    if (!CurrentCommand.IsReadWithoutWrite)
                    {
                        _port.Write(CurrentRequest, 0, CurrentRequest.Count());
                        /*foreach (var b in CurrentRequest)
                        {
                            _port.Write(new byte[] { b }, 0, 1);
                        };*/
                    }
                    
                    if (CurrentCommand.IsWriteWithoutRead)
                    {
                        StopWaitTimer();
                        RaiseReceiveDataCompleteEvent();
                        AutoEvent.Set();
                    }
                }
                else if (TransportType == TransportTypes.TCP)
                {
                    State.ClearTcpBuffer();
                    State.Stream = _stream;

                    TotalBytesWrite += CurrentRequest.Length;

                    if (!CurrentCommand.IsReadWithoutWrite)
                    {
                        _stream.Write(CurrentRequest, 0, CurrentRequest.Length);
                        Thread.Sleep(1000);
                    }

                    if (!CurrentCommand.IsWriteWithoutRead)
                    {
                        ReadData();

                        if(State.BytesReaded < State.TotalBytes)
                        {
                            RaiseLostConnection(false);
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                _stream.Close();
                _tcpClient.Close();
                RaiseLostConnection(true);
            }
        }

        /// <summary>
        /// Первая итерация приема данных от устройства
        /// (должно быть переопределено для каждого устройства согласно его протоколу)
        /// </summary>
        /// <param name="buffer">Входной поток принятных байтов</param>
        protected virtual void FirstIteration(byte[] buffer)
        {
            if (buffer.Length ==0)
            {
                State.TotalBytes = 0;
                State.IsFirstIteration = false;
                return;
            }
            if (CurrentErrorCode == ErrorCode.None)
            {
                if (CurrentCommand.ResponseLengthType == LengthType.Calculated)
                {
                    // вычисляем общую длину пакета (5 = 3 байта вначале пакета + 2 байта контрольной суммы в конце)
                    State.TotalBytes = 5 + buffer[2];
                }
                else if (CurrentCommand.ResponseLengthType == LengthType.Fixed)
                {
                    State.TotalBytes = CurrentCommand.ResponseLength;
                }
            }
            else
            {
                State.TotalBytes = 6;
            }

            State.IsFirstIteration = false;
        }

        private void CoreFirstIteration(byte[] buffer)
        {
            State.TotalBytes = CurrentCommand.ResponseLengthType == LengthType.Fixed ? CurrentCommand.ResponseLength : 1;
            State.IsFirstIteration = false;
        }

        private void _responseWaitTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ResponseWaitTimer.Stop();
            RaiseLostConnection(false);
        }

        /// <summary>
        /// Останавливает ожидающий приема таймер и отсоединяет от него событие
        /// </summary>
        public void WaitTimerClear()
        {
            if (ResponseWaitTimer != null)
            {
                ResponseWaitTimer.Stop();
                ResponseWaitTimer.Elapsed -= _responseWaitTimer_Elapsed;
                ResponseWaitTimer.Dispose();
                ResponseWaitTimer = null;
            }
        }

        /// <summary>
        /// Останавливает таймер, ожидающий получения данных,
        /// и устанавливает флаг, что данные были получены
        /// </summary>
        private void StopWaitTimer()
        {
            if (ResponseWaitTimer != null)
            {
                ResponseWaitTimer.Stop();
            }
        }

        /// <summary>
        /// Запускает таймер, ожидающий получения данных
        /// </summary>
        private void StartWaitTimer()
        {
            if (ResponseWaitTimer != null)
            {
                ResponseWaitTimer.Start();
            }
        }

        /// <summary>
        /// Проверяет контрольную сумму ответа прибора
        /// (должен быть реализован индивидуально каждым транспортом, если протокол прибора реализует проверку контрольной суммы)
        /// </summary>
        /// <returns>true - контрольная сумма верна; false - контрольная сумма неверна</returns>
        protected virtual bool VerifyCheckSum()
        {
            try
            {
                return Buffer.VerifyModbusCheckSum(State.TotalBytes);
            }
            catch
            {
                return false;
            }
        }

        private int _networkAddress;
        public int NetworkAddress
        {
            set
            {
                _networkAddress = value;
            }
        }
        /// <summary>
        /// Проверяет сетевой адрес
        /// (должен быть реализован каждым прибором, если его протокол зависит от сетевого адреса)
        /// </summary>
        /// <returns></returns>
        protected virtual bool CheckNetworkAddress()
        {
            try
            {
                return Buffer[0] == (byte)_networkAddress;
            }
            catch
            {
                return false;
            }
        }
    }
}
