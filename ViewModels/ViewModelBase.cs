using System.ComponentModel;

namespace EtaModemConfigurator.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void CheckProperty(string propertyName)
        {

        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            CheckProperty(propertyName);
            PropertyChangedEventHandler handler = this.PropertyChanged;

            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
    }
}
