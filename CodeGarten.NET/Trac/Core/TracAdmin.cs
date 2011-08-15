using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using CodeGarten.Service;

namespace Trac.Core
{
    public static class TracAdmin
    {
        private struct Response
        {
            public String StandardOutput;
            public String StandardError;
        }

        private static readonly String TracAdminPath = TracConfiguration.Settings.TracAdmin;

        public static bool Add(String userName, String envPath, String permissionOrGroup)
        {
            var responce = Execute(String.Format("{0} permission add {1} {2}", envPath, userName, permissionOrGroup), null);
            return responce.StandardError.Length == 0;
        }

        public static bool Remove(String userName, String envPath, String permissionOrGroup)
        {
            var responce = Execute(String.Format("{0} permission remove {1} {2}", envPath, userName, permissionOrGroup), null);
            return responce.StandardError.Length == 0;
        }

        public static Dictionary<String, List<TracPrivileges>> ListAll(String envPath)
        {         
            var responce = Execute(String.Format("{0} permission list", envPath), null);
            if (responce.StandardError.Length != 0)
                return null;

            var dictionary = new Dictionary<String, List<TracPrivileges>>();

            var list = Parse(responce.StandardOutput);

            for (int i = 0; i < list.Length-1; i += 2)
            {
                var user = list[i];
                if (!dictionary.ContainsKey(user))
                    dictionary.Add(user, new List<TracPrivileges>());
                TracPrivileges outParse;
                if(Enum.TryParse<TracPrivileges>(list[i + 1], out outParse))
                    dictionary[user].Add(outParse);
            }

            return dictionary;
        }

        public static List<TracPrivileges> List(String userOrGroupName, String envPath)
        {
            var responce = Execute(String.Format("{0} permission list {1}", envPath, userOrGroupName), null);
            if (responce.StandardError.Length != 0)
                return null;

            var array = Parse(responce.StandardOutput);

            var list = new List<TracPrivileges>();

            for (int i = 1; i < array.Length; i += 2)
            {
                TracPrivileges outParse;
                if (Enum.TryParse<TracPrivileges>(array[i], out outParse))
                    list.Add(outParse);
            }

            return list;
        }

        public static List<String> ListGroup(String envPath)
        {
            var responce = Execute(String.Format("{0} permission list", envPath), null);
            if (responce.StandardError.Length != 0)
                return null;

            var array = Parse(responce.StandardOutput);

            var list = new List<String>();

            for (int i = 1; i < array.Length; i += 2)
            {
                TracPrivileges outParse;
                if (!Enum.TryParse<TracPrivileges>(array[i], out outParse))
                    list.Add(array[i]);
            }

            return list;

        }
        
        public static List<String> ListGroup(String userName, String envPath)
        {
            var responce = Execute(String.Format("{0} permission list", envPath), null);
            if (responce.StandardError.Length != 0)
                return null;

            var array = Parse(responce.StandardOutput);

            var list = new List<String>();

            for (int i = 1; i < array.Length; ++i)
            {
                TracPrivileges outParse;
                if (array[i].Equals(userName) && !Enum.TryParse<TracPrivileges>(array[i+1], out outParse))
                    list.Add(array[i+1]);
            }

            return list;
        }

        private static String[] Parse(String str)
        {
            return str.Split(
                                new String[] { "-\r\n", "actions:\r\n" },
                                StringSplitOptions.None
                            )[1].Split(new string[] { " ", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static bool InitEnv(String envName, String envPath)
        {
            var responce = Execute(String.Format(@"{0} initenv", envPath), String.Format("{0}\n\n",envName));
            return responce.StandardError.Length == 0;
        }

        private static Response Execute(String args, String input)
        {
            
            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo()
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    FileName = TracAdminPath,
                    Arguments = args
                };

                if (input != null)
                {
                    process.StartInfo.RedirectStandardInput = true;
                    process.Start();
                    process.StandardInput.Write(input);
                    process.StandardInput.Close();
                }
                else
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
