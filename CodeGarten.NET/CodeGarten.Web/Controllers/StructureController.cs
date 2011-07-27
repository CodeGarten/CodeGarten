﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CodeGarten.Data.Access;
using CodeGarten.Data.Model;
using CodeGarten.Web.Attributes;
using CodeGarten.Web.Core;
using CodeGarten.Web.Model;

namespace CodeGarten.Web.Controllers
{
    [Authorize]
    public sealed class StructureController : Controller
    {
        [StructureOwner("id")]
        public JsonResult Synchronization(long id)
        {
            var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

            var ContainerPrototypes = dataBaseManager.ContainerPrototype.GetAll(id).Select(cp => new { cp.Name, ParentName = cp.Parent == null ? null : cp.Parent.Name });
            var Roles = dataBaseManager.Role.GetAll(id).Select(rl => new { rl.ContainerPrototypeName, rl.RoleTypeName, rl.WorkSpaceTypeName, Rules = rl.Rules.Select(rule => new { rule.Name }), rl.RoleBarrier });
            var Bindings =
                dataBaseManager.ContainerPrototype.GetAll(id).Select(cp => cp.Bindings).Select(
                    bl => bl.Select(b => new { b.ContainerPrototypeName, b.WorkSpaceTypeName })).SingleOrDefault();
            var RoleTypes = dataBaseManager.RoleType.GetAll(id).Select(rt => new { rt.Name });
            var WorkSpaceTypes = dataBaseManager.WorkSpaceType.GetAll(id).Select(wk => new { wk.Name });
            var Rules = dataBaseManager.Rule.GetAll(id).Select(rl => new { rl.Name });

            return Json(new
                            {
                                ContainerPrototypes,
                                Roles,
                                Bindings,
                                RoleTypes,
                                WorkSpaceTypes,
                                Rules,
                                Success = true
                            }, JsonRequestBehavior.AllowGet);

        }

        [StructureOwner("id")]
        public ActionResult Design(long id)
        {
            var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

            var structure = dataBaseManager.Structure.Get(id);

            if (!structure.Developing)
                return RedirectToAction("Index", new { id });

            ViewBag.Services = dataBaseManager.Service.GetAll();

            return View(structure);
        }

        [HttpPost]
        [StructureOwner("id")]
        public JsonResult Design(long id, IEnumerable<RoleView> roles)
        {
            try
            {
                var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

                dataBaseManager.ContainerPrototype.ClearAllBindings(id);

                if (roles != null)
                    foreach (var role in roles.Where(r => !string.IsNullOrEmpty(r.WorkSpaceTypeName)))
                    {
                        dataBaseManager.ContainerPrototype.Bind(id, role.ContainerPrototypeName, role.WorkSpaceTypeName);

                        if (!string.IsNullOrEmpty(role.RoleTypeName))
                            dataBaseManager.Role.Create(id, role.ContainerPrototypeName, role.WorkSpaceTypeName,
                                                        role.RoleTypeName,
                                                        role.Rules == null ? null : role.Rules.Select(rule => rule.Name),
                                                        (RoleBarrier) role.RoleBarrier);
                    }

                return FormValidationResponse.Ok();
            }
            catch
            {
                ModelState.AddGlobalError("An error has occured, please try again.");
                return FormValidationResponse.Error(ModelState);
            }
        }

        public ActionResult Index(long? id)
        {
            var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

            if(id == null)
            {
                var structures = dataBaseManager.Structure.GetAll(User.Identity.Name);
                return View("Structures", structures);
            }

            var structure = dataBaseManager.Structure.Get(id.Value);

            if (structure.Developing)
                return RedirectToAction("Design", new { id });

            return View(structure);
        }

        public ActionResult Create()
        {
            var structure = new StructureView();

            return View(structure);
        }

        [HttpPost]
        public ActionResult Create(StructureView structure)
        {
            if (!ModelState.IsValid)
                return View(structure);

            try
            {
                var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

                var dbStructure = dataBaseManager.Structure.Create(structure.Name, structure.Description,
                                                                   structure.Public, User.Identity.Name);

                return RedirectToAction("Design", new { id = dbStructure.Id });
            }
            catch
            {
                ModelState.AddGlobalError("An error has occured, please try again.");
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
        public ActionResult Delete(long id, StructureView structureView)
        {
            try
            {
                var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

                dataBaseManager.Structure.Delete(id);

                return RedirectToAction("Index", "Home");
            }
            catch
            {
                ModelState.AddGlobalError("An error has occured, please try again.");
                return View();
            }
        }

        [HttpPost]
        [StructureOwner("id")]
        public ActionResult Publish(long id)
        {
            try
            {
                var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

                dataBaseManager.Structure.Publish(id);

                return FormValidationResponse.Ok();

            }catch(InvalidOperationException e)
            {
                ModelState.AddGlobalError(e.Message);
                return FormValidationResponse.Error(ModelState);
            }
            catch(Exception)
            {
                ModelState.AddGlobalError("An error has occured, please try again.");
                return FormValidationResponse.Error(ModelState);
            }
        }
    }
}