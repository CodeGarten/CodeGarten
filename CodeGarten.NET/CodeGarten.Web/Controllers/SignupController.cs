using System.Web.Mvc;
using System.Web.Security;
using CodeGarten.Data.Access;
using CodeGarten.Web.Model;

namespace CodeGarten.Web.Controllers
{
    public sealed class SignupController : Controller
    {
        public ActionResult Index()
        {
            var user = new UserView();
            return View(user);
        }

        [HttpPost]
        public ActionResult Index(UserView user)
        {
            if (!ModelState.IsValid)
                return View(user);

            try
            {
                var dataBaseManager = (DataBaseManager)HttpContext.Items["DataBaseManager"];

                dataBaseManager.User.Create(user.Name, user.Password, user.Email);

                FormsAuthentication.SetAuthCookie(user.Name, false);

                return RedirectToAction("Index", "Login");
            }
            catch
            {
                ModelState.AddModelError("", "An error occured, please try again.");
                return View();
            }
        }
    }
}