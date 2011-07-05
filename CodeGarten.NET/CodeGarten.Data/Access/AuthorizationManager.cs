using System;
using System.Collections.Generic;
using System.Linq;
using CodeGarten.Data.Interfaces;
using CodeGarten.Data.Model;

namespace CodeGarten.Data.Access
{
    public sealed class AuthorizationManager
    {
        private readonly Context _dbContext;

        public AuthorizationManager(DataBaseManager db)
        {
            _dbContext = db.DbContext;
        }

        private bool ContainsRoleType(Structure structure, Container container, RoleType roleType)
        {
            return _dbContext.Enrolls.Where(
                (cu) =>
                cu.ContainerId == container.Id &&
                cu.RoleType.Name == roleType.Name &&
                cu.RoleTypeStructureId == structure.Id
                       ).Count() != 0;
        }


        private void CreateGroups(IAuthorization authorization, long structure, long container)
        {
            var roletypes = _dbContext.Enrolls.Where(
                (c) => c.ContainerId == container
                ).Select(
                    (c) => c.RoleType
                ).Distinct();


            foreach (RoleType roletype in roletypes)
            {
                var roletypeContext = roletype;
                var users = _dbContext.Enrolls.Where(
                    (cu) =>
                    cu.ContainerId == container &&
                    cu.RoleTypeName == roletypeContext.Name &&
                    cu.RoleTypeStructureId == structure
                    ).Select(
                        (cu) =>
                        cu.User.Name
                    );

                authorization.AddGroup(authorization.GetGroupName(structure, container, roletype.Name), users);
            }
        }


        private void SetCurrentAndParentRoleTypes(
            IAuthorization authorization,
            Structure structure,
            Container rootContainer,
            string rootName,
            string service,
            IEnumerable<Role> roles
            )
        {
            Container current = rootContainer;

            do
            {
                foreach (var role in roles)
                    if (ContainsRoleType(structure, current, role.RoleType))
                    {
                        var permissions = role.Rule.Permissions.Where(
                            (sp) =>
                            sp.ServiceName == service
                            ).Select(
                                (sp) =>
                                sp.Name
                            );

                        authorization.AddGroupPermission(
                            rootName,
                            authorization.GetGroupName(structure.Id, current.Id, role.RoleTypeName),
                            permissions
                            );
                    }

                current = current.ParentContainer;
            } while (current != null);
        }


        private void SetChildsRoleTypes(
            IAuthorization authorization,
            Structure structure,
            Container rootContainer,
            string rootName,
            string service,
            IEnumerable<Role> roles
            )
        {
            if (rootContainer.Childs == null) return;

            foreach (var child in rootContainer.Childs)
            {
                SetChildsRoleTypes(authorization, structure, child, service, rootName, roles);

                foreach (var role in roles)
                    if (ContainsRoleType(structure, child, role.RoleType))
                    {
                        var permissions = role.Rule.Permissions.Where(
                            (sp) =>
                            sp.ServiceName == service
                            ).Select(
                                (sp) => sp.Name
                            );

                        authorization.AddGroupPermission(
                            rootName,
                            authorization.GetGroupName(structure.Id, child.Id, role.RoleTypeName),
                            permissions
                            );
                    }
            }
        }

        public void CreateServiceAuthorizationStruct(IAuthorization authorization, string serviceName)
        {
            if (authorization == null) throw new ArgumentNullException("authorization");

            foreach (Structure structure in _dbContext.Structures)
            {
                var structureContext = structure;
                var containers =
                    _dbContext.Containers.Where((c) => c.ContainerPrototype.StructureId == structureContext.Id);

                foreach (Container container in containers)
                {
                    CreateGroups(authorization, structure.Id, container.Id);

                    foreach (WorkSpaceType workSpaceType in container.ContainerPrototype.WorkSpaceTypes)
                    {
                        var containerContext = container;
                        var workSpaceTypeContext = workSpaceType;

                        var rootName = authorization.GetContainerName(structure.Id, container.Id, workSpaceType.Name);

                        authorization.CreateContainer(rootName);

                        var roles = _dbContext.Roles.Where(
                            (r) =>
                            r.ContainerPrototypeStructureId == structureContext.Id &&
                            r.WorkSpaceTypeStructureId == structureContext.Id &&
                            r.WorkSpaceTypeName == workSpaceTypeContext.Name &&
                            r.ContainerPrototypeName == containerContext.ContainerPrototype.Name
                            );

                        SetCurrentAndParentRoleTypes(authorization, structure, container, rootName, serviceName, roles);

                        SetChildsRoleTypes(authorization, structure, container, rootName, serviceName, roles);
                    }
                }
            }

            authorization.Dispose();
        }
    }
}