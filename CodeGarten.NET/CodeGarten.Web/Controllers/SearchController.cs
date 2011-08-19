using System.Linq;
using System.Web.Mvc;
using CodeGarten.Data.Access;

namespace CodeGarten.Web.Controllers
{
    [Authorize]
    public sealed class SearchController : Controller
    {
        public ActionResult Index(string search, string type)
        {
            if (search == null)
                return View();

            var dataBaseManager = (DataBaseManager)HttpContext.Items["DataBaseManager"];

            switch (type)
            {
                case "user":
                    {
                        return User(search);
                    }
                case "structure":
                    {
                        return Structure(search);
                    }
                case "project":
                    {
                        return Project(search);
                    }
                default:
                    {
                        ViewBag.Users = dataBaseManager.User.Search(search);
                        ViewBag.Structures = dataBaseManager.Structure.Search(search).Where(s => s.Public && !s.Developing);
                        ViewBag.Projects = dataBaseManager.Container.Search(search).Where(c => c.Prototype.Structure.Public);
                        ViewBag.MixedSearch = true;
                        var ajax = Request.IsAjaxRequest();
                        return Request.IsAjaxRequest() ? (ActionResult)PartialView("_SearchAllResults") : View();
                    }
            }
        }

        [NonAction]
        public new ActionResult User(string search)
        {
            var dataBaseManager = (DataBaseManager)HttpContext.Items["DataBaseManager"];

            var results = dataBaseManager.User.Search(search);
            return Request.IsAjaxRequest() ? (ActionResult)PartialView("_SearchResults", results) : View("Index", results);
        }

        [NonAction]
        public ActionResult Structure(string search)
        {
            var dataBaseManager = (DataBaseManager)HttpContext.Items["DataBaseManager"];

            var results = dataBaseManager.Structure.Search(search).Where(s => s.Public && !s.Developing);
            return Request.IsAjaxRequest() ? (ActionResult)PartialView("_SearchResults", results) : View("Index", results);
        }

        [NonAction]
        public ActionResult Project(string search)
        {
            var dataBaseManager = (DataBaseManager)HttpContext.Items["DataBaseManager"];

            var results = dataBaseManager.Container.Search(search).Where(c => c.Prototype.Structure.Public);
            return Request.IsAjaxRequest() ? (ActionResult)PartialView("_SearchResults", results) : View("Index", results);
        }
    }
}