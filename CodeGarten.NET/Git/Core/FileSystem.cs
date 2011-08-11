using System;
using System.IO;
using GitSharp.Commands;
using GitSharp.Core;

namespace Git.Core
{
    internal sealed class FileSystem
    {
        private readonly DirectoryInfo _base;

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

        public bool HasRepository(string name)
        {
            return _base.GetDirectories(string.Format("{0}.git", name), SearchOption.TopDirectoryOnly).Length != 0;
        }
    }
}