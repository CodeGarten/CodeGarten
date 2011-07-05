using System;
using System.Collections.Generic;

namespace CodeGarten.Data.Interfaces
{
    public interface IAuthorization : IDisposable
    {
        string GetGroupName(long structure, long container, string roleType);
        string GetContainerName(long structure, long container, string workSpace);
        void AddGroup(string container, IEnumerable<string> users);
        void CreateContainer(string container);
        void AddGroupPermission(string container, string group, IEnumerable<string> permissions);
        void AddUserPermission(string container, string user, IEnumerable<string> permissions);
    }
}