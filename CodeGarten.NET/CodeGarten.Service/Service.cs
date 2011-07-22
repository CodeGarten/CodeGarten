using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;


namespace CodeGarten.Service
{
    public abstract class Service
    {
        public string Name { get; private set; }

        private readonly AssemblyCatalog _assemblyCatalog;
        private readonly CompositionContainer _compositionContainer;

        protected String PathService { get; private set; }

        protected Service(String serviceName)
        {
            Name = serviceName;
            PathService = Path.Combine(ServiceConfig.ServicesResourceLibLocation, serviceName);

            Directory.CreateDirectory(PathService);

            _assemblyCatalog = new AssemblyCatalog(GetType().Assembly);
            _compositionContainer = new CompositionContainer(_assemblyCatalog);
        }

        public abstract void OnServiceInstall();

        public abstract bool IsInstaled { get; }

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