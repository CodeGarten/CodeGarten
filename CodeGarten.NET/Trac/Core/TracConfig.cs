using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using CodeGarten.Service;

namespace Trac.Core
{
    public static class TracConfig
    {
        public static TracConfiguration Settings { get; private set; }

        static TracConfig()
        {
            Settings = new TracConfiguration(Path.Combine(ServiceConfig.ServicesDllLocation, "Trac.dll.config"));
            Settings.Load();
        }
    }

    public class TracConfiguration
    {
        private readonly string _xmlPath;

        public TracConfiguration(string xmlPath)
        {
            _xmlPath = xmlPath;
        }

        public void Load()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(_xmlPath);

            var tracAdminNode = xmlDocument.SelectSingleNode("/configuration/tracConfig");
            TracAdmin = tracAdminNode.Attributes["tracAdmin"].Value;
            var node = tracAdminNode.Attributes["inherit"];
            InheritInit = node == null ? null : node.Value;

            var pluginNodes = xmlDocument.SelectNodes("/configuration/tracConfig/plugins/add[@service]");
            Plugins = new Dictionary<string, PluginConfig>();
            foreach (XmlNode pluginNode in pluginNodes)
            {
                var service = pluginNode.Attributes["service"].Value;
                Plugins.Add(service, new PluginConfig(service,
                                             pluginNode.Attributes["cfgFile"].Value));
            }
        }

        public string TracAdmin { get;private set;}
        public string InheritInit { get; private set; }

        public Dictionary<string, PluginConfig> Plugins { get; private set; }

    }

    public class PluginConfig
    {
        public PluginConfig(string service, string configFile)
        {
            Service = service;
            ConfigFile = configFile;
        }

        public string Service { get; private set; }

        public string ConfigFile { get; private set; }
    }
}
