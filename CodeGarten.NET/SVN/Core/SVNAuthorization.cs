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
    
    public sealed class SVNAuthorization : IDisposable
    {
        public sealed class SvnInstance
        {
            private StringBuilder _instance;
            public SvnInstance(string instanceName)
            {
                _instance = CreateRepository(instanceName);
            }

            public void AddGroupPermission(string groupName, string privilege)
            {
                AddGroupToRepo(_instance, groupName, privilege);
            }

            public void AddUserPermission(string userName, string privilege)
            {
                AddUserToRepo(_instance, userName, privilege);
            }

            public string ToString()
            {
                return _instance.ToString();
            }
        }

        private readonly FileStream _file;
        private readonly ICollection<SvnInstance> _instances;
        private readonly StringBuilder _groups;
        private readonly string _path, _fileName, _fileGroup, _fileInstance;
        private const string TmpFormat = "~{0}.tmp";

        public SVNAuthorization(string path, string fileName)
        {
            _path = path;
            _fileName = fileName;
            _fileGroup = Path.Combine(_path, String.Format(TmpFormat, "groups"));
            _fileInstance = Path.Combine(_path, String.Format(TmpFormat, "instances"));
            _instances = new LinkedList<SvnInstance>();
            _groups = new StringBuilder("[groups]\n");
        }

        public SvnInstance CreateInstance(string instanceName)
        {
            var instance = new SvnInstance(instanceName);
            _instances.Add(instance);
            return instance;
        }

        public void CreateGroup(string groupName, IEnumerable<string> users)
        {
            _groups.AppendFormat("{0} =", groupName);

            foreach (var user in users)
                _groups.AppendFormat(" {0},", user);

            _groups[_groups.Length - 1] = '\n';
        }

        public void Save()
        {
            if(_groups.Length != 0)
            {
                var fileGroups = File.CreateText(_fileGroup);
                fileGroups.Write(_groups.ToString());
                fileGroups.Close();
            }

            if(_instances.Count != 0)
            {
                var fileInstance = File.AppendText(_fileInstance);
                foreach (var svnInstance in _instances)
                    fileInstance.WriteLine(svnInstance.ToString());
                fileInstance.Close();
            }
            
            MergeFiles();
        }

        public void Dispose()
        {
            Save();            
        }

        #region Svn_authorization_file

        private void MergeFiles()
        {
            var tmpFile = Path.Combine(_path, String.Format(TmpFormat, _fileName));
            if(File.Exists(_fileGroup))
                File.Copy(_fileGroup, tmpFile);

            File.AppendAllText(tmpFile, File.ReadAllText(_fileInstance));

            OverrideFile(tmpFile, Path.Combine(_path, _fileName));
        }

        private static void OverrideFile(string sourceFileName, string destFileName)
        {
            if (File.Exists(destFileName))
            {
                do
                {
                    try
                    {
                        File.Delete(destFileName);
                        break;
                    }
                    catch (IOException e) // IF the file is in use
                    {}
                } while (true);
            }

            File.Move(sourceFileName, destFileName);
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

        #endregion
    }
}