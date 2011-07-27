using System;
using System.Collections.Generic;
using System.Linq;
using CodeGarten.Data.Interfaces;
using CodeGarten.Data.Model;

namespace CodeGarten.Data.Access
{
    //TODO code quarantined
    //public sealed  class ServiceAuthorizatonInstance
    //{
    //    public Container Container { get; internal set; }
    //    public WorkSpaceType WorkSpaceType { get; internal set; }
    //    public IEnumerable<KeyValuePair<Role, IEnumerable<User>>> GroupRoleUsers { get; internal set; }
    //}

    public sealed class AuthorizationManager
    {
        private readonly Context _dbContext;

        public AuthorizationManager(DataBaseManager db)
        {
            _dbContext = db.DbContext;
        }

        
        //private void CreateGroups(IAuthorizationPermissions authorization, long structure, long container)
        //{
        //    var roletypes = _dbContext.Enrolls.Where(
        //        (c) => c.ContainerId == container
        //        ).Select(
        //            (c) => c.RoleType
        //        ).Distinct();


        //    foreach (RoleType roletype in roletypes)
        //    {
        //        var roletypeContext = roletype;
        //        var users = _dbContext.Enrolls.Where(
        //            (cu) =>
        //            cu.ContainerId == container &&
        //            cu.RoleTypeName == roletypeContext.Name &&
        //            cu.RoleTypeStructureId == structure
        //            ).Select(
        //                (cu) =>
        //                cu.User.Name
        //            );

        //        authorization.CreateGroup(authorization.GroupName(structure, container, roletype.Name), users);
        //    }
        //}

        //public void CreateServiceAuthorization(IAuthorizationPermissions authorization, Container container, WorkSpaceType workSpaceType, string serviceName)
        //{
        //        var structureId = workSpaceType.StructureId;

        //        var roles = _dbContext.Roles.Where(
        //            (r) =>
        //            r.ContainerPrototypeStructureId == structureId &&
        //            r.WorkSpaceTypeStructureId == structureId &&
        //            r.WorkSpaceTypeName == workSpaceType.Name &&
        //            r.ContainerPrototypeName == container.ContainerPrototype.Name
        //            );

        //        foreach (var role in roles)
        //        {
        //            var groupName = authorization.GroupName(structureId, container.Id,
        //                                                    role.RoleTypeName);
                    
        //            var permissions = role.Rule.Permissions.Where(
        //                                                    (sp) =>
        //                                                    sp.ServiceName == serviceName
        //                                                    ).Select(
        //                                                        (sp) =>
        //                                                        sp.Name
        //                                                    );

        //            //authorization.AddGroupPermissions(rootName, groupName, permissions);
        //        }

        //}

        //public void CreateServiceAuthorizationStruct(IAuthorizationInstance authorization, string serviceName)
        //{
        //    if (authorization == null) throw new ArgumentNullException("authorization");

        //    foreach (Structure structure in _dbContext.Structures)
        //    {
        //        var structureContext = structure;
        //        var containers =
        //            _dbContext.Containers.Where((c) => c.ContainerPrototype.StructureId == structureContext.Id);

        //        foreach (Container container in containers)
        //        {
        //            CreateGroups(authorization, structure.Id, container.Id);

        //            foreach (WorkSpaceType workSpaceType in container.ContainerPrototype.WorkSpaceTypes)
        //            {
        //                var containerContext = container;
        //                var workSpaceTypeContext = workSpaceType;

        //                var rootName = authorization.InstanceName(structure.Id, container.Id, workSpaceType.Name);

        //                authorization.CreateInstance(rootName);

        //                var roles = _dbContext.Roles.Where(
        //                    (r) =>
        //                    r.ContainerPrototypeStructureId == structureContext.Id &&
        //                    r.WorkSpaceTypeStructureId == structureContext.Id &&
        //                    r.WorkSpaceTypeName == workSpaceTypeContext.Name &&
        //                    r.ContainerPrototypeName == containerContext.ContainerPrototype.Name
        //                    );

        //                foreach (var role in roles)
        //                {
        //                    var groupName = authorization.GroupName(structureContext.Id, containerContext.Id,
        //                                                            role.RoleTypeName);

        //                    var permissions = role.Rule.Permissions.Where(
        //                                                            (sp) =>
        //                                                            sp.ServiceName == serviceName
        //                                                            ).Select(
        //                                                                (sp) =>
        //                                                                sp.Name
        //                                                            );

        //                    authorization.AddGroupPermissions(rootName, groupName, permissions);    
        //                }

        //                //SetCurrentAndParentRoleTypes(authorization, structure, container, rootName, serviceName, roles);

        //                //SetChildsRoleTypes(authorization, structure, container, rootName, serviceName, roles);
        //            }
        //        }
        //    }

        //    var disposable = authorization as IDisposable;
        //    if(disposable!=null)
        //        disposable.Dispose();
        //}

        //public ServiceAuthorizatonInstance GetServiceAuthorizationInstance(long structure, long containerId, string workspaceType)
        //{
        //    var containerObj = ContainerManager.Get(_dbContext, containerId);
            
        //    var roles =
        //        _dbContext.Roles.Where(
        //            r =>
        //            r.ContainerPrototypeStructureId == structure &&
        //            r.WorkSpaceTypeStructureId == structure &&
        //            r.WorkSpaceTypeName == workspaceType &&
        //            r.ContainerPrototypeName == containerObj.ContainerPrototype.Name
        //            );

        //    var list = new LinkedList<KeyValuePair<Role, IEnumerable<User>>>();
        //    foreach (var role in roles)
        //    {
        //        var roleContext = role;
        //        var users =
        //            _dbContext.Enrolls.Where(e => e.ContainerId == containerId && e.RoleTypeName == roleContext.RoleTypeName).Select(e => e.User);
        //        list.AddFirst(new KeyValuePair<Role, IEnumerable<User>>(role, users));
        //    }

        //    var serviceAuthorization = new ServiceAuthorizatonInstance()
        //                                   {
        //                                       Container = containerObj,
        //                                       WorkSpaceType = WorkSpaceTypeManager.Get(_dbContext, structure, workspaceType),
        //                                       GroupRoleUsers = list
        //                                   };

        //    return serviceAuthorization;
        //}

        //public IEnumerable<ServiceAuthorizatonInstance> GetAllServiceAuthorizationInstances()
        //{
        //    foreach (var container in _dbContext.Containers)
        //        foreach (var workSpaceType in container.ContainerPrototype.WorkSpaceTypes)
        //            yield return
        //                GetServiceAuthorizationInstance(container.ContainerPrototype.StructureId, container.Id,
        //                                                workSpaceType.Name);
        //    yield break;
        //}
    }
}