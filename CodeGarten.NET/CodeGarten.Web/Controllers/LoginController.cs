using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CodeGarten.Data.Access;
using CodeGarten.Data.ModelView;

namespace CodeGarten.Web.Controllers
{
    public sealed class LoginController : Controller
    {
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "User");

            var user = new UserView();

            return View(user);
        }

        [HttpPost]
        public ActionResult Index(string returnUrl, UserView user)
        {
            try
            {
                var dbManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

                if (!dbManager.Authentication.Authenticate(user.Name, user.Password))
                {
                    ModelState.AddModelError("", "Incorrect username/password.");
                    return View();
                }

                FormsAuthentication.SetAuthCookie(user.Name,false);

                if (returnUrl != null)
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "User");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }
    }
}