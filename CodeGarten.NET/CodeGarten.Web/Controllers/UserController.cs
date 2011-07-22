using System.Web.Mvc;
using CodeGarten.Data.Access;

namespace CodeGarten.Web.Controllers
{
    [Authorize]
    public sealed class UserController : Controller
    {

        //public ActionResult Index()
        //{
        //    var user = dataBaseManager.User.Get(User.Identity.Name);

        //    return View(user);
        //}
    }
}