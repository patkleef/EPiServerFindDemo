using System.Web.Mvc;
using EPiServer.Find.Framework;
using EPiServer.Web.Mvc;
using Site.Models.Pages;

namespace Site.Controllers
{
    public class HomePageController : PageController<HomePage>
    {
        public ActionResult Index(HomePage currentPage)
        {
            var client = SearchClient.Instance;

            //var users = client.Search<User>().GetResult();

            //var user1 = client.Search<User>().OrFilter(u => u.Name.Match("Kurtis Weissnat")).OrFilter(u => u.Company.Name.Match("Deckow-Crist")).GetResult();

            //var user2 = client.Search<User>().Filter(u => u.Name.Prefix("K")).GetResult();


            // // Specify highlight settings
            //var hitSpecification = new HitSpecification { HighlightExcerpt = true, HighlightTitle = true };
 
            //// Search
            //var searchResult = SearchClient.Instance.UnifiedSearchFor("Lura@rod.tv").GetResult(hitSpecification);
        

            /* Implementation of action. You can create your own view model class that you pass to the view or
             * you can pass the page type for simpler templates */

            return View(currentPage);
        }
    }
}