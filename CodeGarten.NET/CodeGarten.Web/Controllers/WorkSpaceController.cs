using System.Web.Mvc;
using CodeGarten.Data.Access;

namespace CodeGarten.Web.Controllers
{
    [Authorize]
    public sealed class WorkSpaceController : Controller
    {
        public ActionResult Index(long structureId, long containerId, string workSpaceTypeName, string serviceName)
        {

            var dataBaseManager = (DataBaseManager)HttpContext.Items["DataBaseManager"];
            
            ViewBag.Container = dataBaseManager.Container.Get(containerId);
            ViewBag.WorkSpaceType = dataBaseManager.WorkSpaceType.Get(structureId, workSpaceTypeName);
            ViewBag.StructureId = structureId;
            ViewBag.ServiceName = serviceName;
            return View();
        }

    }
}
