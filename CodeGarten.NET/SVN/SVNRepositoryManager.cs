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
            return SVNAdmin.CreateRepositoy(String.Format(@"{0}\{1}", _parentPath, _repoName));
        }

        public static SVNRepositoryManager Create(String parentPath, String repoName)
        {
            var path = String.Format(@"{0}\{1}", parentPath, repoName);
            if (Directory.Exists(path))
                return null;
            Directory.CreateDirectory(path);
            return new SVNRepositoryManager(parentPath, repoName);
        }
    }
}
