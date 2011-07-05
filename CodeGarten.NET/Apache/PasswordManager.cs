using System;
using System.Configuration;
using System.Diagnostics;

namespace Apache
{
    public static class PasswordManager
    {
        public enum EncodeType
        {
            Md5 = 'm',
            Sha1 = 's',
            PlainText = 'p'
        }

        private static readonly string Db = ConfigurationManager.AppSettings["db"];
        private static readonly string Executable = ConfigurationManager.AppSettings["htdbmExecutable"];

        public static bool CreateUser(string name, string passwd, EncodeType encodeType)
        {
            string response = ExecuteWith(String.Format("-cb{0} {1} {2} {3}", (char) encodeType, Db, name, passwd));
            return response.Contains("created.");
        }

        public static bool DeleteUser(string name)
        {
            string response = ExecuteWith(String.Format("-x {0} {1}", Db, name));
            return response.Contains("modified.");
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