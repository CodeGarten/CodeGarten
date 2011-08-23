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
using CodeGarten.Utils;

namespace CodeGarten.Service
{
    public static class ServiceFactory
    {
        private static readonly DirectoryCatalog DirectoryCatalog;
        private static readonly CompositionContainer CompositionContainer;

        private static readonly ServiceBuilder Builder;

        internal static readonly Dictionary<string, Service> Services;

        public static  Logger ServiceLogger;

        static ServiceFactory()
        {
            var loggerPath = Path.Combine(ServiceConfig.ServicesResourceLibLocation, "ServiceLogger.log");
            var fileLogger = File.Exists(loggerPath) ? File.AppendText(loggerPath) : File.CreateText(loggerPath);
            ServiceLogger = new Logger(fileLogger);
            Builder = new ServiceBuilder();
            Services = new Dictionary<string, Service>();
            DirectoryCatalog = new DirectoryCatalog(ServiceConfig.ServicesDllLocation);
            CompositionContainer = new CompositionContainer(DirectoryCatalog);
            ServiceLogger.Start();
        }

        public static void RegisteServer(IServer server)
        {
            UserManager.OnCreateUser += (sender, user) => server.CreateUser(user.User.Name, user.PasswordPlainText);
            UserManager.OnRemoveUser += (sender, user) => server.DeleteUser(user.User.Name);
            UserManager.OnUserChangePassword +=
                (sender, user) => server.ChangePassword(user.User.Name, user.PasswordPlainText);
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

            Builder.RegisterEvents();
        }

        public static string InstancePath(string service, Container container, WorkSpaceType workSpaceType)
        {
            try
            {
                return Services.ContainsKey(service) ? Services[service].GetInstancePath(container, workSpaceType) : null;   
 
            }catch(NotImplementedException)
            {
                return null;
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

            throw new HttpException((int)HttpStatusCode.NotFound, HttpStatusCode.NotFound.ToString());
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