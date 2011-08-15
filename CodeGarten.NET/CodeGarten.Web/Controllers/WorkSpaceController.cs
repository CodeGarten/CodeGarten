using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CodeGarten.Data.Access;

namespace CodeGarten.Web.Controllers
{
    public class WorkSpaceController : Controller
    {
        //
        // GET: /WorkSpace/structureId/containerId/wkName/workspaceTypeName

        public ActionResult Index(long structureId, long containerId, string workSpaceTypeName, string serviceName)
        {

            var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

            ViewBag.Container = dataBaseManager.Container.Get(containerId);
            ViewBag.WorkSpaceType = dataBaseManager.WorkSpaceType.Get(structureId, workSpaceTypeName);
            ViewBag.StructureId = structureId;
            ViewBag.ServiceName = serviceName;
            return View();
        }

    }
}
