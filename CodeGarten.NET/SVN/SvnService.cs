using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using CodeGarten.Data.Access;
using CodeGarten.Data.Interfaces;
using CodeGarten.Service;
using CodeGarten.Service.Utils;


namespace SVN
{
    [Export(typeof(Service))]
    public class SvnService : Service
    {
        private readonly string _filesPath, _authFileName, _repoPath;
        private readonly SVNAuthorization _svnAuthorization;
        //TODO Implemente dispose

        public SvnService()
            : base(new ServiceModel("Svn", "System Version Control", EnumExtensions.ToEnumerable<SVNPrivileges>()))
        {
            _filesPath = Path.Combine(PathService, "etc");
            Directory.CreateDirectory(_filesPath);
            _authFileName = "auth_file";
            _repoPath = Path.Combine(PathService, "repositories");
            
            _svnAuthorization = new SVNAuthorization(_filesPath, _authFileName);
        }

        #region Service On Creating

        public override void OnServiceCreating(ServiceBuilder serviceBuilder)
        {
            serviceBuilder.OnCreateContainer += OnCreateContainer;
            serviceBuilder.OnEnrollUser += OnUserEnroll;
            serviceBuilder.OnDisenrollUser += OnDisenrollUser;
            serviceBuilder.OnDeleteContainer += OnDeleteContainer;
        }

        #endregion

        #region ServiceBuilder Methods

        private void OnDeleteContainer(object sender, ContainerEventArgs e)
        {
            foreach (var workSpaceType in e.Container.WorkSpaceTypeWithService(Name))
            {
                var instanceName = e.Container.UniqueInstanceName(workSpaceType);
                SVNRepositoryManager.Delete(_repoPath, instanceName);
                _svnAuthorization.RemoveInstance(instanceName);
            }
        }

        private void OnDisenrollUser(object sender, EnrollEventArgs e)
        {
            var group = _svnAuthorization.GetGroup(e.Container.UniqueGroupName(e.Enroll.RoleTypeName));
            group.RemoveUser(e.Enroll.UserName);
        }

        private void OnUserEnroll(object sender, EnrollEventArgs e)
        {
            var group = _svnAuthorization.CreateOrGetGroup(e.Container.UniqueGroupName(e.Enroll.RoleTypeName));
            group.AddUser(e.Enroll.UserName);
        }

        private void OnCreateContainer(object sender, ContainerEventArgs e)
        {
            foreach (var workSpaceType in e.Container.Prototype.Bindings.Select(b => b.WorkSpaceType))
            {
                var instanceName = e.Container.UniqueInstanceName(workSpaceType);

                var repository = SVNRepositoryManager.Create(_repoPath, instanceName);
                if (repository == null)
                    continue; //TODO Servicelogger;
                if (!repository.Initialize())
                    continue; //TODO Servicelogger;

                var instance = _svnAuthorization.CreateOrGetInstance(instanceName);
                foreach (var role in e.Container.Prototype.Bindings.SelectMany(binding => binding.Roles))
                {
                    var groupName = e.Container.UniqueGroupName(role.RoleTypeName);
                    foreach (var rule in role.Rules)
                    {
                        var permissions = rule.Permissions.Where(p => p.ServiceName == Name).Select(p => p.Name);
                        foreach (var permission in permissions)
                            instance.AddGroupPermission(groupName, permission);
                    }
                }

            }
            
        }

        #endregion
    }
}