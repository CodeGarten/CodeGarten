using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Git.Core
{
    public enum Privileges
    {
        r,
        rw
    }

    internal sealed class Authorization
    {
        private readonly string _filePath;

        private XElement _base;

        public Authorization(string filePath)
        {
            _filePath = filePath;
            Load();
        }

        #region Repository Actions

        public bool CreateRepository(string name)
        {
            if (GetRepository(name) != null)
                return false;

            var repository = new XElement("repo");
            repository.SetAttributeValue("location", name);

            _base.Add(repository);

            Save();
            return true;
        }

        public bool DeleteRepository(string name)
        {
            var repository = GetRepository(name);
            if (repository == null)
                return false;

            repository.Remove();

            Save();
            return true;
        }

        public bool AddGroupToRepository(string repositoryName, string groupName, string privilege)
        {
            var repository = GetRepository(repositoryName);

            if (repository == null)
                return false;

            var group = GetGroup(groupName);

            if (group == null)
                return false;

            repository.Add(CreatePermissionGroup(groupName, privilege));

            Save();
            return true;
        }

        public bool RemoveGroupFromRepository(string repositoryName, string groupName, Privileges privilege)
        {
            throw new NotImplementedException();
        }

        public void DeleteRepositoryReferencedGroups(string repositoryName)
        {
            var repository = GetRepository(repositoryName);

            if (repository == null) return;
            foreach (var groupName in repository.Elements("group").Select(g => g.Value))
            {
                DeleteGroup(groupName);
            }
        }

        #endregion

        #region Group Actions

        public bool CreateGroup(string name)
        {
            if (GetGroup(name) != null)
                return false;

            var group = new XElement("group");
            group.SetAttributeValue("ID", name);

            _base.Add(group);

            Save();
            return true;
        }

        public bool DeleteGroup(string name)
        {
            var group = GetGroup(name);
            if (group == null)
                return false;

            group.Remove();

            foreach (
                var groupInstance in
                    _base.Elements("repo").SelectMany(repo => repo.Elements("group").Where(g => g.Value == name)))
            {
                groupInstance.Remove();
            }

            Save();
            return true;
        }

        public bool AddUserToGroup(string groupName, string userName)
        {
            var group = GetGroup(groupName);

            if (group == null)
                return false;

            group.Add(CreateUserElement(userName));

            Save();
            return true;
        }

        public bool RemoveUserFromGroup(string groupName, string userName)
        {
            var group = GetGroup(groupName);

            if (group == null)
                return false;

            group.Elements("user").SingleOrDefault(u => u.Value == userName).Remove();

            Save();
            return true;
        }

        #endregion

        #region IO

        private void Load()
        {
            if (File.Exists(_filePath))
                _base = XElement.Load(_filePath);
            else
            {
                _base = CreateMainStructure();
                Save();
            }
        }

        private void Save()
        {
            _base.Save(_filePath);
        }

        #endregion

        #region Helpers

        private XElement GetRepository(string name)
        {
            return _base.Elements("repo").SingleOrDefault(e => e.Attribute("location").Value == name);
        }

        private XElement GetGroup(string name)
        {
            return _base.Elements("group").SingleOrDefault(e => e.Attribute("ID").Value == name);
        }


        private static XElement CreateMainStructure()
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

        #endregion
    }
}