using System.Net;
using System.Web.Mvc;

namespace CodeGarten.Web.Controllers
{
    //[Authorize]
    public sealed class ErrorController : Controller
    {
        public new ActionResult Server(string aspxerrorpath)
        {
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            //Could use routedata from previous request
            var split = aspxerrorpath.Split('/');

            if (split.Length > 1 && split[1] == "Service")
                return PartialView();

            return View();
        }

        [Authorize]
        public ActionResult Error404(string aspxerrorpath)
        {
            var split = aspxerrorpath.Split('/');

            if (split.Length > 1 && split[1] == "Service")
                return PartialView();

            TempData["Message404_Head"] = "Are you lost?";
            TempData["Message404_Body"] = "We couldn't find what you are looking for. Here's what we found instead.";

            return RedirectToAction("Index", "Search", new {search = aspxerrorpath.Replace('/',' ').Trim()});
        }

        [Authorize]
        public ActionResult Error403(string aspxerrorpath)
        {
            Response.StatusCode = (int)HttpStatusCode.Forbidden;

            var split = aspxerrorpath.Split('/');

            if (split.Length > 1 && split[1] == "Service")
                return PartialView();

            return View();
        }
    }
}
