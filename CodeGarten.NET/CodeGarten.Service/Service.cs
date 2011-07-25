using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using CodeGarten.Data.Access;
using CodeGarten.Data.ModelView;
using CodeGarten.Service.Utils;


namespace CodeGarten.Service
{
    public abstract class Service
    {
        public string Name { get; private set; }
        public ServiceModel ServiceModel { get; private set; }

        private readonly AssemblyCatalog _assemblyCatalog;
        private readonly CompositionContainer _compositionContainer;
        private bool _isInstaled;

        protected String PathService { get; private set; }

        protected Service(ServiceModel service)
        {
            Name = service.Name;
            ServiceModel = service;
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

        #region ControllerFactoryMembers

        public IController CreateController(RequestContext requestContext, string controllerName)
            //TODO TESTE
            //public IController CreateController(string controllerName)
        {
            var lazyController = _compositionContainer.GetExports<IController, IControllerMetadata>().
                Where(
                    e =>
                    e.Metadata != null &&
                    ControllerMatch(e.Metadata.ControllerName, controllerName)
                ).FirstOrDefault();

            return lazyController == null ? null : lazyController.Value;
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