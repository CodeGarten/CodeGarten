using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using CodeGarten.Data;
using CodeGarten.Data.Access;
using CodeGarten.Web.Attributes;
using CodeGarten.Web.Model;

namespace CodeGarten.Web.Controllers
{
    public sealed class ContainerController : Controller
    {

        //TODO rever o controller 
        public ActionResult Index(long id)
        {
            //var container = _context.Containers.Find(id);

            //ViewBag.Enroll =
            //    _context.Enrolls.FirstOrDefault(e => e.UserName == User.Identity.Name && e.ContainerId == id);

            //return View();

            return RedirectToAction("Index", "Container", new { id = id });
        }

        [StructureOwner("structureId")]
        public ActionResult Create(long structureId, long? parent)
        {
            var container = new ContainerView();

            return View(container);
        }

        [HttpPost]
        [StructureOwner("structureId")]
        public ActionResult Create(long structureId, long? parent, ContainerView container)
        {
            try
            {
                var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

                dataBaseManager.Container.Create(structureId, container.Name, container.Description, parent);

                return RedirectToAction("Index", "Structure", new {id = structureId});
            }
            catch (Exception)
            {
                //return View();
                throw;
            }
        }

        [StructureOwner("structureId")]
        public ActionResult Delete(long structureId, long id)
        {
            var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

            var container = dataBaseManager.Container.Get(id);

            return View(container);
        }

        [HttpPost]
        [StructureOwner("structureId")]
        public ActionResult Delete(long structureId, long id, ContainerView containerView, FormCollection formCollection)
        {
            try
            {
                var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;


                dataBaseManager.Container.Delete(id);

                return RedirectToAction("Index", "Structure", new {id = structureId});
            }
            catch (Exception)
            {
                return View();
            }
        }

        public ActionResult Leave(long structureId, long containerId, string roleTypeName)
        {
            try
            {
                var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

                dataBaseManager.User.Disenroll(User.Identity.Name, structureId, containerId, roleTypeName);

                return RedirectToAction("Index", "Container", new { id = containerId });
            }catch
            {
                return RedirectToAction("Index", "Container", new{id = containerId});
            }
        }

        public ActionResult Enroll(long structureId, long containerId)
        {
            //var enroll = new EnrollView();

            //var container = _context.Containers.Find(containerId);

            //ViewBag.RoleTypes =
            //    _context.Roles.Where(
            //        r =>
            //        r.ContainerPrototype.StructureId == structureId &&
            //        r.ContainerPrototypeName == container.ContainerPrototype.Name).Select(r => r.RoleType).
            //        ToList().Select(rt => new SelectListItem {Text = rt.Name, Value = rt.Name});

            //return View();
            return RedirectToAction("Index", "Container", new { id = containerId });
        }

        [HttpPost]
        public ActionResult Enroll(long structureId, long containerId, string roleTypeName)
        {
            try
            {
                var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

                dataBaseManager.User.Enroll(User.Identity.Name, structureId, containerId, roleTypeName);

                return RedirectToAction("Index", new {id = containerId});
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Container", new { id = containerId });
            }
        }
    }
}