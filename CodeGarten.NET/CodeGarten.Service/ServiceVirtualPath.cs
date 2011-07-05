using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Hosting;

namespace CodeGarten.Service
{
    //TODO fazer isto muito melhor URGENTE
    public class ServiceVirtualPath : VirtualPathProvider
    {
        private bool IsServicePath(string virtualPath)
        {
            var relativePath = VirtualPathUtility.ToAppRelative(virtualPath);
            return relativePath.StartsWith("~/Services", StringComparison.CurrentCultureIgnoreCase);
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            if (IsServicePath(virtualPath))
                return new ServiceVirtualFile(virtualPath);
            return base.GetFile(virtualPath);
        }

        public override bool FileExists(string virtualPath)
        {
            return IsServicePath(virtualPath) || base.FileExists(virtualPath);
        }

        public override System.Web.Caching.CacheDependency GetCacheDependency(string virtualPath,
                                                                              System.Collections.IEnumerable
                                                                                  virtualPathDependencies,
                                                                              DateTime utcStart)
        {
            if (IsServicePath(virtualPath))
                return null;
            return base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
        }
    }

    public class ServiceVirtualFile : VirtualFile
    {
        private string _relativePath;

        public ServiceVirtualFile(string virtualPath) : base(virtualPath)
        {
            _relativePath = VirtualPathUtility.ToAppRelative(virtualPath);
        }

        public override Stream Open()
        {
            var parts = _relativePath.Split('/');
            var assemblyName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, parts[1], parts[2]);

            //TODO assembly not found
            var assembly = Assembly.LoadFile(assemblyName);

            var startPosition = parts[0].Length + parts[1].Length + parts[2].Length + 3;

            var resourcePath = _relativePath.Substring(startPosition).Replace('/', '.');

            if (assembly != null && resourcePath != "")
                return assembly.GetManifestResourceStream(resourcePath);

            return null;
        }
    }
}