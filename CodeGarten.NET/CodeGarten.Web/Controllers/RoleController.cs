using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CodeGarten.Data;
using CodeGarten.Data.Model;
using CodeGarten.Web.Attributes;

namespace CodeGarten.Web.Controllers
{
    [Authorize]
    public sealed class RoleController : Controller
    {
        private readonly Context _context = new Context();

        [HttpPost]
        [StructureOwner("structureId")]
        public JsonResult Create(long structureId, IEnumerable<Role> roles)
        {
            _context.Roles.Local.Clear();

            foreach (var role in roles)
            {
                role.ContainerPrototypeStructureId =
                    role.RoleTypeStructureId = role.RuleStructureId = role.WorkSpaceTypeStructureId = structureId;

                _context.Roles.Add(role);
            }

            _context.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}