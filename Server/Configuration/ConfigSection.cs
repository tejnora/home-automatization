using System;
using System.Configuration;
using System.IO;

namespace Server.Configuration
{

    public abstract class ConfigSection : ConfigurationSection
    {
        protected static System.Configuration.Configuration GetDefaultConfiguration()
        {
            var loc = AppDomain.CurrentDomain.FriendlyName;
            var configName = Path.ChangeExtension(loc, "config");
            var fileMap = new ExeConfigurationFileMap { ExeConfigFilename = configName };
            return ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
        }

        protected static void AddConfiguration(string name, ConfigurationSection section)
        {
            var configuration = GetDefaultConfiguration();
            configuration.Sections.Add(name, section);
            configuration.Save();
        }
    }
}