using System.Web.Mvc;
using CodeGarten.Data;

namespace CodeGarten.Web.Controllers
{
    [Authorize]
    public sealed class UserController : Controller
    {
        private readonly Context _context = new Context();

        public ActionResult Index()
        {
            var user = _context.Users.Find(User.Identity.Name);

            return View(user);
        }
    }
}