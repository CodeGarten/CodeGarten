using System;
using System.Linq;
using System.Web.Mvc;
using CodeGarten.Data;
using CodeGarten.Data.Access;
using CodeGarten.Data.ModelView;
using CodeGarten.Web.Attributes;
using CodeGarten.Web.Core;

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
            ViewBag.structureId = structureId;

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

                if (String.IsNullOrEmpty(parent))
                    dataBaseManager.ContainerPrototype.Create(containerPrototype, structureId);
                else
                    dataBaseManager.ContainerPrototype.Create(containerPrototype, structureId, parent);
                    
                return FormValidationResponse.Ok();
            }
            catch(ArgumentException e)
            {
                ModelState.AddGlobalError("EROROROOROROROROROROROROR");
            }

            return FormValidationResponse.Error(ModelState);
        }

        [HttpPost]
        [StructureOwner("structureId")]
        public ActionResult Delete(long structureId, string name)
        {
            try
            {
                var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

                dataBaseManager.ContainerPrototype.Delete(name, structureId);

                return FormValidationResponse.Ok();
            }
            catch
            {
                //TODO Json result error
                return View();
                // JsonValidationResponse.Error("sdfsdfsdf");
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