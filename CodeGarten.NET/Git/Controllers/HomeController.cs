using System.ComponentModel.Composition;
using System.Web.Mvc;
using CodeGarten.Data.Access;
using CodeGarten.Service;
using CodeGarten.Service.Interfaces;
using CodeGarten.Service.Utils;

namespace Git.Controllers
{
    [Authorize]
    [Export(typeof(ServiceController)), ExportMetadata("ControllerName", "Home"),
     PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class HomeController : ServiceController, IServiceEntryPoint
    {
        public ActionResult Index(long structureId, long containerId, string workspaceTypeName)
        {
            ViewBag.containerId = containerId;
            ViewBag.workspaceTypeName = workspaceTypeName;

            var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

            var container = dataBaseManager.Container.Get(containerId);
            var workspace = dataBaseManager.WorkSpaceType.Get(structureId, workspaceTypeName);

            var repository = ((Git)Service).FileSystem[container.UniqueInstanceName(workspace)];

            return PartialView(repository.Head.ObjectId != null);
        }
    }
}