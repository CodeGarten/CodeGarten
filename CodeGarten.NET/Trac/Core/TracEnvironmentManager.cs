using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Trac
{
    public sealed class TracEnvironmentManager
    {
        private readonly String _parentPath;
        private readonly String _envName;

        private TracEnvironmentManager(String parentPath, String envName)
        {
            _parentPath = parentPath;
            _envName = envName;
        }
        
        public bool Initialize()
        {
            return TracAdmin.InitEnv(_envName, String.Format(@"{0}\{1}", _parentPath, _envName));
        }

        public static TracEnvironmentManager Create(String parentPath, String envName)
        {
            var path = String.Format(@"{0}\{1}", parentPath, envName);
            if (Directory.Exists(path))
                return null;
            Directory.CreateDirectory(path);
            return new TracEnvironmentManager(parentPath, envName);
        }
    }
}
