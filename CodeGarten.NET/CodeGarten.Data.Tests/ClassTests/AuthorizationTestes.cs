using System;
using System.Collections.Generic;
using System.Text;
using CodeGarten.Data.Interfaces;

namespace CodeGarten.Data.Tests.ClassTests
{
    internal class AuthorizationTestes : IAuthorization
    {
        private readonly StringBuilder _groups;
        private readonly Dictionary<String, StringBuilder> _repositories;

        private static StringBuilder CreateRepository(String repoName)
        {
            return new StringBuilder(String.Format("[{0}:/]\n", repoName));
        }

        private static void AddUserToRepo(StringBuilder repo, String userName, string privilege)
        {
            repo.AppendFormat("{0} = {1}\n", userName, privilege);
        }

        private static void AddGroupToRepo(StringBuilder repo, String groupName, string privilege)
        {
            repo.AppendFormat("@{0} = {1}\n", groupName, privilege);
        }

        public AuthorizationTestes()
        {
            _repositories = new Dictionary<string, StringBuilder>();
            _groups = new StringBuilder("[groups]\n");
        }

        public void Dispose()
        {
            Console.WriteLine(_groups);
            Console.WriteLine("///////////////////////////////");
            foreach (var repository in _repositories)
                Console.WriteLine(repository.Value.ToString());

            Console.ReadLine();
        }

        public string GetGroupName(long structure, long container, string roletype)
        {
            return String.Format("{0}{1}{2}", structure, container, roletype);
        }


        public string GetContainerName(long structure, long container, string workSpace)
        {
            return String.Format("{0}{1}{2}", structure, container, workSpace);
        }

        public void AddGroup(string containerName, IEnumerable<string> users)
        {
            _groups.AppendFormat("{0} =", containerName);

            foreach (var user in users)
                _groups.AppendFormat(" {0},", user);

            _groups[_groups.Length - 1] = '\n';
        }

        public void CreateContainer(string containerName)
        {
            if (_repositories.ContainsKey(containerName))
                throw new Exception("repositorio ja existe");

            _repositories.Add(containerName, CreateRepository(containerName));
        }

        public void AddGroupPermission(string containerName, string groupName, IEnumerable<string> permissions)
        {
            foreach (var permission in permissions)
            {
                AddGroupToRepo(_repositories[containerName], groupName, permission);
            }
        }

        public void AddUserPermission(string containerName, string userName, IEnumerable<string> permissions)
        {
            foreach (var permission in permissions)
            {
                AddUserToRepo(_repositories[containerName], userName, permission);
            }
        }
    }
}