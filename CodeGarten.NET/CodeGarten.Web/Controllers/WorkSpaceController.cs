using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CodeGarten.Data;

namespace CodeGarten.Web.Controllers
{
    [Authorize]
    public class WorkSpaceController : Controller
    {
        //
        // GET: /WorkSpace/

        public ActionResult Index(long structureId, long containerId, string workspaceTypeName)
        {
            ViewBag.structureId = structureId;
            ViewBag.containerId = containerId;
            ViewBag.workspaceTypeName = workspaceTypeName;
            Context context = new Context();

            ViewBag.services = context.WorkSpaceTypes.Find(workspaceTypeName, structureId).Services;

            context.Dispose();

            return View();
        }
    }
}