using System.Web.Mvc;

namespace CodeGarten.Web.Controllers
{
    public sealed class HomeController : Controller
    {
        public ActionResult Index()
        {
            return User.Identity.IsAuthenticated ? View("AuthenticatedHome") : View();
        }
    }
}