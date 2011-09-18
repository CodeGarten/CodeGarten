using System;
using System.Configuration;
using System.Diagnostics;
using CodeGarten.Service.Interfaces;
using System.IO;
using CodeGarten.Service;

namespace Apache
{
    public static class AuthenticationManager
    {
        public enum EncodeType
        {
            Md5 = 'm',
            Sha1 = 's',
            PlainText = 'p'
        }

        private static readonly string Db;
        private static readonly string Executable;

        static AuthenticationManager()
        {
            var assemblyConfig =
                ConfigurationManager.OpenExeConfiguration(Path.Combine(ServiceConfig.ServicesDllLocation, "Apache.dll"));

            Db = assemblyConfig.AppSettings.Settings["db"].Value;
            Executable = assemblyConfig.AppSettings.Settings["htdbmExecutable"].Value;
        
        }

        public static bool CreateUser(string user, string password)
        {
            string response = ExecuteWith(String.Format("-cb{0} {1} {2} {3}", (char)EncodeType.Sha1, Db, user, password));
            return response.Contains("created."); ;
        }

        public static bool DeleteUser(string user)
        {
            string response = ExecuteWith(String.Format("-x {0} {1}", Db, user));
            return response.Contains("modified.");
        }

        public static bool ChangePassword(string user, string newPassword)
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