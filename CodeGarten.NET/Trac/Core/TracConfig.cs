using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using CodeGarten.Service;

namespace Trac.Core
{
    public class TracConfiguration : ConfigurationSection
    {

        public static TracConfiguration Settings { get; private set; }

        static TracConfiguration()
        {
            var assemblyConfig =
                ConfigurationManager.OpenExeConfiguration(Path.Combine(ServiceConfig.ServicesDllLocation, "Trac.dll"));
            Settings = (TracConfiguration) assemblyConfig.GetSection("tracConfig");
        }

        [ConfigurationProperty("tracAdmin", IsRequired = true)]
        public string TracAdmin
        {
            get { return (string) this["tracAdmin"]; }
            set { this["tracAdmin"] = value; }
        }

        //[ConfigurationProperty("plugins", IsRequired = false, IsDefaultCollection = false)]
        [ConfigurationProperty("plugins", IsRequired = false)]
        [ConfigurationCollection(typeof(PluginsConfigCollection), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public PluginsConfigCollection Plugins
        {
            get { return (PluginsConfigCollection)this["plugins"]; }
        }
    }

    public class PluginsConfigCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new PluginConfig();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PluginConfig) element).Service;
        }
    }

    public class PluginConfig : ConfigurationElement
    {
        [ConfigurationProperty("service", IsRequired = true, IsKey = true)]

        public string Service
        {
            get { return (string) this["service"]; }
            set { this["service"] = value; }
        }

        [ConfigurationProperty("cfgFile", IsRequired = true)]
        public string ConfigFile
        {
            get { return (string)this["cfgFile"]; }
            set { this["cfgFile"] = value; }
        }
    }
}
