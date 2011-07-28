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
    public static class ServiceFactory
    {
        //TODO develop auto refreash
        private static readonly DirectoryCatalog DirectoryCatalog;
        private static readonly CompositionContainer CompositionContainer;

        private static readonly ServiceBuilder Builder;

        private static readonly Dictionary<string, Service> Services;

        static ServiceFactory()
        {
            Builder = new ServiceBuilder();
            Services = new Dictionary<string, Service>();
            DirectoryCatalog = new DirectoryCatalog(ServiceConfig.ServicesDllLocation);
            CompositionContainer = new CompositionContainer(DirectoryCatalog);
        }

        public static void LoadServices()
        {
            var serviceEngine = new ServiceEngine();

            CompositionContainer.ComposeParts(serviceEngine);

            foreach (var service in serviceEngine.Services)
            {
                if (!service.IsInstaled)
                    service.OnServiceInstall();
                
                service.OnServiceCreating(Builder);
                Services.Add(service.Name, service);
            }
        }

        public static bool Install(string name)
        {
            if (!Services.ContainsKey(name)) return false;

            var service = Services[name];
            if (service.IsInstaled) return false;

            service.OnServiceInstall();
            service.OnServiceCreating(Builder);
            return true;
        }

        public static bool InstallAll()
        {
            foreach (var avaibleService in Services)
            {
                var service = avaibleService.Value;
                if (!service.IsInstaled)
                {
                    service.OnServiceInstall();
                    service.OnServiceCreating(Builder);
                }
            }
            return true;
        }

        public static IEnumerable<string> AvaibleServices()
        {
            foreach (var service in Services)
                if (service.Value.IsInstaled)
                    yield return service.Key;
            yield break;
        }

        public static IEnumerable<string> NotInstalledServices()
        {
            foreach (var service in Services)
                if (!service.Value.IsInstaled)
                    yield return service.Key;
            yield break;
        }

        public static IController CreateController(RequestContext requestContext, string serviceName,
                                                   string controllerName)
        {
            if (Services.ContainsKey(serviceName))
                return Services[serviceName].CreateController(requestContext, controllerName);
            //return Services[serviceName].CreateController(controllerName);
            return null;
        }

        #region Class ServiceEngine

        [Export]
        private class ServiceEngine
        {
            [ImportMany] public IEnumerable<Service> Services;
        }

        #endregion
    }
}