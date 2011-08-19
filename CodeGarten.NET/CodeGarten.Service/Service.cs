using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CodeGarten.Data.Access;
using CodeGarten.Data.Model;
using CodeGarten.Service.Interfaces;
using CodeGarten.Service.Utils;
using CodeGarten.Utils;


namespace CodeGarten.Service
{
    public abstract class Service
    {
        public string Name { get; private set; }
        public ServiceModel ServiceModel { get; private set; }
        
        private readonly AssemblyCatalog _assemblyCatalog;
        private readonly CompositionContainer _compositionContainer;
        private bool _isInstaled;

        public String PathService { get; private set; }
        protected Logger Logger { get; private set; }

        protected Service(ServiceModel service)
        {
            Name = service.Name;
            ServiceModel = service;
            Logger = ServiceFactory.ServiceLogger;
            PathService = Path.Combine(ServiceConfig.ServicesResourceLibLocation, Name);

            Directory.CreateDirectory(PathService);

            _assemblyCatalog = new AssemblyCatalog(GetType().Assembly);
            _compositionContainer = new CompositionContainer(_assemblyCatalog);

            _isInstaled = false;
        }

        public virtual void OnServiceInstall()
        {
            using (var databaseManager = new DataBaseManager())
            {
                databaseManager.Service.Create(ServiceModel.Name, ServiceModel.Description, ServiceModel.Permissions);

                _isInstaled = true;
            }
        }

        public virtual bool IsInstaled
        {
            get
            {
                if (_isInstaled) return true;
                using (var dataBaseManager = new DataBaseManager())
                    return (_isInstaled = dataBaseManager.Service.Get(Name) != null);
            }
        }

        public virtual void OnServiceCreating(ServiceBuilder serviceBuilder)
        {
        }

        public abstract string GetInstancePath(Container container, WorkSpaceType workSpaceType);

        #region ControllerFactoryMembers

        public ServiceController CreateController(RequestContext requestContext, string controllerName)
        {
            var lazyController = _compositionContainer.GetExports<ServiceController, IControllerMetadata>().
                Where(
                    e =>
                    e.Metadata != null &&
                    ControllerMatch(e.Metadata.ControllerName, controllerName)
                ).FirstOrDefault();

            if (lazyController != null)
            {
                lazyController.Value._service =
                    ServiceFactory.Services[(string) requestContext.RouteData.Values["service"]];
                return lazyController.Value;
            }

            throw new HttpException((int)HttpStatusCode.NotFound, HttpStatusCode.NotFound.ToString());
        }

        protected static bool ControllerMatch(String arg0, String arg1)
        {
            if (arg0 == arg1) return true;

            if (arg0.Length < arg1.Length)
                return (arg0 + "Controller") == arg1;
            return (arg1 + "Controller") == arg0;
        }

        #endregion
    }
}