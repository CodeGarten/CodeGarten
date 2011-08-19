using System;
using System.Collections.Generic;
using System.IO;

namespace Trac.Core
{
    public sealed class TracEnvironmentManager
    {
        private readonly String _parentPath, _envName;

        public String EnvironmentPath { get; private set; }

        private TracEnvironmentManager(String parentPath, String envName)
        {
            _parentPath = parentPath;
            _envName = envName;
            EnvironmentPath = FormatEnvironmentPath(parentPath, envName);
        }
        
        public bool Initialize()
        {
            return TracAdmin.InitEnv(_envName, EnvironmentPath);
        }

        public bool Initialize(IEnumerable<KeyValuePair<string, string>> services)
        {
            var initFlag = TracAdmin.InitEnv(_envName, EnvironmentPath);
            
            foreach (var service in services)
                TracAdmin.ConfigPlugins(EnvironmentPath, service.Key, service.Value);

            return initFlag;
        }

        public static bool Delete(String parentPath, String envName)
        {
            var path = FormatEnvironmentPath(parentPath, envName);
            if (!Directory.Exists(path))
                return false;
            Directory.Delete(path, true);
            return true;
        }

        public static TracEnvironmentManager Create(String parentPath, String envName)
        {
            var path = FormatEnvironmentPath(parentPath, envName);
            if (Directory.Exists(path))
                return null;
            Directory.CreateDirectory(path);
            return new TracEnvironmentManager(parentPath, envName);
        }
        
        public static String FormatEnvironmentPath(String parentPath, String envName)
        {
            return String.Format(@"{0}\{1}", parentPath, envName);
        }
    }
}
