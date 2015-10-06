using System.Web.Mvc;

namespace Site.Controllers
{
    /// <summary>
    /// Page controller
    /// </summary>
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
        public ViewResult AutoBoosting()
        {
            return View("~/Views/InfluenceSearchResults/AutoBoosting.cshtml");
        }
        public ViewResult BoostingWithWeights()
        {
            return View("~/Views/InfluenceSearchResults/BoostingWithWeights.cshtml");
        }
        public ViewResult BoostingWithFilters()
        {
            return View("~/Views/InfluenceSearchResults/BoostingWithFilters.cshtml");
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