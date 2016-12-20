namespace Saritasa.Tools.Configuration
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Configuration.Internal;
    using System.Reflection;

    using Extensions;

    public sealed class ConfigSystem : IInternalConfigSystem
    {
        private static IInternalConfigSystem clientConfigSystem;

        private object appsettings;

        private object connectionStrings;

        /// <summary>
        /// Re-initializes the ConfigurationManager, allowing us to merge in the settings from Core.Config.
        /// </summary>
        /// <param name="configurationFileName">Can be null, otherwise loading config by specified file name. Default value is "Web.config"</param>
        public static void Install(string configurationFileName = null)
        {
            if (!string.IsNullOrEmpty(configurationFileName))
            {
                AdvancedSettingsManager.defaultConfigurationFileName = configurationFileName;
            }

            FieldInfo[] fiStateValues = null;
            Type tInitState = typeof(System.Configuration.ConfigurationManager).GetNestedType("InitState", BindingFlags.NonPublic);

            if (null != tInitState)
            {
                fiStateValues = tInitState.GetFields();
            }

            FieldInfo fiInit = typeof(System.Configuration.ConfigurationManager).GetField("s_initState", BindingFlags.NonPublic | BindingFlags.Static);
            FieldInfo fiSystem = typeof(System.Configuration.ConfigurationManager).GetField("s_configSystem", BindingFlags.NonPublic | BindingFlags.Static);

            if (fiInit != null && fiSystem != null && null != fiStateValues)
            {
                fiInit.SetValue(null, fiStateValues[1].GetValue(null));
                fiSystem.SetValue(null, null);
            }

            ConfigSystem confSys = new ConfigSystem();
            Type configFactoryType = Type.GetType("System.Configuration.Internal.InternalConfigSettingsFactory, System.Configuration, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", true);
            IInternalConfigSettingsFactory configSettingsFactory = (IInternalConfigSettingsFactory)Activator.CreateInstance(configFactoryType, true);
            configSettingsFactory.SetConfigurationSystem(confSys, false);

            Type clientConfigSystemType = Type.GetType("System.Configuration.ClientConfigurationSystem, System.Configuration, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", true);
            clientConfigSystem = (IInternalConfigSystem)Activator.CreateInstance(clientConfigSystemType, true);
        }

        #region IInternalConfigSystem Members

        public object GetSection(string configKey)
        {
            // get the section from the default location (web.config or app.config)
            object section = clientConfigSystem.GetSection(configKey);

            switch (configKey)
            {
                case "appSettings":
                    if (this.appsettings != null)
                    {
                        return this.appsettings;
                    }

                    if (section is NameValueCollection)
                    {
                        // create a new collection because the underlying collection is read-only
                        var cfg = new NameValueCollection();
                        NameValueCollection localSettings = (NameValueCollection)section;
                        foreach (string key in localSettings)
                        {
                            cfg.Add(key, localSettings[key]);
                        }

                        // merge the settings from core with the local appsettings
                        this.appsettings = cfg.Merge(ConfigurationManager.AppSettings);
                        section = this.appsettings;
                    }

                    break;
                case "connectionStrings":
                    if (this.connectionStrings != null)
                    {
                        return this.connectionStrings;
                    }

                    // create a new collection because the underlying collection is read-only
                    var cssc = new ConnectionStringSettingsCollection();

                    // copy the existing connection strings into the new collection
                    foreach (ConnectionStringSettings connectionStringSetting in ((ConnectionStringsSection)section).ConnectionStrings)
                    {
                        cssc.Add(connectionStringSetting);
                    }

                    // merge the settings from core with the local connectionStrings
                    cssc = cssc.Merge(ConfigurationManager.ConnectionStrings);

                    ConnectionStringsSection connectionStringsSection = new ConnectionStringsSection();

                    foreach (ConnectionStringSettings connectionStringSetting in cssc)
                    {
                        connectionStringsSection.ConnectionStrings.Add(connectionStringSetting);
                    }

                    this.connectionStrings = connectionStringsSection;
                    section = this.connectionStrings;
                    break;
            }

            return section;
        }

        public void RefreshConfig(string sectionName)
        {
            if (sectionName == "appSettings")
            {
                this.appsettings = null;
            }

            if (sectionName == "connectionStrings")
            {
                this.connectionStrings = null;
            }

            clientConfigSystem.RefreshConfig(sectionName);
        }

        public bool SupportsUserConfig
        {
            get { return clientConfigSystem.SupportsUserConfig; }
        }

        #endregion
    }
}
