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
        private bool _isInstaled;
        private readonly string _filesPath;
        private readonly string _authFileName;

        public SvnService() : base("Svn")
        {
            // put this in app.config file
            _filesPath = Path.Combine(PathService, "etc");
            Directory.CreateDirectory(_filesPath);
            _authFileName = "auth_file";

            _isInstaled = false;
        }

        #region  Service Installer

        public override void OnServiceInstall()
        {
            //TODO service api
            using (var databaseManager = new DataBaseManager())
            {
                databaseManager.Service.Create(
                    new ServiceView()
                        {
                            Name = Name
                        },
                    new string[]
                        {
                            SVNPrivileges.r.ToString(),
                            SVNPrivileges.rw.ToString()
                        }
                    );

                _isInstaled = true;
            }
        }

        public override bool IsInstaled
        {
            get
            {
                //TODO service api
                if (_isInstaled) return true;
                using (var dataBaseManager = new DataBaseManager())
                    return dataBaseManager.Service.Get(Name) != null;
            }
        }

        #endregion

        #region Service On Creating

        public override void OnServiceCreating(ServiceBuilder serviceBuilder)
        {
            serviceBuilder.OnCreateContainer += OnCreateContainer;
            serviceBuilder.OnEnrollUser += OnUserEnroll;
        }

        #endregion

        #region ServiceBuilder Methods

        // Event args need to be change
        // make timespan to rewrite the file
        private void OnUserEnroll(object sender, EventArgs e)
        {
            using (var dataBaseManager = new DataBaseManager())
            {
                var filePath = Path.Combine(_filesPath, String.Format("~{0}.tmp", _authFileName));

                var svnAuthorization = new SVNAuthorization(filePath);

                dataBaseManager.Authorization.CreateServiceAuthorizationStruct(svnAuthorization, Name);

                OverrideFile(filePath, Path.Combine(_filesPath, _authFileName));
            }
        }

        //Can be a extension method of File
        public static void OverrideFile(string sourceFileName, string destFileName)
        {
            if (File.Exists(destFileName))
            {
                do
                {
                    try
                    {
                        File.Delete(destFileName);
                        break;
                    }
                    catch (IOException e) // IF the file is in use
                    {
                    }
                } while (true);
            }

            File.Move(sourceFileName, destFileName);
        }

        // need to create a logger
        private void OnCreateContainer(object sender, ContainerEventArgs e)
        {
            //if (!e.Services.ContainsKey(Name)) return;
            
            //var workspacesType = e.Services[Name];

            //foreach (var workSpaceTypeView in workspacesType)
            //{
            //    var pathRepo = Path.Combine(PathService, "repositories");
            //    //TODO AuthorizationManager Create the name
            //    var repoName = String.Format("{0}{1}{2}", e.Strucuture, e.Container.Id, workSpaceTypeView.Name);

            //    var repository = SVNRepositoryManager.Create(pathRepo, repoName);
            //    if (repository == null)
            //        continue; //Servicelogger;
            //    if (!repository.Initialize())
            //        continue; //Servicelogger;
            //}
            
            foreach (var workSpaceType in e.Container.ContainerPrototype.WorkSpaceTypes)
            {
                //TODO organizar isto
                var pathRepo = Path.Combine(PathService, "repositories");
                
                var instanceName = e.Container.UniqueInstanceName(workSpaceType);

                var repository = SVNRepositoryManager.Create(pathRepo, instanceName);
                if (repository == null)
                    continue; //TODO Servicelogger;
                if (!repository.Initialize())
                    continue; //TODO Servicelogger;

                foreach (var role in workSpaceType.Roles.Where(r => r.ContainerPrototypeName == e.Container.ContainerPrototype.Name))
                {
                    var filePath = Path.Combine(_filesPath, String.Format("~{0}.tmp", _authFileName));

                    var svnAuthorization = new SVNAuthorization(filePath);

                    
                }
            }
        }

        #endregion
    }
}