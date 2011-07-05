using System.Linq;
using System.Web.Mvc;
using CodeGarten.Data;
using CodeGarten.Data.Access;
using CodeGarten.Data.ModelView;
using CodeGarten.Web.Attributes;

namespace CodeGarten.Web.Controllers
{
    [Authorize]
    public sealed class RoleController : Controller
    {
        private readonly Context _context = new Context();

        [StructureOwner("structureId")]
        public ActionResult Create(long structureId, string cpName, string wsName)
        {
            var r = new RoleView();

            ViewBag.RoleTypes =
                _context.RoleTypes.Where(conp => conp.StructureId == structureId).Select(
                    conp => new SelectListItem {Text = conp.Name, Value = conp.Name});
            ViewBag.Rules =
                _context.Rules.Where(conp => conp.StructureId == structureId).Select(
                    conp => new SelectListItem {Text = conp.Name, Value = conp.Name});

            return View(r);
        }

        [HttpPost]
        [StructureOwner("structureId")]
        public ActionResult Create(long structureId, string roleTypeName, string ruleName, string cpName, string wsName)
        {
            try
            {
                var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

                dataBaseManager.Role.Create(structureId, cpName, wsName, roleTypeName, ruleName);

                return RedirectToAction("Index", "WorkSpaceType", new {structureId, name = wsName, cpName});
            }
            catch
            {
                return View();
            }
        }

        [StructureOwner("structureId")]
        public ActionResult Delete(long structureId, string containerProto, string workSpaceType, string roleType)
        {
            var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

            var cp = dataBaseManager.Role.Get(containerProto, workSpaceType, roleType);

            return View(cp);
        }

        [HttpPost]
        [StructureOwner("structureId")]
        public ActionResult Delete(long structureId, string containerProto, string workSpaceType, string roleType,
                                   RoleView roleView, FormCollection collection)
        {
            try
            {
                var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

                dataBaseManager.Role.Delete(roleView, structureId);

                return RedirectToAction("Index", "Structure", new {id = structureId});
            }
            catch
            {
                return View();
            }
        }
    }
}