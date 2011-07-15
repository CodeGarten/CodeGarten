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
    public sealed class RuleController : Controller
    {
        private readonly Context _context = new Context();

        [HttpPost]
        [StructureOwner("structureId")]
        public JsonResult Create(long structureId, Rule rule, IEnumerable<string> permissions)
        {
            try
            {
                if (permissions != null)
                    foreach (var permission in permissions.Select(p => p.Split(' ')))
                    {
                        var serviceName = permission[0];
                        var permissionName = permission[1];
                        rule.Permissions.Add(_context.ServicePermissions.Find(permissionName, serviceName));
                    }

                _context.Rules.Add(rule);
                _context.SaveChanges();

                return FormValidationResponse.Ok();
            }
            catch
            {
                return FormValidationResponse.Error(ModelState);
            }
        }

        public PartialViewResult Edit(long structureId, string name)
        {
            ViewBag.Services = _context.Services;
            return PartialView(_context.Rules.Find(name, structureId));
        }

        [HttpPost]
        public JsonResult Edit(long structureId, string name, IEnumerable<string> permissions)
        {
            var rule = _context.Rules.Find(name, structureId);

            rule.Permissions.Clear();

            if (permissions != null)
                foreach (var permission in permissions.Select(p => p.Split(' ')))
                {
                    var serviceName = permission[0];
                    var permissionName = permission[1];
                    rule.Permissions.Add(_context.ServicePermissions.Find(permissionName, serviceName));
                }

            _context.SaveChanges();
            return FormValidationResponse.Ok();
        }

        [HttpPost]
        //[StructureOwner("structureId")]
        public JsonResult Delete(long structureId, string name)
        {
            _context.Rules.Remove(_context.Rules.Find(name, structureId));
            _context.SaveChanges();
            return FormValidationResponse.Ok();
        }
    }
}