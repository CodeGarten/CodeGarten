using System;
using System.Linq;
using System.Web.Mvc;
using CodeGarten.Data;
using CodeGarten.Data.Access;
using CodeGarten.Data.ModelView;
using CodeGarten.Web.Attributes;
using CodeGarten.Web.Model;

namespace CodeGarten.Web.Controllers
{
    [Authorize]
    public sealed class ContainerPrototypeController : Controller
    {
        private readonly Context _context = new Context();

        [StructureOwner("structureId")]
        public ActionResult Index(long structureId, string name)
        {
            var containerPrototype = _context.ContainerPrototypes.Find(name, structureId);

            ViewBag.WorkSpaces = _context.WorkSpaceTypes.Where(ws => ws.StructureId == structureId);

            return View(containerPrototype);
        }

        [StructureOwner("structureId")]
        public ActionResult Create(long structureId, string parent)
        {
            var cp = new ContainerPrototypeView();
            
            return View(cp);
        }

        [HttpPost]
        [StructureOwner("structureId")]
        public JsonResult Create(long structureId, ContainerPrototypeView containerPrototype, string parent)
        {
            try
            {
                var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

                if (parent != null)
                    dataBaseManager.ContainerPrototype.Create(containerPrototype, structureId, parent);
                else
                    dataBaseManager.ContainerPrototype.Create(containerPrototype, structureId);

            }
            catch(ArgumentException e)
            {
                ModelState.AddModelError("form", "EROROROOROROROROROROROROR");
            }
            
            return ModelState.ToJson();
        }

        [HttpPost]
        [StructureOwner("structureId")]
        public ActionResult Delete(long structureId, string name, ContainerPrototypeView containerPrototypeView,
                                   FormCollection collection)
        {
            try
            {
                var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

                dataBaseManager.ContainerPrototype.Delete(containerPrototypeView, structureId);

                return RedirectToAction("Index", "Structure", new {id = structureId});
            }
            catch
            {
                return View();
            }
        }

        [StructureOwner("structureId")]
        public ActionResult AddWorkSpace(long structureId, string workspaceName, string containerName)
        {
            var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

            dataBaseManager.ContainerPrototype.AddWorkSpaceType(structureId, containerName, workspaceName);

            return RedirectToAction("Index", new {structureId, name = containerName});
        }

        [StructureOwner("structureId")]
        public ActionResult RemoveWorkSpace(long structureId, string workspaceName, string containerName)
        {
            var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

            dataBaseManager.ContainerPrototype.RemoveWorkSpaceType(structureId, containerName, workspaceName);

            return RedirectToAction("Index", new {structureId, name = containerName});
        }
    }
}