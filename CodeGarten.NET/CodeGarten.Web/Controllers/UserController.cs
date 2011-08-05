using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using CodeGarten.Data.Access;
using CodeGarten.Web.Core;
using CodeGarten.Web.Model;

namespace CodeGarten.Web.Controllers
{
    [Authorize]
    public sealed class UserController : Controller
    {

        public ActionResult Index(string name)
        {
            var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

            var user = string.IsNullOrEmpty(name)
                           ? dataBaseManager.User.Get(User.Identity.Name)
                           : dataBaseManager.User.Get(name);

            return
                View(new UserView() { Name = user.Name, Email = user.Email });
        }

        [HttpPost]
        public ActionResult ChangeEmail(UserView userView)
        {
            //TODO authorization

            if(!ModelState.IsValidField("Email", userView))
                return PartialView("_EditEmailForm", userView);

            var databaseManager = (DataBaseManager) HttpContext.Items["dataBaseManager"];

            databaseManager.User.ChangeEmail(userView.Name, userView.Email);

            return PartialView("_EditEmailForm", userView);
        }

        public ActionResult Enrolls()
        {
            var databaseManager = (DataBaseManager)HttpContext.Items["DataBaseManager"];

            return View(databaseManager.User.GetEnrolls(User.Identity.Name));
        }
    }
}