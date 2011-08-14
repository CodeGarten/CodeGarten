using System.ComponentModel.Composition;
using System.Web.Mvc;
using CodeGarten.Service;
using CodeGarten.Service.Interfaces;

namespace Trac.Controllers
{
    [Authorize]
    [Export(typeof (ServiceController)), ExportMetadata("ControllerName", "Home"),
     PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class HomeController : ServiceController, IServiceEntryPoint
    {
        //
        // GET: /Home/structureId/containerId/workspaceTypeName

        public ActionResult Index(long structureId, long containerId, string workspaceTypeName)
        {
            ViewBag.containerId = containerId;
            ViewBag.structureId = structureId;
            ViewBag.workspaceTypeName = workspaceTypeName;

            return PartialView();
        }
    }
}