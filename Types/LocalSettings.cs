using System;
using Newtonsoft.Json;
using System.IO;

namespace EtaModemConfigurator.Types
{
    public class LocalSettings
    {
        public int NetworkAddress { get; set; }
        public string LocalComPortName { get; set; }

        public string LocalListenAddress { get; set; }
        public int LocalListenPort { get; set; }
        public string Identifier { get; set; }
        public string ClientAddress { get; set; }
        public int ClientPort { get; set; }
        

        public LocalSettings()
        {
            
        }

        public void Init()
        {
#if DEBUG
            string settingFileName = @"..\..\..\Settings\Settings.json";
#elif (RELEASE)
            string settingFileName = @"Settings.json";
#endif
            if (File.Exists(settingFileName))
            {
                var localSettings = JsonConvert.DeserializeObject<LocalSettings>(File.ReadAllText(settingFileName));
                LocalComPortName = localSettings.LocalComPortName;
                NetworkAddress = localSettings.NetworkAddress;
                LocalListenAddress = localSettings.LocalListenAddress;
                LocalListenPort = localSettings.LocalListenPort;
                Identifier = localSettings.Identifier;
                ClientAddress = localSettings.ClientAddress;
                ClientPort = localSettings.ClientPort;
            }
            else
            {
                NetworkAddress = 254;
                LocalListenPort = 7789;
                ClientPort = 5010;
                ClientAddress = "192.168.0.1";
                SaveSettings();
            }
        }

        public void SaveSettings()
        {
            var serializedSettings = JsonConvert.SerializeObject(this);

#if DEBUG
            //string settingFileName = @"..\..\..\Settings\Settings.json";
            string settingFileName = @"Settings.json";
#elif (RELEASE)
            string settingFileName = @"Settings.json";
#endif

           // if (File.Exists(settingFileName))
            {
                File.Delete(settingFileName);
            }

            File.AppendAllText(settingFileName, serializedSettings);
        }
    }
}
