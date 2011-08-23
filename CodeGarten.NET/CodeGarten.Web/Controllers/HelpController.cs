using System.Web.Mvc;

namespace CodeGarten.Web.Controllers
{
    public sealed class HelpController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

    }
}
