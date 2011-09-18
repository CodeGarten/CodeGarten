using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CodeGarten.Data.Access;
using CodeGarten.Web.Attributes;
using CodeGarten.Web.Core;
using CodeGarten.Web.Model;

namespace CodeGarten.Web.Controllers
{
    [Authorize]
    public sealed class WorkSpaceTypeController : Controller
    {
        [HttpPost]
        [StructureOwner("structureId")]
        public JsonResult Create(long structureId, WorkSpaceTypeView workSpaceType, IEnumerable<string> services)
        {
            if (!ModelState.IsValid)
            {
                return FormValidationResponse.Error(ModelState);
            }

            if(services == null || !services.Any())
            {
                ModelState.AddGlobalError("A workspace must have at least one service.");
                return FormValidationResponse.Error(ModelState);
            }

            try
            {
                var dataBaseManager = (DataBaseManager)HttpContext.Items["DataBaseManager"];

                dataBaseManager.WorkSpaceType.Create(structureId, workSpaceType.Name, services);

                return FormValidationResponse.Ok();
            }
            catch
            {
                ModelState.AddGlobalError("An error has occured, please try again.");
                return FormValidationResponse.Error(ModelState);
            }
        }

        [StructureOwner("structureId")]
        public PartialViewResult Edit(long structureId, string name)
        {
            var dataBaseManager = (DataBaseManager)HttpContext.Items["DataBaseManager"];

            ViewBag.Services = dataBaseManager.ServiceType.GetAll();
            return PartialView(dataBaseManager.WorkSpaceType.Get(structureId, name));
        }

        [HttpPost]
        [StructureOwner("structureId")]
        public JsonResult Edit(long structureId, string name, IEnumerable<string> services)
        {
            if (services == null || !services.Any())
            {
                ModelState.AddGlobalError("A workspace must have at least one service.");
                return FormValidationResponse.Error(ModelState);
            }

            try
            {
                var dataBaseManager = (DataBaseManager)HttpContext.Items["DataBaseManager"];

                dataBaseManager.WorkSpaceType.Edit(structureId, name, services);

                return FormValidationResponse.Ok();

            }catch
            {
                ModelState.AddGlobalError("An error has occured, please try again.");
                return FormValidationResponse.Error(ModelState);
            }
        }

        [HttpPost]
        [StructureOwner("structureId")]
        public JsonResult Delete(long structureId, string name)
        {
            var dataBaseManager = (DataBaseManager)HttpContext.Items["DataBaseManager"];

            dataBaseManager.WorkSpaceType.Delete(structureId, name);

            return FormValidationResponse.Ok();
        }
    }
}