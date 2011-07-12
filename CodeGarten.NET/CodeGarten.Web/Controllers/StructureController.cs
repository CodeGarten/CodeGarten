using System.Linq;
using System.Web.Mvc;
using CodeGarten.Data;
using CodeGarten.Data.Access;
using CodeGarten.Data.ModelView;
using CodeGarten.Web.Attributes;

namespace CodeGarten.Web.Controllers
{
    [Authorize]
    public sealed class StructureController : Controller
    {
        private readonly Context _context = new Context();

        public ActionResult Index(long id)
        {
            var cp = new ContainerPrototypeView();
            ViewBag.StructureId = id;
            return View(cp);
        }

        public ActionResult Create()
        {
            var structure = new StructureView();

            return View(structure);
        }

        [HttpPost]
        public ActionResult Create(StructureView structure)
        {
            try
            {
                var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

                dataBaseManager.Structure.Create(structure, User.Identity.Name);

                return RedirectToAction("Index", new {id = structure.Id});
            }
            catch
            {
                return View();
            }
        }

        [StructureOwner("id")]
        public ActionResult Delete(long id)
        {
            var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

            var structure = dataBaseManager.Structure.Get(id);

            return View(structure);
        }

        [HttpPost]
        [StructureOwner("id")]
        public ActionResult Delete(long id, StructureView structureView, FormCollection formCollection)
        {
            try
            {
                var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

                dataBaseManager.Structure.Delete(structureView);

                return RedirectToAction("Index", "User");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public PartialViewResult Search(string name)
        {
            var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

            return PartialView("_StructureTable", _context.Structures.Where(s => s.Name.StartsWith(name)));
        }
    }
}