using System.ComponentModel.Composition;
using System.Linq;
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
            var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

            var container = dataBaseManager.Container.Get(containerId);
            var workspace = dataBaseManager.WorkSpaceType.Get(structureId, workspaceTypeName);
            var instanceName = container.UniqueInstanceName(workspace);
            var enroll = dataBaseManager.User.Get(User.Identity.Name).Enrolls.SingleOrDefault(e => e.ContainerId == containerId);

            var repository = ((Git)Service).FileSystem[instanceName];

            ViewBag.ServiceName = Service.ServiceModel.Name;
            ViewBag.ServiceDescription = Service.ServiceModel.Description;
            ViewBag.InstanceName = instanceName;
            ViewBag.User = dataBaseManager.User.Get(User.Identity.Name);
            ViewBag.Enroll = enroll;

            if(enroll != null)
            {
                ViewBag.Permissions =
                    container.Prototype.Bindings.Single(b => b.WorkSpaceTypeName == workspaceTypeName).Roles.Where(
                        r => r.RoleTypeName == enroll.RoleTypeName).SelectMany(r => r.Rules).SelectMany(
                            ru => ru.Permissions).Where(p => p.ServiceName == Service.ServiceModel.Name);
            }

            var repo = repository == null ? (bool?) null : repository.Head.ObjectId != null;

            return PartialView(repo);
        }
    }
}