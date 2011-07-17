using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using CodeGarten.Data;
using CodeGarten.Data.Access;
using CodeGarten.Data.Model;
using CodeGarten.Data.ModelView;
using CodeGarten.Web.Attributes;
using CodeGarten.Web.Core;

namespace CodeGarten.Web.Controllers
{
    [Authorize]
    public sealed class StructureController : Controller
    {
        private readonly Context _context = new Context();
        
        public JsonResult Synchronization(long id)
        {
            var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;
            if (dataBaseManager == null)
                return Json(new {Success = false}, JsonRequestBehavior.AllowGet);

            var ContainerPrototypes = dataBaseManager.ContainerPrototype.GetAll(id).Select(cp => new {cp.Name, ParentName = cp.Parent==null?null:cp.Parent.Name});
            var Roles = dataBaseManager.Role.GetAll(id).Select(rl => new {rl.ContainerPrototypeName,rl.RoleTypeName,rl.WorkSpaceTypeName,rl.RuleName});
            var RoleTypes = dataBaseManager.RoleType.GetAll(id).Select(rt => new {rt.Name});
            var WorkSpaceTypes = dataBaseManager.WorkSpaceType.GetAll(id).Select(wk => new {wk.Name});
            var Rules = dataBaseManager.Rule.GetAll(id).Select(rl => new {rl.Name});

            return Json(new
                            {
                                ContainerPrototypes,
                                Roles,
                                RoleTypes,
                                WorkSpaceTypes,
                                Rules,
                                Success = true
                            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Design(long id)
        {
            var structure = _context.Structures.Find(id);

            if (!structure.Developing)
                return RedirectToAction("Index", new {id});

            ViewBag.Services = _context.Services;

            return View(structure);
        }

        [HttpPost]
        public JsonResult Design(long id, IEnumerable<Role> roles)
        {
            foreach (var role in _context.Roles.Where(r => r.ContainerPrototypeStructureId == id))
                _context.Roles.Remove(role);

            foreach (var role in roles)
            {
                role.ContainerPrototypeStructureId =
                    role.RoleTypeStructureId = role.RuleStructureId = role.WorkSpaceTypeStructureId = id;

                _context.Roles.Add(role);
            }

            _context.SaveChanges();

            return FormValidationResponse.Ok();
        }

        public ActionResult Index(long id)
        {
            var structure = _context.Structures.Find(id);

            if (structure.Developing)
                return RedirectToAction("Design", new{id});

            return View();
        }

        public ActionResult Create()
        {
            var structure = new Structure();

            return View(structure);
        }

        [HttpPost]
        public ActionResult Create(Structure structure)
        {
            try
            {
                structure.Developing = true;
                structure.Public = true;
                structure.Administrators.Add(_context.Users.Find(User.Identity.Name));
                structure.CreatedOn = DateTime.Now;

                _context.Structures.Add(structure);
                _context.SaveChanges();

                return RedirectToAction("Design", new {id = structure.Id});
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

        [HttpPost]
        [StructureOwner("id")]
        public ActionResult Publish(long id)
        {
            var structure = _context.Structures.Find(id);
            structure.Developing = false;
            _context.SaveChanges();

            return FormValidationResponse.Ok();
        }
    }
}