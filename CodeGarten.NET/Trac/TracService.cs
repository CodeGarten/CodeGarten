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
            foreach (var workSpaceType in e.Prototype.WorkSpaceTypeWithService(Name))
            {
                var envName = e.Container.UniqueInstanceName(workSpaceType);
                if (!TracEnvironmentManager.Delete(PathService, envName))
                {
                    Logger.Log(String.Format("Service {0} -> Delete instance \"{1}\" fail", Name, envName));
                    continue;
                }
            }


        }

        private void OnDisenrollUser(object sender, EnrollEventArgs e)
        {
            foreach (var workSpaceType in e.Container.Prototype.WorkSpaceTypeWithService(Name))
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
            foreach (var workSpaceType in e.Container.Prototype.WorkSpaceTypeWithService(Name))
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
            foreach (var workSpaceType in e.Prototype.WorkSpaceTypeWithService(Name))
            {
                var envName = e.Container.UniqueInstanceName(workSpaceType);
                var tracEnvironment = TracEnvironmentManager.Create(_envPath, envName);

                if (tracEnvironment == null)
                {
                    Logger.Log(String.Format("Service {0} -> Create folder \"{1}\" fail", Name, envName));
                    continue;
                }
                if (!tracEnvironment.Initialize())
                {
                    Logger.Log(String.Format("Service {0} -> Initialize instance \"{1}\" fail", Name, envName));
                    continue;
                }

                var tracPermissions = new TracPermissionManager(tracEnvironment.EnvironmentPath);
                foreach (var role in e.Prototype.Bindings.SelectMany(binding => binding.Roles))
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
