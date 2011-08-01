using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;


namespace SVN
{
    public sealed class SVNRepositoryManager
    {
        private readonly String _parentPath;
        private readonly String _repoName;

        private SVNRepositoryManager(String parentPath, String repoName)
        {
            _parentPath = parentPath;
            _repoName = repoName;
        }

        public bool Initialize()
        {
            return SVNAdmin.CreateRepositoy(Path.Combine(_parentPath, _repoName));
        }

        public static SVNRepositoryManager Create(String parentPath, String repoName)
        {
            var path = Path.Combine(parentPath, repoName);

            if (Directory.Exists(path))
                return null;
            Directory.CreateDirectory(path);
            return new SVNRepositoryManager(parentPath, repoName);
        }

        public static bool Delete(String parentPath, String repoName)
        {
            var path = Path.Combine(parentPath, repoName);
            if (!Directory.Exists(path))
                return false;
            Directory.Delete(path, true);
            return true;
        }
    }
}