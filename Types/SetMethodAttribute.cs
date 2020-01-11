using System;

namespace EtaModemConfigurator.Types
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SetMethodAttribute : Attribute
    {
        private readonly string _description;

        public SetMethodAttribute(string description)
        {
            _description = description;
        }

        public string GetDescription()
        {
            return _description;
        }
    }
}
