using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using CodeGarten.Data.Access;
using CodeGarten.Service;
using CodeGarten.Service.Utils;

namespace Trac
{
    [Export(typeof(Service))]
    public class Trac : Service
    {
        private readonly string _envPath;

        public Trac() : base(new ServiceModel("Trac", "", EnumExtensions.ToEnumerable<TracPrivileges>()))
        {
            _envPath = Path.Combine(PathService, "envs");
        }

        public override void OnServiceCreating(ServiceBuilder serviceBuilder)
        {
            serviceBuilder.OnCreateContainer += OnCreateContainer;
            serviceBuilder.OnEnrollUser += OnEnrollUser;
            serviceBuilder.OnDisenrollUser += OnDisenrollUser;
            serviceBuilder.OnDeleteContainer += OnDeleteContainer;
        }

        private void OnDeleteContainer(object sender, ContainerEventArgs e)
        {
            foreach (var workSpaceType in e.Container.WorkSpaceTypeWithService(Name))
                if (!TracEnvironmentManager.Delete(PathService, e.Container.UniqueInstanceName(workSpaceType)))
                    continue; //TODO service logger
            
        }

        private void OnDisenrollUser(object sender, EnrollEventArgs e)
        {
            foreach (var workSpaceType in e.Container.WorkSpaceTypeWithService(Name))
            {
                var tracPermissions =
                    new TracPermissionManager(TracEnvironmentManager.FormatEnvironmentPath(_envPath,
                                                                                           e.Enroll.Container.
                                                                                               UniqueInstanceName(
                                                                                                   workSpaceType)));
                tracPermissions.RemoveGroupUser(e.Enroll.UserName,
                                             e.Enroll.Container.UniqueGroupName(e.Enroll.RoleTypeName));
            }

        }

        private void OnEnrollUser(object sender, EnrollEventArgs e)
        {
            foreach (var workSpaceType in e.Container.WorkSpaceTypeWithService(Name))
            {
                var tracPermissions =
                    new TracPermissionManager(TracEnvironmentManager.FormatEnvironmentPath(_envPath,
                                                                                           e.Enroll.Container.
                                                                                               UniqueInstanceName(
                                                                                                   workSpaceType)));
                tracPermissions.AddGroupUser(e.Enroll.UserName,
                                             e.Enroll.Container.UniqueGroupName(e.Enroll.RoleTypeName));
            }
        }

        private void OnCreateContainer(object sender, ContainerEventArgs e)
        {
            foreach (var workSpaceType in e.Container.WorkSpaceTypeWithService(Name))
            {
                
                var tracEnvironment = TracEnvironmentManager.Create(_envPath,
                                                                    e.Container.UniqueInstanceName(workSpaceType));
                if(tracEnvironment==null)
                    continue;//TODO service logger
                if(!tracEnvironment.Initialize())
                    continue;//TODO service logger

                var tracPermissions = new TracPermissionManager(tracEnvironment.EnvironmentPath);
                foreach (var role in e.Container.ContainerPrototype.Roles)
                {
                    var groupName = e.Container.UniqueGroupName(role.RoleTypeName);
                    foreach (var rule in role.Rules)
                    {
                        var permissions = rule.Permissions.Where(p => p.ServiceName == Name).Select(p => p.Name);
                        foreach (var permission in permissions)
                            tracPermissions.Add(groupName, permission);
                    }
                }
            }
            
        }
    }
}
