namespace Saritasa.Tools.Configuration
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.IO;
    using System.Linq;

    public class AdvancedSettingsManager
    {
        #region fields

        public static string defaultConfigurationFileName = "Web.config";

        /// <summary>
        /// default path to the config file that contains the settings we are using
        /// </summary>
        private static string configurationFile;

        /// <summary>
        /// Stores an instance of this class, to cut down on I/O: No need to keep re-loading that config file
        /// </summary>
        /// <remarks>Cannot use system.web.caching since agents will not have access to this by default, so use static member instead.</remarks>
        private static AdvancedSettingsManager instance;

        /// <summary>
        /// Fallback configuration.
        /// </summary>
        private static Configuration config;

        /// <summary>
        /// Settings Environment
        /// </summary>
        private static string settingsEnvironment;

        private static EnvironmentSectionGroup currentSettingsGroup;

        #endregion

        #region Constructors

        private AdvancedSettingsManager()
        {
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();

            fileMap.ExeConfigFilename = configurationFile;

            config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            settingsEnvironment = "Localhost"; // default to localhost

            // get the name of the machine we are currently running on
            string machineName = Environment.MachineName.ToUpper();

            // compare to known environment machine names
            if (config.AppSettings.Settings["ProductionNames"] != null && !string.IsNullOrWhiteSpace(config.AppSettings.Settings["ProductionNames"].Value) &&
                config.AppSettings.Settings["ProductionNames"].Value.Split(',').Where(x => machineName.Contains(x)).Count() > 0)
            {
                settingsEnvironment = "Production";
            }
            else if (config.AppSettings.Settings["QANames"] != null && !string.IsNullOrWhiteSpace(config.AppSettings.Settings["QANames"].Value)
                && config.AppSettings.Settings["QANames"].Value.Split(',').Where(x => machineName.Contains(x)).Count() > 0)
            {
                settingsEnvironment = "Qa";
            }
            else if (config.AppSettings.Settings["DevelopmentNames"] != null && !string.IsNullOrWhiteSpace(config.AppSettings.Settings["DevelopmentNames"].Value)
                && config.AppSettings.Settings["DevelopmentNames"].Value.Split(',').Where(x => machineName.Contains(x)).Count() > 0)
            {
                settingsEnvironment = "Dev";
            }

            // If there is a value in the EnvironmentOverride appsetting, ignore results of auto detection and set it here
            // This allows us to hit production data from localhost without monkeying with all the config settings.
            if (config.AppSettings.Settings["EnvironmentOverride"] != null &&
                !string.IsNullOrWhiteSpace(config.AppSettings.Settings["EnvironmentOverride"].Value))
            {
                settingsEnvironment = config.AppSettings.Settings["EnvironmentOverride"].Value;
            }

            // Get the name of the section we are using in this environment & load the appropriate section of the config file
            currentSettingsGroup = config.GetSectionGroup(SettingsEnvironment) as EnvironmentSectionGroup;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns the name of the current environment
        /// </summary>
        public string SettingsEnvironment
        {
            get
            {
                return settingsEnvironment;
            }
        }

        /// <summary>
        /// Returns the ConnectionStrings section
        /// </summary>
        public ConnectionStringSettingsCollection ConnectionStrings
        {
            get
            {
                if (currentSettingsGroup == null)
                {
                    return config.ConnectionStrings.ConnectionStrings;
                }

                return currentSettingsGroup.ConnectionStrings.ConnectionStrings;
            }
        }

        /// <summary>
        /// Returns the AppSettings Section
        /// </summary>
        public NameValueCollection AppSettings
        {
            get
            {
                NameValueCollection settings = new NameValueCollection();

                if (currentSettingsGroup == null)
                {
                    foreach (KeyValueConfigurationElement element in config.AppSettings.Settings)
                    {
                        settings.Add(element.Key, element.Value);
                    }

                    return settings;
                }

                foreach (KeyValueConfigurationElement element in currentSettingsGroup.AppSettings.Settings)
                {
                    settings.Add(element.Key, element.Value);
                }

                return settings;
            }
        }

        #endregion

        #region static factory methods

        /// <summary>
        /// Public factory method
        /// </summary>
        /// <returns></returns>
        public static AdvancedSettingsManager SettingsFactory()
        {
            // If there is a bin folder, such as in web projects look for the config file there first
            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\bin"))
            {
                configurationFile = string.Format(@"{0}\bin\{1}", AppDomain.CurrentDomain.BaseDirectory, defaultConfigurationFileName);
            }
            else
            {
                // agents, for example, won't have a bin folder in production
                configurationFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, defaultConfigurationFileName);
            }

            // If we still cannot find it, quit now!
            if (!File.Exists(configurationFile))
            {
                throw new FileNotFoundException(configurationFile);
            }

            return CreateSettingsFactoryInternal();
        }

        /// <summary>
        /// Overload that allows you to pass in the full path and filename of the config file you want to use.
        /// </summary>
        /// <param name="fullPathToConfigFile"></param>
        /// <returns></returns>
        public static AdvancedSettingsManager SettingsFactory(string fullPathToConfigFile)
        {
            configurationFile = fullPathToConfigFile;
            return CreateSettingsFactoryInternal();
        }

        /// <summary>internal Factory Method
        /// </summary>
        /// <returns>ConfigurationSettings object
        /// </returns>
        internal static AdvancedSettingsManager CreateSettingsFactoryInternal()
        {
            // If we havent created an instance yet, do so now
            if (instance == null)
            {
                instance = new AdvancedSettingsManager();
            }

            return instance;
        }

        #endregion
    }
}
