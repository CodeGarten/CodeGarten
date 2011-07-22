using System;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
//using CodeGarten.Service;

namespace CodeGarten.Web.Core
{
    //public class ControllerFactory : DefaultControllerFactory
    //{
    //    public ControllerFactory()
    //    {
    //    }

    //    public override IController CreateController(RequestContext requestContext, string controllerName)
    //    {
    //        var routes = requestContext.RouteData.Values;
    //        string service;
    //        if (!routes.ContainsKey("service") || (service = routes["service"].ToString()) == "")
    //            return base.CreateController(requestContext, controllerName);

    //        return ServiceFactory.CreateController(requestContext, service, controllerName);
    //    }

    //    public override void ReleaseController(IController controller)
    //    {
    //        var disposable = controller as IDisposable;
    //        if (disposable != null)
    //            disposable.Dispose();
    //    }
    //}
}