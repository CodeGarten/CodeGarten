using System;
using System.Linq;
using System.Web.Mvc;
using CodeGarten.Data.Access;
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

            if (Request.IsAjaxRequest())
                return PartialView("_Container", container);

            return View(container);
        }

        [StructureOwner("structureId")]
        public ActionResult Create(long structureId, string prototypeName, long? parent)
        {
            var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

            var container = new ContainerView();

            ViewBag.Prototype = dataBaseManager.ContainerPrototype.Get(structureId, prototypeName);

            return View(container);
        }

        [HttpPost]
        [StructureOwner("structureId")]
        public ActionResult Create(long structureId, string prototypeName, long? parent, ContainerView container)
        {
            try
            {
                var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

                if (!ModelState.IsValid)
                {
                    ViewBag.Prototype = dataBaseManager.ContainerPrototype.Get(structureId, prototypeName);
                    return View(container);
                }

                var createContainer = dataBaseManager.Container.Create(structureId, container.Name, container.Description, parent, prototypeName);

                foreach(var password in container.Passwords.Where(p => !string.IsNullOrEmpty(p.Password)))
                    dataBaseManager.Container.AddPassword(structureId, createContainer.Id, password.RoleType, password.Password);

                return RedirectToAction("Index", new {id = createContainer.Id});
            }
            catch
            {
                return View(container);
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
            var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

            if (dataBaseManager.User.Get(User.Identity.Name).Enrolls.Any(e => e.ContainerId == containerId && !e.Inherited))
                return RedirectToAction("Index", new {id = containerId});

            return View(dataBaseManager.Container.Get(containerId));
        }

        [HttpPost]
        public ActionResult Enroll(long structureId, long containerId, string roleTypeName, string password)
        {
            try
            {
                var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

                if (dataBaseManager.User.Get(User.Identity.Name).Enrolls.Any(e => e.ContainerId == containerId && !e.Inherited))
                    return RedirectToAction("Index", new { id = containerId });

                if(!dataBaseManager.User.Enroll(User.Identity.Name, structureId, containerId, roleTypeName, password))
                {
                    ModelState.AddModelError("password", "Incorrect password");
                    return View(dataBaseManager.Container.Get(containerId));
                }

                return RedirectToAction("Index", new {id = containerId});
            }
            catch
            {
                var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;
                ModelState.AddModelError("form", "An error occured. Please try again.");
                return View(dataBaseManager.Container.Get(containerId));
            }
        }
    }
}