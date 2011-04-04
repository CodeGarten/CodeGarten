using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SVN
{
    public enum SVNPrivileges
    {
        r,
        rw
    }

    public sealed class SVNAuthorization : IDisposable
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
            AddUserToRepo(_repositories[repoName], "*", privileges);
        }

        public void AddUserPermissionsToRepository(String repoName, String userName, SVNPrivileges privileges)
        {
            if(!_repositories.ContainsKey(repoName))
                _repositories.Add(repoName, CreateRepository(repoName));
            AddUserToRepo(_repositories[repoName], userName, privileges);
        }

        public void AddGroupPermissionsToRepository(String repoName, String groupName, SVNPrivileges privileges)
        {
            if (!_repositories.ContainsKey(repoName))
                _repositories.Add(repoName, CreateRepository(repoName));
            AddGroupToRepo(_repositories[repoName], groupName, privileges);
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

        private static void AddUserToRepo(StringBuilder repo, String userName, SVNPrivileges privilege)
        {
            repo.AppendFormat("{0} = {1}\n", userName, privilege.ToString());
        }

        private static void AddGroupToRepo(StringBuilder repo, String groupName, SVNPrivileges privilege)
        {
            repo.AppendFormat("@{0} = {1}\n", groupName, privilege.ToString());
        }
        
    }
}
