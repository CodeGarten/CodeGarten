﻿using System;
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
            
            if(!File.Exists(_fileGroup))
                File.Create(_fileGroup).Close();

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

                    var instanceName = line.Substring(1, line.IndexOf(':'));
                    var instance = new SvnInstance(instanceName);

                    while ((line = textReader.ReadLine()) != null && line != "")
                    {
                        var itens = line.Split(new[] { '@', ' ', '=' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        var groupName = itens[0];
                        itens.RemoveAt(0);
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
                    if (line == "")
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
            AnchorInstances();

            if (!_anchorInstances.ContainsKey(instanceName))
            {
                var returnInstance = new SvnInstance(instanceName);
                _anchorInstances.Add(instanceName, returnInstance);

                return returnInstance;
            }

            return _anchorInstances[instanceName];
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

        public SvnGroup CreateGroup(string groupName)
        {
            AnchorGroups();

            if (!_anchorGroups.ContainsKey(groupName))
            {
                var returnGroup = new SvnGroup(groupName);
                _anchorGroups.Add(groupName, returnGroup);                

                return returnGroup;
            }

            return _anchorGroups[groupName];
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
            using (TextWriter textWriter = new StreamWriter(_fileGroup))
            {
                foreach (var svnGroup in _anchorGroups)
                    textWriter.WriteLine(svnGroup.Value.ToString());
                _anchorGroups = null;
            }
                
        }

        private void SaveInstances()
        {
            if (_anchorInstances == null) return;
            using (TextWriter textWriter = new StreamWriter(_fileInstance))
            {
                foreach (var svnInstance in _anchorInstances)
                    textWriter.WriteLine(svnInstance.Value + "\n");
                _anchorInstances = null;
            }
                
        }

        public void Save()
        {
            
            if(_anchorGroups!=null)
                using (TextWriter textWriter = File.CreateText(_fileOverride))
                {
                    textWriter.WriteLine("[groups]");
                    foreach (var svnGroup in _anchorGroups)
                        textWriter.WriteLine(svnGroup.Value.ToString());
                }
            else
                File.Copy(_fileGroup, _fileOverride);
                
            if(_anchorInstances!=null)
                using (TextWriter textWriter = File.AppendText(_fileOverride))
                    foreach (var svnInstance in _anchorInstances)
                        textWriter.WriteLine(svnInstance.Value.ToString());
            else
                File.AppendAllText(_fileOverride, File.ReadAllText(_fileInstance));

            OverrideFile(_fileOverride, _file);

            SaveGroups();
            SaveInstances();
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