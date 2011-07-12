using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CodeGarten.Data.Access;
using CodeGarten.Data.ModelView;
using Apache;

namespace CodeGarten.Web.Controllers
{
    public sealed class SignupController : Controller
    {
        //
        // GET: /Signup/

        public ActionResult Index()
        {
            var user = new UserView();
            return View(user);
        }

        [HttpPost]
        public ActionResult Index(UserView user)
        {
            try
            {
                var dbManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

                dbManager.User.Create(user);

                FormsAuthentication.SetAuthCookie(user.Name,false);

                return RedirectToAction("Index", "Login");
            }
            catch
            {
                return View();
            }
        }
    }
}