using System;
using System.Web;
using System.Web.Mvc;
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
                    return View();

                var cookie = new HttpCookie("authenticated");
                cookie["name"] = user.Name;

                HttpContext.Response.Cookies.Add(cookie);

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
            var authenticatedCookie = HttpContext.Request.Cookies["authenticated"] ?? new HttpCookie("authenticated");

            authenticatedCookie.Expires = DateTime.Now.Subtract(new TimeSpan(1, 0, 0));

            HttpContext.Response.Cookies.Add(authenticatedCookie);

            return RedirectToAction("Index", "Home");
        }
    }
}