using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CodeGarten.Data;
using CodeGarten.Data.Model;
using CodeGarten.Web.Attributes;
using CodeGarten.Web.Core;

namespace CodeGarten.Web.Controllers
{
    [Authorize]
    public sealed class WorkSpaceTypeController : Controller
    {
        private readonly Context _context = new Context();

        [HttpPost]
        [StructureOwner("structureId")]
        public JsonResult Create(long structureId, WorkSpaceType workSpaceType, IEnumerable<string> services)
        {
            try
            {
                if (services != null)
                    foreach (var service in services.Select(s => _context.Services.Find(s)))
                        workSpaceType.Services.Add(service);

                _context.WorkSpaceTypes.Add(workSpaceType);
                _context.SaveChanges();

                return FormValidationResponse.Ok();
            }
            catch
            {
                return FormValidationResponse.Error(ModelState);
                return FormValidationResponse.Error(ModelState);
            }
        }

        public PartialViewResult Edit(long structureId, string name)
        {
            ViewBag.Services = _context.Services;
            return PartialView(_context.WorkSpaceTypes.Find(name, structureId));
        }

        [HttpPost]
        public JsonResult Edit(long structureId, string name, IEnumerable<string> services)
        {
            var workspace = _context.WorkSpaceTypes.Find(name, structureId);

            workspace.Services.Clear();

            if (services != null)
                foreach (var service in services.Select(s => _context.Services.Find(s)))
                    workspace.Services.Add(service);

            _context.SaveChanges();
            return FormValidationResponse.Ok();
        }

        [HttpPost]
        //[StructureOwner("structureId")]
        public JsonResult Delete(long structureId, string name)
        {
            _context.WorkSpaceTypes.Remove(_context.WorkSpaceTypes.Find(name, structureId));
            _context.SaveChanges();
            return FormValidationResponse.Ok();
        }
    }
}