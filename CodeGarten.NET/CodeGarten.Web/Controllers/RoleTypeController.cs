using System.Web.Mvc;
using CodeGarten.Data.Access;
using CodeGarten.Data.ModelView;
using CodeGarten.Web.Attributes;

namespace CodeGarten.Web.Controllers
{
    [Authorize]
    public sealed class RoleTypeController : Controller
    {
        [StructureOwner("structureId")]
        public ActionResult Create(long structureId)
        {
            var rt = new RoleTypeView();

            return View(rt);
        }

        [HttpPost]
        [StructureOwner("structureId")]
        public ActionResult Create(long structureId, RoleTypeView roleType)
        {
            try
            {
                var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

                dataBaseManager.RoleType.Create(roleType, structureId);

                return RedirectToAction("Index", "Structure", new {id = structureId});
            }
            catch
            {
                return View();
            }
        }

        [StructureOwner("structureId")]
        public ActionResult Delete(long structureId, string name)
        {
            var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

            var rt = dataBaseManager.RoleType.Get(structureId, name);

            return View(rt);
        }

        [HttpPost]
        [StructureOwner("structureId")]
        public ActionResult Delete(long structureId, string name, RoleTypeView roleTypeView, FormCollection collection)
        {
            try
            {
                var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

                dataBaseManager.RoleType.Delete(roleTypeView, structureId);

                return RedirectToAction("Index", "Structure", new {id = structureId});
            }
            catch
            {
                return View();
            }
        }
    }
}