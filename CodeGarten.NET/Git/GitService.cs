using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using CodeGarten.Data.Access;
using CodeGarten.Data.Model;
using CodeGarten.Service;
using CodeGarten.Service.Utils;
using Git.Core;
using Service = CodeGarten.Service.Service;

namespace Git
{
    [Export(typeof(Service))]
    public sealed class Git : Service
    {
        public readonly Authorization Authorization;
        public readonly FileSystem FileSystem;

        public Git()
            : base(
                new ServiceModel("Git", "Distributed Version Control System", EnumExtensions.ToEnumerable<Privileges>())
                )
        {
            FileSystem = new FileSystem(Path.Combine(PathService, "repositories"));

            Directory.CreateDirectory(Path.Combine(PathService, "etc"));
            Authorization = new Authorization(Path.Combine(PathService, "etc", "autho_file.xml"));
        }

        public override string GetInstancePath(Container container, WorkSpaceType workSpaceType)
        {
            return Path.Combine(Path.Combine(PathService, "repositories"), container.UniqueInstanceName(workSpaceType));
        }

        public override void OnServiceCreating(ServiceBuilder serviceBuilder)
        {
            serviceBuilder.OnCreateContainer += CreateContainer;
            serviceBuilder.OnDeleteContainer += Deletecontainer;
            serviceBuilder.OnEnrollUser += EnrollUser;
            serviceBuilder.OnDisenrollUser += DisenrollUser;
        }

        private void DisenrollUser(object sender, EnrollEventArgs e)
        {
            if (e.Container.Prototype.WorkSpaceTypeWithService(Name).Any())
                Authorization.RemoveUserFromGroup(e.Container.UniqueGroupName(e.Enroll.RoleTypeName), e.Enroll.UserName);
        }

        private void EnrollUser(object sender, EnrollEventArgs e)
        {
            if (e.Container.Prototype.WorkSpaceTypeWithService(Name).Any())
                Authorization.AddUserToGroup(e.Container.UniqueGroupName(e.Enroll.RoleTypeName), e.Enroll.UserName);
        }

        private void Deletecontainer(object sender, ContainerEventArgs e)
        {
            foreach (
                var repositoryName in
                    e.Prototype.WorkSpaceTypeWithService(Name).Select(
                        workSpaceType => e.Container.UniqueInstanceName(workSpaceType)))
            {
                FileSystem.DeleteRepository(repositoryName);

                Authorization.DeleteRepositoryReferencedGroups(repositoryName);
                Authorization.DeleteRepository(repositoryName);
            }
        }

        private void CreateContainer(object sender, ContainerEventArgs e)
        {
            try
            {
                foreach (
                    var repositoryName in
                        e.Prototype.WorkSpaceTypeWithService(Name).Select(
                            workspaceType => e.Container.UniqueInstanceName(workspaceType)))
                {
                    FileSystem.CreateRepository(repositoryName);
                    Authorization.CreateRepository(repositoryName);

                    foreach (var role in e.Prototype.Bindings.SelectMany(binding => binding.Roles))
                    {
                        var groupName = e.Container.UniqueGroupName(role.RoleTypeName);
                        Authorization.CreateGroup(groupName);

                        foreach (
                            var permission in
                                role.Rules.SelectMany(
                                    rule => rule.Permissions.Where(p => p.ServiceName == Name).Select(p => p.Name)))
                        {
                            Authorization.AddGroupToRepository(repositoryName, groupName, permission);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Log(String.Format("Service {0} -> {1} @ {2}", Name, exception.Message, DateTime.Now));
            }
        }
    }
}