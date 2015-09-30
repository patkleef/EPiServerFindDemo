using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using EPiServer.Find.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Site
{
    public class EPiServerApplication : EPiServer.Global
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        protected override void RegisterRoutes(RouteCollection routes)
        {
            base.RegisterRoutes(routes);

            routes.MapRoute(
                     "Statistics-Autocomplete",
                     "Statistics/AutoComplete",
                     new { controller = "Page", action = "AutoComplete" }
                 );
            routes.MapRoute(
                     "Statistics-DidYouMean",
                     "Statistics/DidYouMean",
                     new { controller = "Page", action = "DidYouMean" }
                 );
            routes.MapRoute(
                     "Statistics-SpellCheck",
                     "Statistics/SpellCheck",
                     new { controller = "Page", action = "SpellCheck" }
                 );

            routes.MapRoute(
                     "InfluenceSearchResults-BestBets",
                     "InfluenceSearchResults/BestBets",
                     new { controller = "Page", action = "BestBets" }
                 );
            routes.MapRoute(
                     "InfluenceSearchResults-Synonyms",
                     "InfluenceSearchResults/Synonyms",
                     new { controller = "Page", action = "Synonyms" }
                 );
            routes.MapRoute(
                     "InfluenceSearchResults-AutoBoosting",
                     "InfluenceSearchResults/AutoBoosting",
                     new { controller = "Page", action = "AutoBoosting" }
                 );
            routes.MapRoute(
                     "InfluenceSearchResults-BoostingWithWeights",
                     "InfluenceSearchResults/BoostingWithWeights",
                     new { controller = "Page", action = "BoostingWithWeights" }
                 );
            routes.MapRoute(
                     "InfluenceSearchResults-BoostingWithFilters",
                     "InfluenceSearchResults/BoostingWithFilters",
                     new { controller = "Page", action = "BoostingWithFilters" }
                 );

            routes.MapRoute(
                     "MoreLike",
                     "MoreLike",
                     new { controller = "Page", action = "MoreLike" }
                 );
            routes.MapRoute(
                     "Highlighting-UnifiedSearch",
                     "Highlighting/UnifiedSearch",
                     new { controller = "Page", action = "HighlightingUnifiedSearch" }
                 );
            routes.MapRoute(
                     "Highlighting-TypedSearch",
                     "Highlighting/TypedSearch",
                     new { controller = "Page", action = "HighlightingTypedSearch" }
                 );
            routes.MapRoute(
                     "TypedSearch",
                     "TypedSearch",
                     new { controller = "Page", action = "TypedSearch" }
                 );
        }
    }

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
        }
    }
}