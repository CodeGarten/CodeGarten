using System.Configuration;
using System.IO;
using System.Security.Principal;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using CodeGarten.Data.Access;
using CodeGarten.Service;
using CodeGarten.Utils;
using CodeGarten.Web.Controllers;
using CodeGarten.Web.Core;

namespace CodeGarten.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly Logger Logger =
            new Logger(File.Exists(ConfigurationManager.AppSettings["Log"])
                           ? File.AppendText(ConfigurationManager.AppSettings["Log"])
                           : File.CreateText(ConfigurationManager.AppSettings["Log"]));

        //public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        //{
        //    filters.Add(new HandleErrorAttribute());
        //}

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            //routes.IgnoreRoute("{*favicon}", new {favicon = @"(.*/)?favicon.ico(/.*)?"});

            routes.MapRoute
                (
                    "ServiceEntryPoint",
                    "Service/{service}/{structureId}/{containerId}/{workspaceTypeName}",
                    new {controller = "Home", action = "Index"}
                );

            routes.MapRoute
                (
                    "Service",
                    "Service/{service}/{controller}/{action}",
                    new {service = "", controller = "Home", action = "Index"}
                );

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new {controller = "Home", action = "Index", id = UrlParameter.Optional} // Parameter defaults
                );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            //RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            ControllerBuilder.Current.SetControllerFactory(new ControllerFactory());

            HostingEnvironment.RegisterVirtualPathProvider(new ServiceVirtualPath());

            ServiceFactory.LoadServices();

            Logger.Start();
            Logger.Log("Web application started.");
        }

        protected void Application_End()
        {
            Logger.Log("Web application stopped.");
            Logger.Stop();
        }

        public void Application_AuthenticateRequest()
        {
            var authCookie = Request.Cookies.Get(FormsAuthentication.FormsCookieName);

            if (authCookie == null)
                return;

            var ticket = FormsAuthentication.Decrypt(authCookie.Value);

            Context.User = new GenericPrincipal(new GenericIdentity(ticket.Name), null);
        }

        public void Application_BeginRequest()
        {
            Context.Items["DataBaseManager"] = new DataBaseManager();
        }

        public void Application_EndRequest()
        {
            var dataBaseManager = Context.Items["DataBaseManager"] as DataBaseManager;

            if (dataBaseManager != null)
                dataBaseManager.Dispose();
        }

        protected void Application_Error()
        {
            var exception = Server.GetLastError();
            Logger.Log(exception.Message);
        }
    }
}