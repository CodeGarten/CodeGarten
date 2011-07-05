using System.Linq;
using System.Web.Mvc;
using CodeGarten.Data;
using CodeGarten.Data.Access;
using CodeGarten.Data.ModelView;
using CodeGarten.Web.Attributes;

namespace CodeGarten.Web.Controllers
{
    [Authorize]
    public sealed class WorkSpaceTypeController : Controller
    {
        private readonly Context _context = new Context();

        [StructureOwner("structureId")]
        public ActionResult Index(long structureId, string name, string cpName)
        {
            var workspaceType = _context.WorkSpaceTypes.Find(name, structureId);

            ViewBag.Services = _context.Services;

            ViewBag.CPname = cpName;

            ViewBag.Roles =
                _context.Roles.Where(
                    r =>
                    r.ContainerPrototypeStructureId == structureId && r.ContainerPrototypeName == cpName &&
                    r.WorkSpaceTypeName == name);

            return View(workspaceType);
        }

        [StructureOwner("structureId")]
        public ActionResult Create(long structureId)
        {
            var wt = new WorkSpaceTypeView();

            return View(wt);
        }

        [HttpPost]
        [StructureOwner("structureId")]
        public ActionResult Create(long structureId, WorkSpaceTypeView workSpaceType, string cpName)
        {
            try
            {
                var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

                dataBaseManager.WorkSpaceType.Create(workSpaceType, structureId);

                return cpName == null
                           ? RedirectToAction("Index", "Structure", new {id = structureId})
                           : RedirectToAction("Index", "ContainerPrototype", new {structureId, name = cpName});
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

            var wt = dataBaseManager.WorkSpaceType.Get(structureId, name);

            return View(wt);
        }

        [HttpPost]
        [StructureOwner("structureId")]
        public ActionResult Delete(long structureId, string name, WorkSpaceTypeView workSpaceTypeView)
        {
            try
            {
                var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

                dataBaseManager.WorkSpaceType.Delete(workSpaceTypeView, structureId);

                return RedirectToAction("Index", "Structure", new {id = structureId});
            }
            catch
            {
                return View();
            }
        }

        [StructureOwner("structureId")]
        public ActionResult AddService(long structureId, string workspaceName, string serviceName)
        {
            try
            {
                var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

                dataBaseManager.WorkSpaceType.AddService(structureId, workspaceName, serviceName);

                return RedirectToAction("Index", new {structureId, name = workspaceName});
            }
            catch
            {
                return RedirectToAction("Index", new {structureId, name = workspaceName});
            }
        }

        [StructureOwner("structureId")]
        public ActionResult RemoveService(long structureId, string workspaceName, string serviceName)
        {
            try
            {
                var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

                dataBaseManager.WorkSpaceType.RemoveService(structureId, workspaceName, serviceName);

                return RedirectToAction("Index", new {structureId, name = workspaceName});
            }
            catch
            {
                return RedirectToAction("Index", new {structureId, name = workspaceName});
            }
        }
    }
}