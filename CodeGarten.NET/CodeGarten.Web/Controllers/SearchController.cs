using System.Linq;
using System.Web.Mvc;
using CodeGarten.Data.Access;

namespace CodeGarten.Web.Controllers
{
    public class SearchController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public PartialViewResult Index(string query, string type)
        {
            var dataBaseManager = HttpContext.Items["DataBaseManager"] as DataBaseManager;

            switch (type)
            {
                case "user":
                    return PartialView("_SearchResults", dataBaseManager.User.Search(query));
                case "structure":
                    return PartialView("_SearchResults", dataBaseManager.Structure.Search(query).Where(s => s.Public && !s.Developing));
                case "project":
                    return PartialView("_SearchResults", dataBaseManager.Container.Search(query).Where(c => c.Prototype.Structure.Public));
                case "all":
                    {
                        ViewBag.Users = dataBaseManager.User.Search(query);
                        ViewBag.Structures = dataBaseManager.Structure.Search(query).Where(s => s.Public && !s.Developing);
                        ViewBag.Projects = dataBaseManager.Container.Search(query).Where(c => c.Prototype.Structure.Public);
                        return PartialView("_SearchAllResults");
                    }
            }
            return null;
        }
    }
}