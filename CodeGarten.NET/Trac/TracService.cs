﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using CodeGarten.Data.Access;
using CodeGarten.Data.Model;
using CodeGarten.Service;
using CodeGarten.Service.Utils;
using Trac.Core;
using Service = CodeGarten.Service.Service;

namespace Trac
{
    [Export(typeof(Service))]
    public class Trac : Service
    {
        private readonly string _envPath;

        public Trac() : base(new ServiceModel("Trac", "Integrated SCM & Project Management", EnumExtensions.ToEnumerable<TracPrivileges>()))
        {
            _envPath = Path.Combine(PathService, "envs");
        }

        public override string GetInstancePath(Container container, WorkSpaceType workSpaceType)
        {
            return Path.Combine(_envPath, container.UniqueInstanceName(workSpaceType));
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
            foreach (var workSpaceType in e.Type.WorkSpaceTypeWithService(Name))
            {
                var envName = e.Container.UniqueInstanceName(workSpaceType);
                if (!TracEnvironmentManager.Delete(_envPath, envName))
                {
                    Logger.Log(String.Format("Service {0} -> Delete instance \"{1}\" fail", Name, envName));
                    continue;
                }
            }


        }

        private void OnDisenrollUser(object sender, EnrollEventArgs e)
        {

            foreach (var workSpaceType in e.Container.Type.WorkSpaceTypeWithService(Name))
            {
                var tracPermissions =
                    new TracPermissionManager(TracEnvironmentManager.FormatEnvironmentPath(_envPath,
                                                                                           e.Container.
                                                                                               UniqueInstanceName(
                                                                                                   workSpaceType)));
                tracPermissions.RemoveGroupUser(e.Enroll.UserName,
                                             e.Container.UniqueGroupName(e.Enroll.RoleTypeName));
            }

        }

        private void OnEnrollUser(object sender, EnrollEventArgs e)
        {
            foreach (var workSpaceType in e.Container.Type.WorkSpaceTypeWithService(Name))
            {
                var tracPermissions =
                    new TracPermissionManager(TracEnvironmentManager.FormatEnvironmentPath(_envPath,
                                                                                           e.Container.
                                                                                               UniqueInstanceName(
                                                                                                   workSpaceType)));
                tracPermissions.AddGroupUser(e.Enroll.UserName,
                                             e.Container.UniqueGroupName(e.Enroll.RoleTypeName));
            }
        }

        private void OnCreateContainer(object sender, ContainerEventArgs e)
        {
            foreach (var workSpaceType in e.Type.WorkSpaceTypeWithService(Name))
            {
                var contextType = workSpaceType;
                var envName = e.Container.UniqueInstanceName(workSpaceType);
                var tracEnvironment = TracEnvironmentManager.Create(_envPath, envName);

                if (tracEnvironment == null)
                {
                    Logger.Log(String.Format("Service {0} -> Create folder \"{1}\" fail", Name, envName));
                    continue;
                }
                
                if (!tracEnvironment.Initialize(workSpaceType.Services.Where(s => s.Name != Name).Select(
                    s => new KeyValuePair<string, string>(s.Name, 
                        ServiceFactory.InstancePath(s.Name, e.Container, contextType))
                        )))
                {
                    Logger.Log(String.Format("Service {0} -> Initialize instance \"{1}\" fail", Name, envName));
                    continue;
                }

                var tracPermissions = new TracPermissionManager(tracEnvironment.EnvironmentPath);
                tracPermissions.RemoveAll();
                foreach (var role in e.Type.Bindings.SelectMany(binding => binding.Roles))
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
