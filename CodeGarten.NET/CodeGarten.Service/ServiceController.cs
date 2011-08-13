using System;
using System.Web.Mvc;

namespace CodeGarten.Service
{
    public class ServiceController : Controller
    {
        protected internal Service Service { get; set; }

        protected override PartialViewResult PartialView(string viewName, object model)
        {
            return base.PartialView(String.Format("~/Services/{0}.dll/{0}/Views/{1}/{2}.cshtml", Url.RequestContext.RouteData.Values["service"], Url.RequestContext.RouteData.Values["controller"], viewName ?? Url.RequestContext.RouteData.Values["action"]), model);
        }

        protected override ViewResult View(string viewName, string masterName, object model)
        {
            return
                View(
                    String.Format("~/Services/{0}.dll/{0}/Views/{1}/{2}.cshtml",
                                  Url.RequestContext.RouteData.Values["service"],
                                  masterName ?? Url.RequestContext.RouteData.Values["controller"],
                                  viewName ?? Url.RequestContext.RouteData.Values["action"]), model);
        }
    }
}
