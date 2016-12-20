namespace Saritasa.Tools.Configuration
{
    using System.Collections.Specialized;
    using System.Configuration;

    public static class ConfigurationManager
    {
        public static NameValueCollection AppSettings
        {
            get
            {
                return AdvancedSettingsManager.SettingsFactory().AppSettings;
            }
        }

        public static ConnectionStringSettingsCollection ConnectionStrings
        {
            get
            {
                return AdvancedSettingsManager.SettingsFactory().ConnectionStrings;
            }
        }
    }
}
