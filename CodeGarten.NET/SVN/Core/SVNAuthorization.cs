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
    

    //TODO thread-safe
    public sealed class SVNAuthorization : IDisposable
    {
        public sealed class SvnInstance
        {
            private readonly StringBuilder _instance;

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

            public override string ToString()
            {
                return _instance.ToString();
            }

        }

        public sealed class SvnGroup
        {
            private readonly string _group;
            private readonly ICollection<string> _users;

            public SvnGroup(string groupName)
            {
                _users = new LinkedList<string>();
                _group = String.Format("{0} =", groupName);
            }

            public void AddUser(IEnumerable<string> users)
            {
                foreach (var user in users)
                    _users.Add(user);
            }

            public void AddUser(string userName)
            {
                _users.Add(userName);
            }

            public void RemoveUser(string userName)
            {
                _users.Remove(userName);
            }

            public override string ToString()
            {
                var returnString = new StringBuilder(_group);

                foreach (var user in _users)
                    returnString.AppendFormat(" {0},", user);

                returnString[returnString.Length - 1] = '\n';

                return returnString.ToString();
            }
        }

        private readonly WeakReference _groups, _instances;
        private readonly string _path, _fileName, _fileGroup, _fileInstance, _fileOverride, _file;
        private const string TmpFormat = "~{0}.tmp";

        public SVNAuthorization(string path, string fileName)
        {
            _path = path;
            _fileName = fileName;
            _fileGroup = Path.Combine(_path, String.Format(TmpFormat, "groups"));
            _fileInstance = Path.Combine(_path, String.Format(TmpFormat, "instances"));
            _fileOverride = Path.Combine(_path, String.Format(TmpFormat, _fileName));
            _file = Path.Combine(_path, fileName);
            
            _instances = new WeakReference(null);
            _groups = new WeakReference(null);
            
        }

        private Dictionary<string, SvnInstance> FileInstances()
        {
            using(TextReader textReader = new StreamReader(_fileInstance))
            {
                string line;
                var instances = new Dictionary<string, SvnInstance>();

                while((line = textReader.ReadLine())!=null)
                {
                    if(!line.StartsWith("["))
                        continue;

                    var instanceName = line.Substring(1, line.IndexOf(':'));
                    var instance = new SvnInstance(instanceName);

                    while ((line = textReader.ReadLine()) != null && line != "")
                    {
                        var itens = line.Split(new[] {'@', ' ', '='}, StringSplitOptions.RemoveEmptyEntries).ToList();
                        var groupName = itens[0];
                        itens.RemoveAt(0);
                        instance.AddGroupPermission(groupName, itens[1]);
                        
                    }

                    instances.Add(instanceName, instance);
                }

                return instances;
            }
        }

        private Dictionary<string, SvnGroup> FileGroups()
        {
            using(TextReader textReader = new StreamReader(_fileGroup))
            {
                string line;
                var groups = new Dictionary<string, SvnGroup>();
                while((line = textReader.ReadLine())!=null)
                {
                    if(line=="")
                        continue;

                    var itens = line.Split(new[] { '=', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    var groupName = itens[0];
                    itens.RemoveAt(0);

                    var group = new SvnGroup(groupName);
                    group.AddUser(itens);
                    groups.Add(groupName, group);
                }

                return groups;
            }            
        }

        public SvnInstance CreateOrGetInstance(string instanceName)
        {
            Dictionary<string, SvnInstance> instances;
            if(!_instances.IsAlive)
                 _instances.Target = instances = FileInstances();
            else
                instances = (Dictionary<string, SvnInstance>)_instances.Target;

            if (!instances.ContainsKey(instanceName))
            {
                var returnInstance = new SvnInstance(instanceName);
                instances.Add(instanceName, returnInstance);

                SaveInstances(instances);

                return returnInstance;
            }

            return instances[instanceName];
        }

        public void RemoveInstance(string instanceName)
        {
            Dictionary<string, SvnInstance> instances;
            if (!_instances.IsAlive)
                _instances.Target = instances = FileInstances();
            else
                instances = (Dictionary<string, SvnInstance>)_instances.Target;

            instances.Remove(instanceName);

            SaveInstances(instances);
        }

        public SvnGroup CreateOrGetGroup(string groupName)
        {

            Dictionary<string, SvnGroup> groups;
            if (!_groups.IsAlive)
                _groups.Target = groups = FileGroups();
            else
                groups = (Dictionary<string, SvnGroup>) _groups.Target;

            if (!groups.ContainsKey(groupName))
            {
                var returnGroup = new SvnGroup(groupName);
                groups.Add(groupName, returnGroup);

                SaveGroups(groups);

                return returnGroup;
            }

            return groups[groupName];
        }

        public SvnGroup GetGroup(string groupName)
        {
            Dictionary<string, SvnGroup> groups;
            if (!_groups.IsAlive)
                _groups.Target = groups = FileGroups();
            else
                groups = (Dictionary<string, SvnGroup>)_groups.Target;

            return groups[groupName];
        }

        public SvnInstance GetInstance(string instanceName)
        {
            Dictionary<string, SvnInstance> instances;
            if (!_instances.IsAlive)
                _instances.Target = instances = FileInstances();
            else
                instances = (Dictionary<string, SvnInstance>)_instances.Target;

            return instances[instanceName];
        }

        private void SaveGroups(Dictionary<string , SvnGroup> groups)
        {
            using (TextWriter textWriter = new StreamWriter(_fileGroup))
                foreach (var svnGroup in groups)
                    textWriter.WriteLine(svnGroup.Value.ToString());
        }

        private void SaveInstances(Dictionary<string, SvnInstance> instances)
        {
            using (TextWriter textWriter = new StreamWriter(_fileInstance))
                foreach (var svnInstance in instances)
                    textWriter.WriteLine(svnInstance.Value+"\n");
        }

        public void Save()
        {
            
            using(TextWriter textWriter = File.CreateText(_fileOverride))
            {
                Dictionary<string, SvnGroup> groups;
                if (!_groups.IsAlive)
                    _groups.Target = groups = FileGroups();
                else
                    groups = (Dictionary<string, SvnGroup>)_groups.Target;

                textWriter.WriteLine("[groups]");
                foreach (var svnGroup in groups)
                    textWriter.WriteLine(svnGroup.ToString());

                Dictionary<string, SvnInstance> instances;
                if (!_instances.IsAlive)
                    _instances.Target = instances = FileInstances();
                else
                    instances = (Dictionary<string, SvnInstance>)_instances.Target;

                foreach (var svnInstance in instances)
                    textWriter.WriteLine(svnInstance.ToString());
            }

            OverrideFile(_fileOverride, _file);
        }

        public void Dispose()
        {
            Save();            
        }

        #region Svn_authorization_file

        //private void MergeFiles()
        //{
        //    var tmpFile = Path.Combine(_path, String.Format(TmpFormat, _fileName));
        //    if(File.Exists(_fileGroup))
        //        File.Copy(_fileGroup, tmpFile);

        //    File.AppendAllText(tmpFile, File.ReadAllText(_fileInstance));

        //    OverrideFile(tmpFile, Path.Combine(_path, _fileName));
        //}

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