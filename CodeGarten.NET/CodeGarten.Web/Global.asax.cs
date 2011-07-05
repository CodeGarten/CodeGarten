using System.Security.Principal;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using CodeGarten.Data.Access;
using CodeGarten.Service;
using CodeGarten.Web.Core;

namespace CodeGarten.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new {favicon = @"(.*/)?favicon.ico(/.*)?"});

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

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            ControllerBuilder.Current.SetControllerFactory(new ControllerFactory());

            HostingEnvironment.RegisterVirtualPathProvider(new ServiceVirtualPath());

            ServiceFactory.LoadServices();
            ServiceFactory.InstallAll();
        }

        public void Application_AuthenticateRequest()
        {
            var authCookie = Request.Cookies["authenticated"];

            if (authCookie != null)
                Context.User = new GenericPrincipal(new GenericIdentity(authCookie["name"]), null);
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
    }
}