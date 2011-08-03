using System.Web.Mvc;
using CodeGarten.Data.Access;
using CodeGarten.Web.Model;

namespace CodeGarten.Web.Controllers
{
    public sealed class ProfileController : Controller
    {

        public ActionResult Edit(string userName)
        {
            var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;
            var user = dataBaseManager.User.Get(userName);
            
            return View(new UserView(){Name = user.Name, Email = user.Email});
        }

        [HttpPost]
        public ActionResult Edit(UserView user)
        {
            if (!ModelState.IsValid)
                return View(user);

            return RedirectToAction("Index", "Profile");
        }

        public ActionResult Index(string name)
        {
            var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

            return
                View(string.IsNullOrEmpty(name)? dataBaseManager.User.Get(User.Identity.Name)
                         : dataBaseManager.User.Get(name));
        }
    }
}