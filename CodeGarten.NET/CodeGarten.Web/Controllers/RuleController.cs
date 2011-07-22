using System.Collections.Generic;
using System.Web.Mvc;
using CodeGarten.Data.Access;
using CodeGarten.Web.Attributes;
using CodeGarten.Web.Core;
using CodeGarten.Web.Model;

namespace CodeGarten.Web.Controllers
{
    [Authorize]
    public sealed class RuleController : Controller
    {
        [HttpPost]
        [StructureOwner("structureId")]
        public JsonResult Create(long structureId, RuleView rule, IEnumerable<string> permissions)
        {
            if (!ModelState.IsValid)
                return FormValidationResponse.Error(ModelState);

            try
            {
                var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

                dataBaseManager.Rule.Create(structureId, rule.Name, permissions);

                return FormValidationResponse.Ok();
            }
            catch
            {
                ModelState.AddGlobalError("An error has occured, please try again.");
                return FormValidationResponse.Error(ModelState);
            }
        }

        public PartialViewResult Edit(long structureId, string name)
        {
            var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

            ViewBag.Services = dataBaseManager.Service.GetAll();
            return PartialView(dataBaseManager.Rule.Get(structureId, name));
        }

        [HttpPost]
        [StructureOwner("structureId")]
        public JsonResult Edit(long structureId, string name, IEnumerable<string> permissions)
        {
            var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

            dataBaseManager.Rule.Edit(structureId, name, permissions);

            return FormValidationResponse.Ok();
        }

        [HttpPost]
        [StructureOwner("structureId")]
        public JsonResult Delete(long structureId, string name)
        {
            var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

            dataBaseManager.Rule.Delete(structureId, name);
            
            return FormValidationResponse.Ok();
        }
    }
}