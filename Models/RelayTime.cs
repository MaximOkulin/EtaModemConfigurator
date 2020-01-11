using EtaModemConfigurator.ViewModels;
using System;

namespace EtaModemConfigurator.Models
{
    public class RelayTime : ViewModelBase, ICloneable
    {
        public ushort DayNumber { get; set; }
        public DateTime Date { get; set; }

        private byte _hourOn;
        private byte? _originalHourOn;

        public byte HourOn {
            get => _hourOn;
            set
            {
                if (value < 0 || value > 23) return;
                _hourOn = value;

                if (_originalHourOn == null)
                {
                    _originalHourOn = value;
                }
                CheckChanges();

                OnPropertyChanged("HourOn");
            }
        }


        private byte _minuteOn;
        private byte? _originalMinuteOn;

        public byte MinuteOn
        {
            get => _minuteOn;
            set
            {
                if (value < 0 || value > 59) return;
                _minuteOn = value;

                if (_originalMinuteOn == null)
                {
                    _originalMinuteOn = value;
                }
                CheckChanges();

                OnPropertyChanged("MinuteOn");
            }
        }

        private byte _hourOff;
        private byte? _originalHourOff;

        public byte HourOff
        {
            get => _hourOff;
            set
            {
                if (value < 0 || value > 23) return;
                _hourOff = value;

                if (_originalHourOff == null)
                {
                    _originalHourOff = value;
                }
                CheckChanges();

                OnPropertyChanged("HourOff");
            }
        }

        private byte _minuteOff;
        private byte? _originalMinuteOff;

        public byte MinuteOff
        {
            get => _minuteOff;
            set
            {
                if (value < 0 || value > 59) return;
                _minuteOff = value;

                if (_originalMinuteOff == null)
                {
                    _originalMinuteOff = value;
                }
                CheckChanges();

                OnPropertyChanged("MinuteOff");
            }
        }


        public int RelayNumber { get; set; }

        private bool _isEdited = false;
        public bool IsEdited {
            get => _isEdited;
            set
            {
                bool isNeedToRaiseRelayTimeChanged = false;
                if (_isEdited != value)
                {
                    isNeedToRaiseRelayTimeChanged = true;
                }

                _isEdited = value;
                OnPropertyChanged("IsEdited");

                if (isNeedToRaiseRelayTimeChanged)
                {
                    RaiseRelayTimeChanged();
                }
            }
        }

        public void SetSuccessfullEdit()
        {
            _originalHourOn = _hourOn;
            _originalMinuteOn = _minuteOn;
            _originalHourOff = _hourOff;
            _originalMinuteOff = _minuteOff;
            IsEdited = false;
        }

        private void CheckChanges()
        {
            if (_originalHourOn != null && _originalMinuteOn != null && _originalHourOff != null && _originalMinuteOff != null)
            {
                IsEdited = !(_originalHourOn.Value == _hourOn && _originalMinuteOn.Value == _minuteOn &&
                           _originalHourOff.Value == _hourOff && _originalMinuteOff.Value == _minuteOff);
            }
            else
            {
                IsEdited = false;
            }
        }

        public delegate void RelayTimeChangedEventHandler(object sender, System.EventArgs e);

        // событие "Приём данных завершен"
        public event RelayTimeChangedEventHandler RelayTimeChanged;

        private void RaiseRelayTimeChanged()
        {
            RelayTimeChanged?.Invoke(this, null);
        }

        public object Clone()
        {
            return new RelayTime
            {
                DayNumber = DayNumber,
                Date = Date,
                HourOn = HourOn,
                MinuteOn = MinuteOn,
                HourOff = HourOff,
                MinuteOff = MinuteOff,
                RelayNumber = RelayNumber
            };
        }
    }
}
