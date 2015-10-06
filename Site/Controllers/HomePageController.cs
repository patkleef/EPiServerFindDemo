using System.Web.Mvc;
using EPiServer.Web.Mvc;
using Site.Models.Pages;

namespace Site.Controllers
{
    public class HomePageController : PageController<HomePage>
    {
        public ActionResult Index(HomePage currentPage)
        {
            return View(currentPage);
        }
    }
}