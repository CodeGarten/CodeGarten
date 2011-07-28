using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting;
using System.Security;
using System.Text;

namespace SVN
{
    internal static class SVNAdmin
    {
        private struct Response
        {
            public String StandardOutput;
            public String StandardError;
        }

        private static readonly String SvnAdminPath;

        static SVNAdmin()
        {
            var fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = @"SVN.dll.config";
            var assemblyConfig = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            SvnAdminPath = assemblyConfig.AppSettings.Settings["SvnAdminLocation"].Value;
        }

        public static bool CreateRepositoy(String svnRepoPath)
        {
            var responce = Execute(String.Format("create {0} --config-dir={0}", svnRepoPath));
            return responce.StandardError.Length == 0;
        }

        private static Response Execute(String args)
        {
            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo()
                                        {
                                            RedirectStandardOutput = true,
                                            RedirectStandardError = true,
                                            UseShellExecute = false,
                                            CreateNoWindow = true,
                                            FileName = SvnAdminPath,
                                            Arguments = args
                                        };

                process.Start();

                var response = new Response()
                                   {
                                       StandardOutput = process.StandardOutput.ReadToEnd(),
                                       StandardError = process.StandardError.ReadToEnd()
                                   };

                process.WaitForExit();

                if (response.StandardError != "")
                    throw new Exception(response.StandardError);

                return response;
            }
        }
    }
}