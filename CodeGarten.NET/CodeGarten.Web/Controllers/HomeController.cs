using System.Linq;
using System.Web.Mvc;
using CodeGarten.Data;

namespace CodeGarten.Web.Controllers
{
    public sealed class HomeController : Controller
    {
        private readonly Context _context = new Context();

        public ActionResult Index()
        {
            ViewBag.TopStructures =
                _context.Structures.OrderBy(s => _context.Enrolls.Count(e => e.RoleTypeStructureId == s.Id)).Take(10);

            ViewBag.RecentStructures =
                _context.Structures.OrderByDescending(s => s.CreatedOn).Take(10);

            return View();
        }
    }
}