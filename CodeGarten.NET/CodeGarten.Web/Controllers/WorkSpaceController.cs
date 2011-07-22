using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CodeGarten.Data;
using CodeGarten.Data.Access;

namespace CodeGarten.Web.Controllers
{
    [Authorize]
    public sealed class WorkSpaceController : Controller
    {
        public ActionResult Index(long structureId, long containerId, string workspaceTypeName)
        {
            var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

            ViewBag.structureId = structureId;
            ViewBag.containerId = containerId;
            ViewBag.workspaceTypeName = workspaceTypeName;

            ViewBag.services = dataBaseManager.WorkSpaceType.Get(structureId, workspaceTypeName).Services;

            return View();
        }
    }
}