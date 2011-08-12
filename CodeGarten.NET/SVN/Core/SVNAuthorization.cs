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

    public sealed class SvnInstance
    {
        private readonly StringBuilder _instance;

        public SvnInstance(string instanceName)
        {
            _instance = new StringBuilder(String.Format("[{0}:/]\n", instanceName));
        }

        public void AddGroupPermission(string groupName, string privilege)
        {
            _instance.AppendFormat("@{0} = {1}\n", groupName, privilege);
        }

        public void AddUserPermission(string userName, string privilege)
        {
            _instance.AppendFormat("{0} = {1}\n", userName, privilege);
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

    //TODO thread-safe
    public sealed class SVNAuthorization : IDisposable
    {
        private readonly WeakReference _wkGroups, _wkInstances;
        private Dictionary<string, SvnInstance>  _anchorInstances;
        private Dictionary<string, SvnGroup> _anchorGroups;
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

            if (!File.Exists(_fileGroup))
                using (var textWriter = File.CreateText(_fileGroup))
                    textWriter.WriteLine("[groups]");
                
                

            if(!File.Exists(_fileInstance))
                File.Create(_fileInstance).Close();

            _wkInstances = new WeakReference(null);
            _wkGroups = new WeakReference(null);

        }

        private Dictionary<string, SvnInstance> ParseFileInstances()
        {
            using (TextReader textReader = new StreamReader(_fileInstance))
            {
                string line;
                var instances = new Dictionary<string, SvnInstance>();

                while ((line = textReader.ReadLine()) != null)
                {
                    if (!line.StartsWith("["))
                        continue;

                    var instanceName = line.Substring(1, line.IndexOf(':')-1);
                    var instance = new SvnInstance(instanceName);

                    while ((line = textReader.ReadLine()) != null && line != "")
                    {
                        var itens = line.Split(new[] { '@', ' ', '=' }, StringSplitOptions.RemoveEmptyEntries);
                        var groupName = itens[0];
                        instance.AddGroupPermission(groupName, itens[1]);
                    }

                    instances.Add(instanceName, instance);
                }

                return instances;
            }
        }

        private Dictionary<string, SvnGroup> ParseFileGroups()
        {
            using (TextReader textReader = new StreamReader(_fileGroup))
            {
                string line;
                var groups = new Dictionary<string, SvnGroup>();
                while ((line = textReader.ReadLine()) != null)
                {
                    if (line == "" || line.StartsWith("["))
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

        private void AnchorInstances()
        {
            if (_anchorInstances != null)
                return;

            if (_wkInstances.IsAlive)
                _anchorInstances = (Dictionary<string, SvnInstance>)_wkInstances.Target;
            else
                _wkInstances.Target = _anchorInstances = ParseFileInstances();
        }

        private void AnchorGroups()
        {
            if (_anchorGroups != null)
                return;

            if (_wkGroups.IsAlive)
                _anchorGroups = (Dictionary<string, SvnGroup>) _wkGroups.Target;
            else
                _wkGroups.Target = _anchorGroups = ParseFileGroups();
        }

        public SvnInstance CreateInstance(string instanceName)
        {
            if(_anchorInstances==null)
                _anchorInstances = new Dictionary<string, SvnInstance>();

            if (!_anchorInstances.ContainsKey(instanceName))
            {
                var returnInstance = new SvnInstance(instanceName);
                _anchorInstances.Add(instanceName, returnInstance);

                return returnInstance;
            }

            return _anchorInstances[instanceName];
        }


        public SvnGroup CreateGroup(string groupName)
        {
            if(_anchorGroups==null)
                _anchorGroups = new Dictionary<string, SvnGroup>();

            if (!_anchorGroups.ContainsKey(groupName))
            {
                var returnGroup = new SvnGroup(groupName);
                _anchorGroups.Add(groupName, returnGroup);

                return returnGroup;
            }

            return _anchorGroups[groupName];
        }

        public bool RemoveInstance(string instanceName)
        {
            AnchorInstances();

            return _anchorInstances.Remove(instanceName);
        }

        public bool RemoveGroups(string groupName)
        {
            AnchorGroups();

            return _anchorGroups.Remove(groupName);
        }

        public SvnGroup GetGroup(string groupName)
        {
            AnchorGroups();

            return _anchorGroups[groupName];
        }

        public SvnInstance GetInstance(string instanceName)
        {
            AnchorInstances();

            return _anchorInstances[instanceName];
        }

        private void SaveGroups()
        {
            if (_anchorGroups == null) return;

            TextWriter textWriter;
            if(_wkGroups.IsAlive)
            {
                textWriter = new StreamWriter(_fileGroup);
                textWriter.WriteLine("[groups]");
            }
            else
                textWriter = File.AppendText(_fileGroup);
                                
            foreach (var svnGroup in _anchorGroups)
                textWriter.Write(svnGroup.Value.ToString());
            
            textWriter.Dispose();    
        }

        private void SaveInstances()
        {
            if (_anchorInstances == null) return;

            TextWriter textWriter = 
                _wkInstances.IsAlive ? new StreamWriter(_fileInstance) : File.AppendText(_fileInstance);

            foreach (var svnInstance in _anchorInstances)
                textWriter.WriteLine(svnInstance.Value.ToString());
            
            textWriter.Dispose();
        }

        public void Save()
        {

            SaveGroups();
            SaveInstances();

            if (_anchorGroups != null && _wkGroups.IsAlive)
                using (TextWriter textWriter = File.CreateText(_fileOverride))
                {
                    textWriter.WriteLine("[groups]");
                    foreach (var svnGroup in _anchorGroups)
                        textWriter.WriteLine(svnGroup.Value.ToString());
                }
            else
                File.Copy(_fileGroup, _fileOverride);

            if (_anchorInstances != null && _wkInstances.IsAlive)
                using (TextWriter textWriter = File.AppendText(_fileOverride))
                    foreach (var svnInstance in _anchorInstances)
                        textWriter.WriteLine(svnInstance.Value.ToString());
            else
                File.AppendAllText(_fileOverride, File.ReadAllText(_fileInstance));

            _anchorInstances = null;
            _anchorGroups = null;

            OverrideFile(_fileOverride, _file);
        }

        public void Dispose()
        {
            Save();
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
                    { }
                } while (true);
            }

            File.Move(sourceFileName, destFileName);
        }
    }
}