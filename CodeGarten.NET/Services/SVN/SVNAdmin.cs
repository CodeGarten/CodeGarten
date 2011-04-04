using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SVN
{
    static class SVNAdmin
    {
        private struct Response
        {
            public String StandardOutput;
            public String StandardError;
        }

        private static readonly String SvnAdminPath;

        static SVNAdmin()
        {
            SvnAdminPath = ConfigurationManager.AppSettings["SvnAdminLocation"];
        }

        public static bool CreateRepositoy(String svnRepoPath)
        {
            var responce = Execute(String.Format("create {0}", svnRepoPath));
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
                    FileName =  SvnAdminPath,
                    Arguments = args
                };

                process.Start();

                var response = new Response()
                {
                    StandardOutput = process.StandardOutput.ReadToEnd(),
                    StandardError = process.StandardError.ReadToEnd()
                };

                process.WaitForExit();

                return response;
            }
        }
    }
}
