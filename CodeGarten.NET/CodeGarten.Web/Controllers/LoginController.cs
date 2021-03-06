﻿using System.Net;
using System.Web.Mvc;
using System.Web.Security;
using CodeGarten.Data.Access;
using CodeGarten.Web.Model;

namespace CodeGarten.Web.Controllers
{
    public sealed class LoginController : Controller
    {
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            var user = new UserView();

            return View(user);
        }

        [HttpPost]
        public ActionResult Index(string returnUrl, UserView user)
        {
            try
            {
                var dataBaseManager = (DataBaseManager)HttpContext.Items["DataBaseManager"];

                var dbUser = dataBaseManager.User.Get(user.Name);

                if (dbUser == null)
                {
                    ModelState.AddModelError("Name", "This user does not exist.");
                    Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return View(user);
                }

                if (!dataBaseManager.Authentication.Authenticate(user.Name, user.Password))
                {
                    ModelState.AddModelError("Password", "Incorrect password.");
                    Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return View(user);
                }

                FormsAuthentication.SetAuthCookie(user.Name, false);

                if (returnUrl != null)
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }
            catch
            {
                ModelState.AddModelError("", "An error occured, please try again.");
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
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