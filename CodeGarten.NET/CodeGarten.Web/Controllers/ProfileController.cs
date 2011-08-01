using System.Web.Mvc;
using CodeGarten.Data.Access;

namespace CodeGarten.Web.Controllers
{
    public sealed class ProfileController : Controller
    {
        public ActionResult Index(string name)
        {
            var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

            return
                View(string.IsNullOrEmpty(name)
                         ? dataBaseManager.User.Get(User.Identity.Name)
                         : dataBaseManager.User.Get(name));
        }
    }
}