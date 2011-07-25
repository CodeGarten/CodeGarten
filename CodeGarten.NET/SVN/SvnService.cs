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
    [Export(typeof (Service))]
    public class SvnService : Service
    {
        private readonly string _filesPath;
        private readonly string _authFileName;
        
        public SvnService()
            : base(new ServiceModel("Svn", "System Version Control", new []{SVNPrivileges.r.ToString(),SVNPrivileges.rw.ToString()}))
        {
            // put this in app.config file
            _filesPath = Path.Combine(PathService, "etc");
            Directory.CreateDirectory(_filesPath);
            _authFileName = "auth_file";
        }

        #region  Service Installer

        //public override void OnServiceInstall()
        //{
        //    //TODO service api
        //    using (var databaseManager = new DataBaseManager())
        //    {
        //        databaseManager.Service.Create(
        //            new ServiceView()
        //                {
        //                    Name = Name
        //                },
        //            new string[]
        //                {
        //                    SVNPrivileges.r.ToString(),
        //                    SVNPrivileges.rw.ToString()
        //                }
        //            );

        //        _isInstaled = true;
        //    }
        //}

        //public override bool IsInstaled
        //{
        //    get
        //    {
        //        //TODO service api
        //        if (_isInstaled) return true;
        //        using (var dataBaseManager = new DataBaseManager())
        //            return dataBaseManager.Service.Get(Name) != null;
        //    }
        //}

        #endregion

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
            //using (var dataBaseManager = new DataBaseManager())
            //{
            //    var filePath = Path.Combine(_filesPath, String.Format("~{0}.tmp", _authFileName));

            //    var svnAuthorization = new SVNAuthorization(filePath);

            //    dataBaseManager.Authorization.CreateServiceAuthorizationStruct(svnAuthorization, Name);

            //    OverrideFile(filePath, Path.Combine(_filesPath, _authFileName));
            //}

            using (var svnAuthorization = new SVNAuthorization(_filesPath, _authFileName))
            {
                //foreach (var VARIABLE in e.Enroll)
                //{
                    
                //}
                //var groupName = e.  .UniqueGroupName(role);
                //svnAuthorization.CreateGroup();
            }
        }

        
        //public static void OverrideFile(string sourceFileName, string destFileName)
        //{
        //    if (File.Exists(destFileName))
        //    {
        //        do
        //        {
        //            try
        //            {
        //                File.Delete(destFileName);
        //                break;
        //            }
        //            catch (IOException e) // IF the file is in use
        //            {
        //            }
        //        } while (true);
        //    }

        //    File.Move(sourceFileName, destFileName);
        //}

        private void OnCreateContainer(object sender, ContainerEventArgs e)
        {
            
            //TODO organizar isto
            var pathRepo = Path.Combine(PathService, "repositories");

            using (var svnAuthorization = new SVNAuthorization(_filesPath, _authFileName))
            {
                foreach (var workSpaceType in e.Container.ContainerPrototype.WorkSpaceTypes)
                {
                    var instanceName = e.Container.UniqueInstanceName(workSpaceType);

                    var repository = SVNRepositoryManager.Create(pathRepo, instanceName);
                    if (repository == null)
                        continue; //TODO Servicelogger;
                    if (!repository.Initialize())
                        continue; //TODO Servicelogger;

                    var instance = svnAuthorization.CreateInstance(instanceName);
                    foreach (var role in e.Container.ContainerPrototype.Roles)
                    {
                        var groupName = e.Container.UniqueGroupName(role);
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