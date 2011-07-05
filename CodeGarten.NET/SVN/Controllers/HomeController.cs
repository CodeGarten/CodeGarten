using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace SVN.Controllers
{
    [Authorize]
    [Export(typeof (IController)), ExportMetadata("ControllerName", "Home"),
     PartCreationPolicy(CreationPolicy.NonShared)]
    public class HomeController : Controller //, IEntryPointController
    {
        //
        // GET: /Home/structureId/containerId/workspaceTypeName

        public ActionResult Index(long structureId, long containerId, string workspaceTypeName)
        {
            ViewBag.containerId = containerId;
            ViewBag.structureId = structureId;
            ViewBag.workspaceTypeName = workspaceTypeName;

            return PartialView("~/Services/SVN.dll/SVN/Views/Home/Index.cshtml");
        }
    }
}