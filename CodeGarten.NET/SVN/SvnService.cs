using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using CodeGarten.Data.Access;
using CodeGarten.Data.Interfaces;
using CodeGarten.Data.ModelView;
using CodeGarten.Service;
using CodeGarten.Service.Utils;


namespace SVN
{
    [Export(typeof(Service))]
    public class SvnService : Service
    {
        private readonly string _filesPath, _authFileName, _repoPath;
        

        public SvnService()
            : base(new ServiceModel("Svn", "System Version Control", EnumExtensions.ToEnumerable<SVNPrivileges>()))
        {
            //TODO put this in app.config file ---> base path can be static
            _filesPath = Path.Combine(PathService, "etc");
            Directory.CreateDirectory(_filesPath);
            _authFileName = "auth_file";
            _repoPath = Path.Combine(PathService, "repositories");
        }

        #region Service On Creating

        public override void OnServiceCreating(ServiceBuilder serviceBuilder)
        {
            serviceBuilder.OnCreateContainer += OnCreateContainer;
            serviceBuilder.OnEnrollUser += OnUserEnroll;
        }

        #endregion

        #region ServiceBuilder Methods

        private void OnUserEnroll(object sender, EnrollEventArgs e)
        {
            using (var svnAuthorization = new SVNAuthorization(_filesPath, _authFileName))
            {
                //foreach (var VARIABLE in e.Enroll)
                //{

                //}
                //var groupName = e.  .UniqueGroupName(role);
                //svnAuthorization.CreateGroup();
            }
        }

        private void OnCreateContainer(object sender, ContainerEventArgs e)
        {
            using (var svnAuthorization = new SVNAuthorization(_filesPath, _authFileName))
            {
                foreach (var workSpaceType in e.Container.ContainerPrototype.WorkSpaceTypes)
                {
                    var instanceName = e.Container.UniqueInstanceName(workSpaceType);

                    var repository = SVNRepositoryManager.Create(_repoPath, instanceName);
                    if (repository == null)
                        continue; //TODO Servicelogger;
                    if (!repository.Initialize())
                        continue; //TODO Servicelogger;

                    var instance = svnAuthorization.CreateInstance(instanceName);
                    foreach (var role in e.Container.ContainerPrototype.Roles)
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
        }

        #endregion
    }
}