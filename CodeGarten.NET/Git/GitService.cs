using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using CodeGarten.Data.Access;
using CodeGarten.Service;
using CodeGarten.Service.Utils;
using Git.Core;

namespace Git
{
    [Export(typeof (Service))]
    public sealed class Git : Service
    {
        private readonly Authorization _authorization;
        private readonly FileSystem _fileSystem;

        public Git()
            : base(
                new ServiceModel("Git", "Distributed Version Control System", EnumExtensions.ToEnumerable<Privileges>())
                )
        {
            _fileSystem = new FileSystem(Path.Combine(PathService, "repositories"));

            Directory.CreateDirectory(Path.Combine(PathService, "etc"));
            _authorization = new Authorization(Path.Combine(PathService, "etc", "autho_file.xml"));
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
            _authorization.RemoveUserFromGroup(e.Container.UniqueGroupName(e.Enroll.RoleTypeName), e.Enroll.UserName);
        }

        private void EnrollUser(object sender, EnrollEventArgs e)
        {
            _authorization.AddUserToGroup(e.Container.UniqueGroupName(e.Enroll.RoleTypeName), e.Enroll.UserName);
        }

        private void Deletecontainer(object sender, ContainerEventArgs e)
        {
            foreach (
                var repositoryName in
                    e.Prototype.WorkSpaceTypeWithService(Name).Select(
                        workSpaceType => e.Container.UniqueInstanceName(workSpaceType)))
            {
                _fileSystem.DeleteRepository(repositoryName);

                _authorization.DeleteRepositoryReferencedGroups(repositoryName);
                _authorization.DeleteRepository(repositoryName);
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
                    _fileSystem.CreateRepository(repositoryName);
                    _authorization.CreateRepository(repositoryName);

                    foreach (var role in e.Prototype.Bindings.SelectMany(binding => binding.Roles))
                    {
                        var groupName = e.Container.UniqueGroupName(role.RoleTypeName);
                        _authorization.CreateGroup(groupName);

                        foreach (
                            var permission in
                                role.Rules.SelectMany(
                                    rule => rule.Permissions.Where(p => p.ServiceName == Name).Select(p => p.Name)))
                        {
                            _authorization.AddGroupToRepository(repositoryName, groupName, permission);
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