using System.ComponentModel.Composition;
using System.Web.Mvc;
using CodeGarten.Data.Access;
using CodeGarten.Service;
using CodeGarten.Service.Utils;
using CodeGarten.Service.Interfaces;
using System.Linq;

namespace SVN.Controllers
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
            var dataBaseManager = (DataBaseManager)HttpContext.Items["DataBaseManager"];

            var workspace = dataBaseManager.WorkSpaceType.Get(structureId, workspaceTypeName);
            var container = dataBaseManager.Container.Get(containerId);
            var enroll = dataBaseManager.User.Get(User.Identity.Name).Enrolls.SingleOrDefault(e => e.ContainerId == containerId);

            ViewBag.enroll = enroll;
            ViewBag.InstanceName = container.UniqueInstanceName(workspace);

            if (enroll != null)
            {
                ViewBag.Permissions =
                    container.Type.Bindings.Single(b => b.WorkSpaceTypeName == workspaceTypeName).Roles.Where(
                        r => r.RoleTypeName == enroll.RoleTypeName).SelectMany(r => r.Rules).SelectMany(
                            ru => ru.Permissions).Where(p => p.ServiceName == Service.ServiceModel.Name);
            }

            return PartialView(Service.ServiceModel);
        }
    }
}