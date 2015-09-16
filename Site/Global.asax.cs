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


            var config = GlobalConfiguration.Configuration;
            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new LowercaseContractResolver();
            config.Formatters.JsonFormatter.SerializerSettings = settings;
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
                     "InfluenceSearchResults-Boosting",
                     "InfluenceSearchResults/Boosting",
                     new { controller = "Page", action = "Boosting" }
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

    public class LowercaseContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToLower();
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