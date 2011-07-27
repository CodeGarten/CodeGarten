using System.Web.Mvc;
using CodeGarten.Data.Access;
using CodeGarten.Web.Attributes;
using CodeGarten.Web.Core;
using CodeGarten.Web.Model;

namespace CodeGarten.Web.Controllers
{
    [Authorize]
    public sealed class ContainerPrototypeController : Controller
    {
        [HttpPost]
        [StructureOwner("structureId")]
        public JsonResult Create(long structureId, ContainerPrototypeView containerPrototype, string parent)
        {
            if (!ModelState.IsValid)
                return FormValidationResponse.Error(ModelState);
            try
            {
                var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

                dataBaseManager.ContainerPrototype.Create(structureId, containerPrototype.Name, parent);

                return FormValidationResponse.Ok();
            }
            catch
            {
                ModelState.AddGlobalError("An error has occured, please try again.");
                return FormValidationResponse.Error(ModelState);
            }
        }

        [HttpPost]
        [StructureOwner("structureId")]
        public JsonResult Delete(long structureId, string name)
        {
            try
            {
                var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

                dataBaseManager.ContainerPrototype.Delete(structureId, name);

                return FormValidationResponse.Ok();
            }
            catch
            {
                ModelState.AddGlobalError("An error has occured, please try again.");
                return FormValidationResponse.Error(ModelState);
            }
        }
    }
}