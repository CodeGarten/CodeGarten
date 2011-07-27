using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trac
{
    public static class PermissionUser
    {
        public const String Anonymous = "anonymous";
        public const String Authenticated = "authenticated";
    }

    public sealed class TracPermissionManager
    {        
        private readonly String _envPath;

        public TracPermissionManager(String envPath)
        {
            _envPath = envPath;
        }

        public bool Add(String name, TracPrivileges privileges)
        {
            return TracAdmin.Add(name, _envPath, privileges.ToString());
        }

        public bool Add(String name, params TracPrivileges[] privilegeses)
        {
            var stringBuilder = new StringBuilder(privilegeses[0].ToString());
            for  (int i=1 ; i<privilegeses.Length ; ++i)
                stringBuilder.Append(' ').Append(privilegeses[i].ToString());

            return TracAdmin.Add(name, _envPath, stringBuilder.ToString());
        }

        public bool Add(String name, params String[] privilegeses)
        {
            var stringBuilder = new StringBuilder(privilegeses[0]);
            for (int i = 1; i < privilegeses.Length; ++i)
                stringBuilder.Append(' ').Append(privilegeses[i]);

            return TracAdmin.Add(name, _envPath, stringBuilder.ToString());
        }

        public bool AddAll(String name)
        {
            return TracAdmin.Add(name, _envPath, "'*'");
        }

        public bool Remove(String name, TracPrivileges privileges)
        {
            return TracAdmin.Remove(name, _envPath, privileges.ToString());
        }

        public bool Remove(String name, params TracPrivileges[] privilegeses)
        {
            var stringBuilder = new StringBuilder(privilegeses[0].ToString());
            for (int i = 1; i < privilegeses.Length; ++i)
                stringBuilder.Append(' ').Append(privilegeses[i].ToString());

            return TracAdmin.Remove(name, _envPath, stringBuilder.ToString());
        }

        public bool Remove(String name, params String[] privilegeses)
        {
            var stringBuilder = new StringBuilder(privilegeses[0]);
            for (int i = 1; i < privilegeses.Length; ++i)
                stringBuilder.Append(' ').Append(privilegeses[i]);

            return TracAdmin.Remove(name, _envPath, stringBuilder.ToString());
        }

        public bool RemoveAll(String name)
        {
            return TracAdmin.Remove(name, _envPath, "'*'");
        }

        public bool RemoveAll()
        {
            return TracAdmin.Remove("'*'", _envPath, "'*'");
        }

        public bool AddGroupUser(String userName, String groupName)
        {
            return TracAdmin.Add(userName, _envPath, groupName);
        }

        public bool RemoveGroupUser(String userName, String groupName)
        {
            return TracAdmin.Remove(userName, _envPath, groupName);
        }

        public List<String> ListGroups(String name)
        {
            return TracAdmin.ListGroup(name, _envPath);
        }

        public List<String> ListGroups()
        {
            return TracAdmin.ListGroup(_envPath);
        }

        public List<TracPrivileges> List(String name)
        {
            return TracAdmin.List(name, _envPath);
        }

        public Dictionary<String, List<TracPrivileges>> List()
        {
            return TracAdmin.ListAll(_envPath);
        }
    }
}
