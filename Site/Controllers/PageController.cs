using System.Web.Mvc;

namespace Site.Controllers
{
    public class PageController : Controller
    {
        public ViewResult AutoComplete()
        {
            return View("~/Views/Statistics/AutoComplete.cshtml");
        }
        public ViewResult DidYouMean()
        {
            return View("~/Views/Statistics/DidYouMean.cshtml");
        }
        public ViewResult SpellCheck()
        {
            return View("~/Views/Statistics/SpellCheck.cshtml");
        }

        public ViewResult BestBets()
        {
            return View("~/Views/InfluenceSearchResults/BestBets.cshtml");
        }
        public ViewResult Synonyms()
        {
            return View("~/Views/InfluenceSearchResults/Synonyms.cshtml");
        }
        public ViewResult Boosting()
        {
            return View("~/Views/InfluenceSearchResults/Boosting.cshtml");
        }

        public ViewResult MoreLike()
        {
            return View("~/Views/MoreLike/Index.cshtml");
        }

        public ViewResult HighlightingUnifiedSearch()
        {
            return View("~/Views/Highlighting/HighlightingUnifiedSearch.cshtml");
        }
        public ViewResult HighlightingTypedSearch()
        {
            return View("~/Views/Highlighting/HighlightingTypedSearch.cshtml");
        }
        public ViewResult TypedSearch()
        {
            return View("~/Views/TypedSearch/Index.cshtml");
        }
    }
}