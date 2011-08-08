﻿using System.ComponentModel.DataAnnotations;
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

            var user = (ViewBag.isMe = string.IsNullOrEmpty(name))
                           ? dataBaseManager.User.Get(User.Identity.Name)
                           : dataBaseManager.User.Get(name);

             

            return
                View(new UserView() { Name = user.Name, Email = user.Email });
        }

        [HttpPost]
        public ActionResult ChangeEmail(UserView userView)
        {
            if (User.Identity.Name != userView.Name)
                return RedirectToAction("Index", new {name = userView.Name});

            if (!(ViewBag.success=ModelState.IsValidField("Email", userView)))
                return PartialView("_EditEmailForm", userView);

            var databaseManager = (DataBaseManager) HttpContext.Items["DataBaseManager"];

            databaseManager.User.ChangeEmail(userView.Name, userView.Email);
            
            return PartialView("_EditEmailForm", userView);
        }

        [HttpPost]
        public ActionResult ChangePassword(UserView userView, string currentPassword)
        {
            if(!ModelState.IsValidField("Password", userView))
                return PartialView("_EditPasswordForm", userView);

            var databaseManager = (DataBaseManager) HttpContext.Items["DataBaseManager"];

            if (!(ViewBag.success=databaseManager.Authentication.Authenticate(userView.Name, currentPassword)))
            {
                ModelState.AddModelError("currentPassword", "");
                return PartialView("_EditPasswordForm", userView);
            }

            databaseManager.User.ChangePassword(userView.Name, userView.Password);

            return PartialView("_EditPasswordForm", new UserView(){Name = userView.Name});
        }

        public ActionResult Enrolls()
        {
            var databaseManager = (DataBaseManager)HttpContext.Items["DataBaseManager"];

            return View(databaseManager.User.GetEnrolls(User.Identity.Name));
        }
    }
}