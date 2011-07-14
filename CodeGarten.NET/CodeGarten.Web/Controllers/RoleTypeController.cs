﻿using System.Web.Mvc;
using CodeGarten.Data.Access;
using CodeGarten.Data.ModelView;
using CodeGarten.Web.Attributes;
using CodeGarten.Web.Core;

namespace CodeGarten.Web.Controllers
{
    [Authorize]
    public sealed class RoleTypeController : Controller
    {

        [HttpPost]
        [StructureOwner("structureId")]
        public JsonResult Create(long structureId, RoleTypeView roleType)
        {
            try
            {
                var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

                dataBaseManager.RoleType.Create(roleType, structureId);

                return Json(new { Errors = new string[0] }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                ModelState.AddModelError("form", "An error occured. Please try again.");
                return ModelState.ToJson();
            }
        }

        //[StructureOwner("structureId")]
        public PartialViewResult Delete(long structureId, string name)
        {
            var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

            return PartialView(dataBaseManager.RoleType.Get(structureId,name));
        }

        [HttpPost]
        //[StructureOwner("structureId")]
        public JsonResult Delete(long structureId, string name, FormCollection formCollection)
        {
            var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

            dataBaseManager.RoleType.Delete(new RoleTypeView{Name = name}, structureId);

            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}