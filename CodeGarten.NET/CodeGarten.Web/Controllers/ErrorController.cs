using System.Net;
using System.Web.Mvc;

namespace CodeGarten.Web.Controllers
{
    [Authorize]
    public sealed class ErrorController : Controller
    {
        public new ActionResult Server(string aspxerrorpath)
        {
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return View();
        }

        public ActionResult Error404(string aspxerrorpath)
        {
            //Response.StatusCode = (int)HttpStatusCode.NotFound;
            //return View();

            TempData["Message404_Head"] = "Are you lost?";
            TempData["Message404_Body"] = "We couldn't find what you are looking for. Here's what we found instead.";
            return RedirectToAction("Index", "Search", new {search = aspxerrorpath.Replace('/',' ').Trim()});
        }

        public ActionResult Error403(string aspxerrorpath)
        {
            Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return View();
        }
    }
}
