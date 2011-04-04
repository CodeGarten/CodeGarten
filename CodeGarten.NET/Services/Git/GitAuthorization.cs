using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;

namespace Git
{
    static class GitAuthorization
    {
        private static readonly string FilePath = ConfigurationManager.AppSettings["authorizationFile"];

        public static XElement CreateRepository(string name, Dictionary<string,string> permissions)
        {
            var elem = new XElement("repo", permissions == null?null:permissions.Select(pair => CreatePermissionGroup(pair.Key, pair.Value)));
            elem.SetAttributeValue("location", name);

            return elem;
        }

        public static void AddRepositoryGroup(XElement repository, string group, string permission)
        {
            repository.Add(CreatePermissionGroup(group,permission));
        }

        public static void AddRepositoryUser(XElement repository, string user, string permission)
        {
            repository.Add(CreatePermissionUser(user, permission));
        }








        public static XElement CreateGroup(string name, params string[] users)
        {
            var elem = new XElement("group", users.Select(CreateUserElement));
            elem.SetAttributeValue("ID", name);

            return elem;
        }

        public static void AddGroupUser(XElement group, string user)
        {
            group.Add(CreateUserElement(user));
        }









        public static void Save(XElement element)
        {
            element.Save(FilePath);
        }

        public static XElement CreateMainStructure()
        {
            return new XElement("autho_file");
        }






        private static XElement CreatePermissionGroup(string group, string permission)
        {
            return CreatePermissionElement("group", permission, group);
        }

        private static XElement CreatePermissionUser(string user, string permission)
        {
            return CreatePermissionElement("user", permission, user);
        }

        private static XElement CreatePermissionElement(string type, string permission, string entity)
        {
            var elem = new XElement(type);
            elem.SetAttributeValue("perm", permission);
            elem.SetValue(entity);
            return elem;
        }

        private static XElement CreateUserElement(string name)
        {
            var elem = new XElement("user");
            elem.SetValue(name);
            return elem;
        }
    }
}
