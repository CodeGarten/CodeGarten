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

    //public interface IAuthorizationGroup

    public interface IAuthorizationPermissions
    {
        string GroupName(long structureId, long containerId, string roleTypeName);
        void CreateGroup(string groupName, IEnumerable<string> users);
        void AddGroupPermissions(string instanceName, string groupName, IEnumerable<string> permissions);
        void AddUserPermissions(string instanceName, string username, IEnumerable<string> permissions);
    }

    public interface IAuthorizationInstance : IAuthorizationPermissions
    {
        void CreateInstance(string instanceName);
        void DeleteInstance(string instanceName);
        string InstanceName(long structureId, long container, string workspaceTypeName);
    }
}