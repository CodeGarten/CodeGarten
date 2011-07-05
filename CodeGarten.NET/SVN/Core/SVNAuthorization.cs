using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using CodeGarten.Data.Interfaces;

namespace SVN
{
    public enum SVNPrivileges
    {
        r,
        rw
    }

    public sealed class SVNAuthorization : IAuthorization, IDisposable
    {
        private readonly StringBuilder _groups;
        private readonly Dictionary<String, StringBuilder> _repositories;
        private readonly String _filePath;

        public SVNAuthorization(String pathFile)
        {
            _repositories = new Dictionary<string, StringBuilder>();
            _groups = new StringBuilder("[groups]\n");
            _filePath = pathFile;
        }

        public void AddAllPermissionsToRepository(String repoName, SVNPrivileges privileges)
        {
            if (!_repositories.ContainsKey(repoName))
                _repositories.Add(repoName, CreateRepository(repoName));
            AddUserToRepo(_repositories[repoName], "*", privileges.ToString());
        }

        public void AddUserPermissionsToRepository(String repoName, String userName, SVNPrivileges privileges)
        {
            if (!_repositories.ContainsKey(repoName))
                _repositories.Add(repoName, CreateRepository(repoName));
            AddUserToRepo(_repositories[repoName], userName, privileges.ToString());
        }

        public void AddGroupPermissionsToRepository(String repoName, String groupName, SVNPrivileges privileges)
        {
            if (!_repositories.ContainsKey(repoName))
                _repositories.Add(repoName, CreateRepository(repoName));
            AddGroupToRepo(_repositories[repoName], groupName, privileges.ToString());
        }

        public void CreateGroup(String groupName, IEnumerable<String> usersGroup)
        {
            _groups.AppendFormat("{0} =", groupName);

            foreach (var user in usersGroup)
                _groups.AppendFormat(" {0},", user);

            _groups[_groups.Length - 1] = '\n';
        }

        public bool Save()
        {
            using (TextWriter tw = new StreamWriter(_filePath))
            {
                tw.Write(_groups.ToString());

                foreach (var repository in _repositories)
                    tw.Write(repository.Value.ToString());

                return true;
            }
        }

        public void Dispose()
        {
            Save();
        }

        private static StringBuilder CreateRepository(String repoName)
        {
            return new StringBuilder(String.Format("[{0}:/]\n", repoName));
        }

        private static void AddUserToRepo(StringBuilder repo, String userName, string privilege)
        {
            repo.AppendFormat("{0} = {1}\n", userName, privilege);
        }

        private static void AddGroupToRepo(StringBuilder repo, String groupName, string privilege)
        {
            repo.AppendFormat("@{0} = {1}\n", groupName, privilege);
        }

        #region IAuthorization Members

        public string GetGroupName(long structure, long container, string roleType)
        {
            return String.Format("{0}{1}{2}", structure, container, roleType);
        }

        public string GetContainerName(long structure, long container, string workSpace)
        {
            return String.Format("{0}{1}{2}", structure, container, workSpace);
        }

        public void AddGroup(string container, IEnumerable<string> users)
        {
            CreateGroup(container, users);
        }

        //TODO make this better
        public void CreateContainer(string container)
        {
            if (_repositories.ContainsKey(container))
                throw new Exception("repositorio ja existe");

            _repositories.Add(container, CreateRepository(container));
        }

        public void AddGroupPermission(string container, string group, IEnumerable<string> permissions)
        {
            foreach (var permission in permissions)
                AddGroupToRepo(_repositories[container], group, permission);
        }

        public void AddUserPermission(string container, string user, IEnumerable<string> permissions)
        {
            foreach (var permission in permissions)
                AddUserToRepo(_repositories[container], user, permission);
        }

        #endregion
    }
}