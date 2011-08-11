using System.ComponentModel.Composition;
using System.Web.Mvc;
using CodeGarten.Service.Interfaces;

namespace SVN.Controllers
{
    [Authorize]
    [Export(typeof (IController)), ExportMetadata("ControllerName", "Home"),
     PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class HomeController : Controller, IServiceEntryPoint
    {
        public ActionResult Index(long structureId, long containerId, string workspaceTypeName)
        {
            ViewBag.containerId = containerId;
            ViewBag.workspaceTypeName = workspaceTypeName;

            return PartialView("~/Services/Git.dll/Git/Views/Home/Index.cshtml");
        }
    }
}