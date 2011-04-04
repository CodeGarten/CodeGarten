using System;
using System.Configuration;
using System.IO;
using GitSharp.Commands;
using GitSharp.Core;

namespace Git
{
    static class GitFileSystem
    {
        private static readonly string Extension = ConfigurationManager.AppSettings["extension"];
        private static readonly string RepositoryFolder = ConfigurationManager.AppSettings["repositoryFolder"];

        public static bool DeleteRepository(string name)
        {
            var fullPath = String.Format(@"{0}\{1}{2}", RepositoryFolder, name, Extension);

            if (Directory.Exists(fullPath))
            {
                Directory.Delete(fullPath, true);
                return true;
            }

            return false;
        }

        public static bool CreateRepository(string name)
        {
            var fullPath = String.Format(@"{0}\{1}{2}",RepositoryFolder , name, Extension);

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
