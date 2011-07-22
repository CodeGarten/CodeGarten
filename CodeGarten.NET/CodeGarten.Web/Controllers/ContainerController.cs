using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using CodeGarten.Data;
using CodeGarten.Data.Access;
//using CodeGarten.Service;
using CodeGarten.Web.Attributes;
using CodeGarten.Web.Model;

namespace CodeGarten.Web.Controllers
{
    public sealed class ContainerController : Controller
    {
        public ActionResult Index(long id)
        {
            var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

            var container = dataBaseManager.Container.Get(id);

            ViewBag.Enroll = dataBaseManager.User.Enrolls(User.Identity.Name, id);

            return View(container);
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

                dataBaseManager.Container.Create(structureId, container.Name, parent);

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
        public ActionResult Delete(long structureId, long id, ContainerView containerView)
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
            var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

            dataBaseManager.User.Leave(User.Identity.Name, containerId);

            return RedirectToAction("Index", "Structure", new {id = structureId});
        }

        //public ActionResult Enroll(long structureId, long containerId)
        //{
        //    var enroll = new EnrollView();

        //    var container = dataBaseManager.Container.Get(containerId);

        //    ViewBag.RoleTypes =
        //        _context.Roles.Where(
        //            r =>
        //            r.ContainerPrototype.StructureId == structureId &&
        //            r.ContainerPrototypeName == container.ContainerPrototype.Name).Select(r => r.RoleType).
        //            ToList().Select(rt => new SelectListItem {Text = rt.Name, Value = rt.Name});

        //    return View(enroll);
        //}

        //[HttpPost]
        //public ActionResult Enroll(long structureId, long containerId, string roleTypeName)
        //{
        //    try
        //    {
        //        var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

        //        dataBaseManager.User.Enroll(User.Identity.Name, structureId, containerId, roleTypeName);

        //        return RedirectToAction("Index", new {id = containerId});
        //    }
        //    catch (Exception)
        //    {
        //        return View();
        //    }
        //}
    }
}