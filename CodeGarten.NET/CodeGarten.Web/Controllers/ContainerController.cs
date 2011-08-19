using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CodeGarten.Data.Access;
using CodeGarten.Web.Attributes;
using CodeGarten.Web.Core;
using CodeGarten.Web.Model;

namespace CodeGarten.Web.Controllers
{
    [Authorize]
    public sealed class ContainerController : Controller
    {
        public ActionResult Index(long id, bool? partial)
        {
            var dataBaseManager = (DataBaseManager)HttpContext.Items["DataBaseManager"];

            var container = dataBaseManager.Container.Get(id);

            if (container == null)
                throw new HttpException((int)HttpStatusCode.NotFound, HttpStatusCode.NotFound.ToString());

            //Request.IsAjaxRequest() did not work very well
            if (partial != null && partial.Value)
                return PartialView("_Container", container);

            return View(container);
        }

        [StructureOwner("structureId")]
        public ActionResult Create(long structureId, string prototypeName, long? parent)
        {
            var dataBaseManager = (DataBaseManager)HttpContext.Items["DataBaseManager"];

            var container = new ContainerView();

            var prototype = dataBaseManager.ContainerPrototype.Get(structureId, prototypeName);

            if (prototype == null)
                throw new HttpException((int)HttpStatusCode.NotFound, HttpStatusCode.NotFound.ToString());

            ViewBag.Prototype = prototype;

            return View(container);
        }

        [HttpPost]
        [StructureOwner("structureId")]
        public ActionResult Create(long structureId, string prototypeName, long? parent, ContainerView container)
        {
            var dataBaseManager = (DataBaseManager)HttpContext.Items["DataBaseManager"];

            if (!ModelState.IsValid)
            {
                var prototype = dataBaseManager.ContainerPrototype.Get(structureId, prototypeName);

                if (prototype == null)
                    throw new HttpException((int)HttpStatusCode.NotFound, HttpStatusCode.NotFound.ToString());

                ViewBag.Prototype = prototype;
                return View(container);
            }

            try
            {
                var createContainer = dataBaseManager.Container.Create(structureId, container.Name, container.Description, parent, prototypeName);

                foreach (var password in container.Passwords.Where(p => !string.IsNullOrEmpty(p.Password)))
                    dataBaseManager.Container.AddPassword(structureId, createContainer.Id, password.RoleType, password.Password);

                return RedirectToAction("Index", new { id = createContainer.Id });
            }
            catch
            {
                return View(container);
            }
        }

        [StructureOwner("structureId")]
        public ActionResult Delete(long structureId, long id)
        {
            var dataBaseManager = (DataBaseManager)HttpContext.Items["DataBaseManager"];

            var container = dataBaseManager.Container.Get(id);

            if (container == null)
                throw new HttpException((int)HttpStatusCode.NotFound, HttpStatusCode.NotFound.ToString());

            ViewBag.EnrolledUsers = dataBaseManager.User.GetAll().Where(u => u.Enrolls.Any(e => e.ContainerId == id));

            return View(container);
        }

        [HttpPost]
        [StructureOwner("structureId")]
        public ActionResult Delete(long structureId, long id, ContainerView containerView)
        {
            var dataBaseManager = (DataBaseManager)HttpContext.Items["DataBaseManager"];

            var container = dataBaseManager.Container.Get(id);

            if (container == null)
                throw new HttpException((int)HttpStatusCode.NotFound, HttpStatusCode.NotFound.ToString());

            try
            {
                var parent = dataBaseManager.Container.Delete(id);

                return parent != null
                           ? RedirectToAction("Index", new { id = parent.Id })
                           : RedirectToAction("Index", "Structure", new { id = structureId });
            }
            catch (Exception)
            {
                ModelState.AddGlobalError("An error has occured, please try again.");
                return View();
            }
        }

        public ActionResult Disenroll(long structureId, long containerId, string roleTypeName)
        {
            var dataBaseManager = (DataBaseManager)HttpContext.Items["DataBaseManager"];

            if (dataBaseManager.Container.Get(containerId) == null || dataBaseManager.RoleType.Get(structureId, roleTypeName) == null)
                throw new HttpException((int)HttpStatusCode.NotFound, HttpStatusCode.NotFound.ToString());

            try
            {
                dataBaseManager.User.Disenroll(User.Identity.Name, structureId, containerId, roleTypeName);

                return RedirectToAction("Index", "Container", new { id = containerId });
            }
            catch
            {
                return RedirectToAction("Index", "Container", new { id = containerId });
            }
        }

        public ActionResult Enroll(long structureId, long containerId)
        {
            var dataBaseManager = (DataBaseManager)HttpContext.Items["DataBaseManager"];

            if(dataBaseManager.Structure.Get(structureId) == null || dataBaseManager.Container.Get(containerId) == null)
                throw new HttpException((int)HttpStatusCode.NotFound, HttpStatusCode.NotFound.ToString());

            if (dataBaseManager.User.Get(User.Identity.Name).Enrolls.Any(e => e.ContainerId == containerId && !e.Inherited))
                return RedirectToAction("Index", new { id = containerId });

            return View(dataBaseManager.Container.Get(containerId));
        }

        [HttpPost]
        public ActionResult Enroll(long structureId, long containerId, string roleTypeName, string password)
        {
            var dataBaseManager = (DataBaseManager)HttpContext.Items["DataBaseManager"];

            var container = dataBaseManager.Container.Get(containerId);

            if (dataBaseManager.Structure.Get(structureId) == null || container == null || dataBaseManager.RoleType.Get(structureId, roleTypeName) == null)
                throw new HttpException((int) HttpStatusCode.NotFound, HttpStatusCode.NotFound.ToString());

            try
            {
                if (dataBaseManager.User.Get(User.Identity.Name).Enrolls.Any(e => e.ContainerId == containerId && !e.Inherited))
                    return RedirectToAction("Index", new { id = containerId });

                if (!dataBaseManager.User.Enroll(User.Identity.Name, structureId, containerId, roleTypeName, password))
                {
                    ModelState.AddModelError("password", "Incorrect password");
                    return View(container);
                }

                return RedirectToAction("Index", new { id = containerId });
            }
            catch
            {
                ModelState.AddModelError("form", "An error occured. Please try again.");
                return View(container);
            }
        }
    }
}