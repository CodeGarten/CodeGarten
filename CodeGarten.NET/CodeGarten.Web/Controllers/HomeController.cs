using System.Web.Mvc;

namespace CodeGarten.Web.Controllers
{
    public sealed class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Structured collaborative development";
            return User.Identity.IsAuthenticated ? View("AuthenticatedHome") : View();
        }
    }
}