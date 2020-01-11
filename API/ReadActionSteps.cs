using EtaModemConfigurator.Base;
using System;
using System.Threading;

namespace EtaModemConfigurator.API
{
    public partial class ActionSteps : ActionStepsBase
    {
        private readonly Functions _functions;
        private readonly Commands _commands;

        public Functions Functions
        {
            get => _functions;
        }

        public ActionSteps(Transport.Transport transport, ManualResetEvent autoEvent, int deviceAddres): base(transport, autoEvent)
        {
            _functions = new Functions(deviceAddres);
            _commands = new Commands();
        }

        public void StartSession()
        {
            Transport.CurrentCommand = _commands.StartSession;
            Transport.Send(new byte[] { });
            Wait();
        }

        public void ReadIsSetting()
        {
            Transport.CurrentCommand = _commands.ReadIsSetting;
            Transport.Send(_functions.ReadIsSetting(), true);
            Wait();
        }

        public void ReadConfiguration1()
        {
            Transport.CurrentCommand = _commands.ReadConfiguration1;
            Transport.Send(_functions.ReadConfiguration1(), true);
            Wait();
        }

        public void SetControlMode()
        {
            Transport.CurrentCommand = _commands.SetControlMode;
            Transport.Send(_functions.SetControlMode(), true);
            Wait();
        }

        public void ReadIdentifierPart1()
        {
            Transport.CurrentCommand = _commands.ReadIdentifierPart1;
            Transport.SerialPortReceiveDelay = 3000;
            Transport.Send(_functions.ReadIdentifierPart1(), true);
            Wait();
        }

        public void ReadIdentifierPart2()
        {
            Transport.CurrentCommand = _commands.ReadIdentifierPart2;
            Transport.Send(_functions.ReadIdentifierPart2(), true);
            Wait();
        }

        public void ReadIdentifierPart3()
        {
            Transport.CurrentCommand = _commands.ReadIdentifierPart3;
            Transport.Send(_functions.ReadIdentifierPart3(), true);
            Wait();
        }

        public void ReadIdentifierPart4()
        {
            Transport.CurrentCommand = _commands.ReadIdentifierPart4;
            Transport.Send(_functions.ReadIdentifierPart4(), true);
            Wait();
        }

        public void ReadIdentifierPart5()
        {
            Transport.CurrentCommand = _commands.ReadIdentifierPart5;
            Transport.Send(_functions.ReadIdentifierPart5(), true);
            Wait();
        }

        public void ReadPin()
        {
            Transport.CurrentCommand = _commands.ReadPin;
            Transport.SerialPortReceiveDelay = 500;
            Transport.Send(_functions.ReadPin(), true);
            Wait();
        }

        public void ReadApnPart1()
        {
            Transport.CurrentCommand = _commands.ReadApnPart1;
            Transport.Send(_functions.ReadApnPart1(), true);
            Wait();
        }

        public void ReadApnPart2()
        {
            Transport.CurrentCommand = _commands.ReadApnPart2;
            Transport.Send(_functions.ReadApnPart2(), true);
            Wait();
        }

        public void ReadLogin()
        {
            Transport.CurrentCommand = _commands.ReadLogin;
            Transport.Send(_functions.ReadLogin(), true);
            Wait();
        }

        public void ReadPassword()
        {
            Transport.CurrentCommand = _commands.ReadPassword;
            Transport.Send(_functions.ReadPassword(), true);
            Wait();
        }

        public void ReadListenPortServCount()
        {
            Transport.CurrentCommand = _commands.ReadListenPortServCount;
            Transport.Send(_functions.ReadListenPortServCount(), true);
            Wait();
        }

        public void ReadAddr_1()
        {
            Transport.CurrentCommand = _commands.ReadAddr_1;
            Transport.Send(_functions.ReadAddr_1(), true);
            Wait();
        }

        public void ReadPort_1()
        {
            Transport.CurrentCommand = _commands.ReadPort_1;
            Transport.Send(_functions.ReadPort_1(), true);
            Wait();
        }

        public void ReadAddr_2()
        {
            Transport.CurrentCommand = _commands.ReadAddr_2;
            Transport.Send(_functions.ReadAddr_2(), true);
            Wait();
        }

        public void ReadPort_2()
        {
            Transport.CurrentCommand = _commands.ReadPort_2;
            Transport.Send(_functions.ReadPort_2(), true);
            Wait();
        }

        public void ReadAddr_3()
        {
            Transport.CurrentCommand = _commands.ReadAddr_3;
            Transport.Send(_functions.ReadAddr_3(), true);
            Wait();
        }

        public void ReadPort_3()
        {
            Transport.CurrentCommand = _commands.ReadPort_3;
            Transport.Send(_functions.ReadPort_3(), true);
            Wait();
        }

        public void ReadCommonSettings()
        {
            Transport.CurrentCommand = _commands.ReadCommonSettings;
            Transport.Send(_functions.ReadCommonSettings(), true);
            Wait();
        }

        public void ReadCheckRebootSettings()
        {
            Transport.CurrentCommand = _commands.ReadCheckRebootSettings;
            Transport.Send(_functions.ReadCheckRebootSettings(), true);
            Wait();
        }

        public void ReadSignalQuality()
        {
            Transport.CurrentCommand = _commands.ReadSignalQuality;
            Transport.Send(_functions.ReadSignalQuality(), true);
            Wait();
        }

        public void ReadSoftwareVersion()
        {
            Transport.CurrentCommand = _commands.ReadSoftware;
            Transport.Send(_functions.ReadSoftwareVersion(), true);
            Wait();
        }

        public void ReadCalendar(DateTime startDate, DateTime endDate, int startDayNumber)
        {
            Transport.CurrentCommand = _commands.ReadCalendar;
            Transport.Send(_functions.ReadCalendar(startDate, endDate, startDayNumber), true);
            Wait();
        }
    }
}
