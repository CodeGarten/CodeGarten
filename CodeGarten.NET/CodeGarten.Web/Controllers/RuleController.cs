using System.Web.Mvc;
using CodeGarten.Data;
using CodeGarten.Data.Access;
using CodeGarten.Data.ModelView;
using CodeGarten.Web.Attributes;

namespace CodeGarten.Web.Controllers
{
    [Authorize]
    public sealed class RuleController : Controller
    {
        private readonly Context _context = new Context();

        [StructureOwner("structureId")]
        public ActionResult Index(long structureId, string name)
        {
            var rule = _context.Rules.Find(name, structureId);

            ViewBag.Services = _context.Services;

            return View(rule);
        }

        [StructureOwner("structureId")]
        public ActionResult Create(long structureId)
        {
            var r = new RuleView();

            return View(r);
        }

        [HttpPost]
        [StructureOwner("structureId")]
        public ActionResult Create(long structureId, RuleView rule)
        {
            try
            {
                var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

                dataBaseManager.Rule.Create(rule, structureId);

                return RedirectToAction("Index", new {structureId, name = rule.Name});
            }
            catch
            {
                return View();
            }
        }

        [StructureOwner("structureId")]
        public ActionResult Delete(long structureId, string name)
        {
            var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

            var cp = dataBaseManager.Rule.Get(structureId, name);

            return View(cp);
        }

        [HttpPost]
        [StructureOwner("structureId")]
        public ActionResult Delete(long structureId, string name, RuleView ruleView, FormCollection collection)
        {
            try
            {
                var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

                dataBaseManager.Rule.Delete(ruleView, structureId);

                return RedirectToAction("Index", "Structure", new {id = structureId});
            }
            catch
            {
                return View();
            }
        }

        //[HttpPost]
        [StructureOwner("structureId")]
        public ActionResult AddPermission(long structureId, string serviceName, string permissionName, string ruleName)
        {
            try
            {
                var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

                dataBaseManager.Rule.AddPermission(structureId, ruleName, serviceName, permissionName);

                return RedirectToAction("Index", new {structureId, name = ruleName});
            }
            catch
            {
                return RedirectToAction("Index", new {structureId, name = ruleName});
            }
        }

        //[HttpPost]
        [StructureOwner("structureId")]
        public ActionResult RemovePermission(long structureId, string serviceName, string permissionName,
                                             string ruleName)
        {
            try
            {
                var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

                dataBaseManager.Rule.RemovePermission(structureId, ruleName, serviceName, permissionName);

                return RedirectToAction("Index", new {structureId, name = ruleName});
            }
            catch
            {
                return RedirectToAction("Index", new {structureId, name = ruleName});
            }
        }
    }
}