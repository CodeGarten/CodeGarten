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
            Authorization = new Authorization(Path.Combine(PathService, @"etc\"));
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
            foreach (var workspace in e.Container.Prototype.WorkSpaceTypeWithService(Name))
                Authorization.RemoveUser(e.Enroll.UserName, e.Container.UniqueInstanceName(workspace));
        }

        private void EnrollUser(object sender, EnrollEventArgs e)
        {
            foreach (var binding in e.Container.Prototype.Bindings.Where(b => e.Container.Prototype.WorkSpaceTypeWithService(Name).Select(w => w.Name).Contains(b.WorkSpaceTypeName)))
                foreach (var permission in binding.Roles.SelectMany(r => r.Rules).SelectMany(r => r.Permissions).Where(p => p.Service.Name == Name))
                    Authorization.AddUser(e.Enroll.UserName, e.Container.UniqueInstanceName(binding.WorkSpaceType), (Privileges)Enum.Parse(typeof(Privileges), permission.Name));
        }

        private void Deletecontainer(object sender, ContainerEventArgs e)
        {
            foreach (var repositoryName in e.Prototype.WorkSpaceTypeWithService(Name).Select(workSpaceType => e.Container.UniqueInstanceName(workSpaceType)))
            {
                FileSystem.DeleteRepository(repositoryName);
                Authorization.DeleteRepository(repositoryName);
            }
        }

        private void CreateContainer(object sender, ContainerEventArgs e)
        {
            foreach (var repositoryName in e.Prototype.WorkSpaceTypeWithService(Name).Select(workspaceType => e.Container.UniqueInstanceName(workspaceType)))
            {
                FileSystem.CreateRepository(repositoryName);
                Authorization.CreateRepository(repositoryName);
            }
        }
    }
}