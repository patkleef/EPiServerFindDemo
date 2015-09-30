using System;
using System.Web.Mvc;
using EPiServer.Find;
using EPiServer.Find.Cms;
using EPiServer.Find.Framework;
using EPiServer.Web.Mvc;
using Site.Models.Pages;

namespace Site.Controllers
{
    public class HomePageController : PageController<HomePage>
    {
        public ActionResult Index(HomePage currentPage)
        {
            var result4 =
                 SearchClient.Instance.UnifiedSearch().For("to").UsingAutoBoost().GetResult();
                 //SearchClient.Instance.Search<ArticlePage>().For("to").GetContentResult();

           // var result2 =
               // SearchClient.Instance.Search<ArticlePage>().For("to").UsingAutoBoost().GetContentResult();

            return View(currentPage);
        }
    }
}