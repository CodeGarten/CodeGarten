using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using CodeGarten.Service;

namespace SVN.Core
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

            var assemblyConfig =
                ConfigurationManager.OpenExeConfiguration(Path.Combine(ServiceConfig.ServicesDllLocation, "SVN.dll"));
            
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