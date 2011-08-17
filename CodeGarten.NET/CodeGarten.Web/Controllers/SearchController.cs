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
                        var results = dataBaseManager.User.Search(search);
                        return Request.IsAjaxRequest() ? (ActionResult)PartialView("_SearchResults", results) : View(results);
                    }
                case "structure":
                    {
                        var results = dataBaseManager.Structure.Search(search).Where(s => s.Public && !s.Developing);
                        return Request.IsAjaxRequest() ? (ActionResult)PartialView("_SearchResults", results) : View(results);
                    }
                case "project":
                    {
                        var results = dataBaseManager.Container.Search(search).Where(c => c.Prototype.Structure.Public);
                        return Request.IsAjaxRequest() ? (ActionResult)PartialView("_SearchResults", results) : View(results);
                    }
                default:
                    {
                        ViewBag.Users = dataBaseManager.User.Search(search);
                        ViewBag.Structures = dataBaseManager.Structure.Search(search).Where(s => s.Public && !s.Developing);
                        ViewBag.Projects = dataBaseManager.Container.Search(search).Where(c => c.Prototype.Structure.Public);
                        ViewBag.MixedSearch = true;
                        return Request.IsAjaxRequest() ? (ActionResult)PartialView("_SearchAllResults") : View();
                    }
            }
        }
    }
}