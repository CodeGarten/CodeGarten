using System;
using System.IO;
using GitSharp.Commands;
using GitSharp.Core;

namespace Git.Core
{
    public sealed class FileSystem
    {
        private readonly DirectoryInfo _base;

        public Repository this[string instance]
        {
            get
            {
                return !Directory.Exists(String.Format(@"{0}\{1}.git", _base, instance))
                           ? null
                           : Repository.Open(String.Format(@"{0}\{1}.git", _base, instance));
            }
        }

        public FileSystem(string base_path)
        {
            _base = new DirectoryInfo(base_path);
            _base.Create();
        }

        public bool DeleteRepository(string name)
        {
            var fullPath = String.Format(@"{0}\{1}.git", _base, name);

            if (Directory.Exists(fullPath))
            {
                Directory.Delete(fullPath, true);
                return true;
            }

            return false;
        }

        public bool CreateRepository(string name)
        {
            var fullPath = String.Format(@"{0}\{1}.git", _base, name);

            if (Directory.Exists(fullPath))
                return false;

            new InitCommand
                {
                    GitDirectory = fullPath,
                    Bare = true,
                    Quiet = false
                }.Execute();

            var repository = Repository.Open(fullPath);
            repository.Config.setBoolean("http", null, "receivepack", true);
            repository.Config.save();
            return true;
        }
    }
}