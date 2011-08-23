using System;
using System.Configuration;
using System.Diagnostics;
using CodeGarten.Service.Interfaces;

namespace Apache
{
    public class ApacheServer : IServer
    {
        public enum EncodeType
        {
            Md5 = 'm',
            Sha1 = 's',
            PlainText = 'p'
        }

        private static readonly string Db = ConfigurationManager.AppSettings["db"];
        private static readonly string Executable = ConfigurationManager.AppSettings["htdbmExecutable"];

        public bool CreateUser(string user, string password)
        {
            string response = ExecuteWith(String.Format("-cb{0} {1} {2} {3}", (char)EncodeType.Sha1, Db, user, password));
            return response.Contains("created."); ;
        }

        public bool DeleteUser(string user)
        {
            string response = ExecuteWith(String.Format("-x {0} {1}", Db, user));
            return response.Contains("modified.");
        }

        public bool ChangePassword(string user, string newPassword)
        {
            string response = ExecuteWith(String.Format("-cb{0} {1} {2} {3}", (char)EncodeType.Sha1, Db, user, newPassword));
            return response.Contains("created."); ;
        }

        private static string ExecuteWith(string arguments)
        {
            try
            {
                using (var process = new Process())
                {
                    process.StartInfo = new ProcessStartInfo(Executable)
                                            {
                                                CreateNoWindow = true,
                                                RedirectStandardOutput = true,
                                                UseShellExecute = false,
                                                Arguments = arguments
                                            };
                    process.Start();
                    string response = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    return response;
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}