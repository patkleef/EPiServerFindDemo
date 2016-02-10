using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Http;
using EPiServer.Find;
using EPiServer.Find.Api;
using EPiServer.Find.Api.Facets;
using EPiServer.Find.Cms;
using EPiServer.Find.Framework;
using EPiServer.Find.Framework.Statistics;
using EPiServer.Find.Helpers.Text;
using EPiServer.Find.Statistics;
using EPiServer.Find.UnifiedSearch;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using Site.Business.Content;
using Site.Models.Pages;

namespace Site.Controllers.Api
{
    /// <summary>
    /// Search api controller
    /// </summary>
    public class SearchController : ApiController
    {
        private readonly IClient _client;
        private readonly UrlResolver _urlResolver;
        private readonly IStatisticTagsHelper _statisticsTagsHelper;

        /// <summary>
        /// Public constructor
        /// </summary>
        public SearchController()
        {
            _client = SearchClient.Instance;
            _urlResolver = ServiceLocator.Current.GetInstance<UrlResolver>();
            _statisticsTagsHelper = ServiceLocator.Current.GetInstance<IStatisticTagsHelper>();
        }

        #region Unified Search

        /// <summary>
        /// Unified search
        /// http://world.episerver.com/documentation/Items/Developers-Guide/EPiServer-Find/10/DotNET-Client-API/Searching/Unified-search/
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Search/")]
        public IHttpActionResult Search(string query)
        {
            var hitSpecification = new HitSpecification();

            var result = _client.UnifiedSearchFor(query)
                .StatisticsTrack()
                .GetResult();

            return Json(result);
        }

        #endregion

        #region Typed search

        /// <summary>
        /// Typed search
        /// http://world.episerver.com/documentation/Items/Developers-Guide/EPiServer-Find/10/DotNET-Client-API/Searching/Searching/
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/TypedSearch/")]
        public IHttpActionResult TypedSearch(string query)
        {
            var result = _client.Search<ArticlePage>()
                .For(query)
                .GetContentResult();

            var trackId = new TrackContext().Id;
            TrackQuery(query, result.TotalMatching, trackId);

            return Json(result.Items.Select((x, i) => new
            {
                Title = x.PageName,
                Url = GetTrackingUrl(x, query, trackId, i)
            }));
        }

        /// <summary>
        /// Track query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="nrOfHits"></param>
        /// <param name="id"></param>
        public void TrackQuery(string query, int nrOfHits, string id)
        {
            SearchClient.Instance.Statistics().TrackQuery(query, x =>
            {
                x.Id = id;
                x.Tags = _statisticsTagsHelper.GetTags();
                x.Query.Hits = nrOfHits;
            });
        }

        /// <summary>
        /// Get tracking url
        /// </summary>
        /// <param name="page"></param>
        /// <param name="query"></param>
        /// <param name="trackId"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private string GetTrackingUrl(ArticlePage page, string query, string trackId, int index)
        {
            var tags = _statisticsTagsHelper.GetTags().Concatenate(",");

            return string.Format("{0}?_t_id={1}&_t_q={2}&_t_tags={3}&_t_ip={4}&_t_hit.id={5}&_t_hit.pos={6}",
                _urlResolver.GetUrl(page),
                trackId,
                query,
                tags,
                string.Empty,
                _client.Conventions.TypeNameConvention.GetTypeName(page.GetType()) + "/" + _client.Conventions.IdConvention.GetId(page),
                (index + 1));
        }

        #endregion

        #region Auto complete

        /// <summary>
        /// Auto complete
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Search/AutoComplete")]
        public IHttpActionResult AutoComplete(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return Json(string.Empty);
            }
            var result = _client.Statistics().GetAutocomplete(query);

            return Json(result);
        }

        #endregion

        #region Did you mean

        /// <summary>
        /// Did you mean
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Search/DidYouMean")]
        public IHttpActionResult DidYouMean(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return Json(string.Empty);
            }
            var result = _client.Statistics().GetDidYouMean(query);

            return Json(result);
        }

        #endregion

        #region Spell check

        /// <summary>
        /// Spell check
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Search/SpellCheck")]
        public IHttpActionResult SpellCheck(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return Json(string.Empty);
            }
            var result = _client.Statistics().GetSpellcheck(query);

            return Json(result);
        }

        #endregion

        #region Best bets

        /// <summary>
        /// Best bets
        /// http://world.episerver.com/documentation/Items/Developers-Guide/EPiServer-Find/10/DotNET-Client-API/Searching/Best-Bets/
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Search/BestBets")]
        public IHttpActionResult BestBets(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return Json(string.Empty);
            }
            var result = _client.UnifiedSearchFor(query)
                .ApplyBestBets()
                .GetResult(new HitSpecification());

            return Json(result);
        }

        #endregion

        #region Synonyms

        /// <summary>
        /// Synonyms
        /// http://world.episerver.com/documentation/Items/Developers-Guide/EPiServer-Find/10/DotNET-Client-API/Searching/Synonyms/
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Search/Synonyms")]
        public IHttpActionResult Synonyms(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return Json(string.Empty);
            }
            var result = _client.UnifiedSearchFor(query)
                .UsingSynonyms()
                .GetResult(new HitSpecification());

            return Json(result);
        }

        #endregion

        #region Auto boosting

        /// <summary>
        /// Auto boosting
        /// http://world.episerver.com/documentation/Items/Developers-Guide/EPiServer-Find/10/DotNET-Client-API/Searching/auto-boosting/
        /// </summary>
        /// <param name="query"></param>
        /// <param name="useAutoBoosting"></param>
        /// <param name="decayScale"></param>
        /// <param name="decayOffset"></param>
        /// <param name="decayShape"></param>
        /// <param name="decayOrigin"></param>
        /// <param name="decayMinimum"></param>
        /// <param name="hitBoostScale"></param>
        /// <param name="hitBoostOffset"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Search/AutoBoosting")]
        public IHttpActionResult AutoBoosting(
            string query,
            bool useAutoBoosting,
            int decayScale,
            int decayOffset,
            double decayShape,
            DateTime decayOrigin,
            double decayMinimum,
            double hitBoostScale,
            double hitBoostOffset)
        {
            if (!useAutoBoosting)
            {
                var result = _client.UnifiedSearch().For(query).GetResult();

                return Json(result);
            }
            else
            {
                var result = _client.UnifiedSearch().For(query)
                                .UsingAutoBoost
                                (
                                    TimeSpan.FromDays(decayScale),
                                    TimeSpan.FromDays(decayOffset),
                                    decayShape,
                                    decayMinimum,
                                    decayOrigin,
                                    hitBoostScale,
                                    hitBoostOffset
                                )
                                .GetResult();

                return Json(result);
            }
        }

        #endregion

        #region Boosting with weights

        /// <summary>
        /// Boosting weights
        /// http://world.episerver.com/documentation/Items/Developers-Guide/EPiServer-Find/10/DotNET-Client-API/Searching/Boosting-with-weights/
        /// </summary>
        /// <param name="query"></param>
        /// <param name="useBoostingWithWeights"></param>
        /// <param name="searchTitle"></param>
        /// <param name="searchText"></param>
        /// <param name="searchSummary"></param>
        /// <param name="searchAttachment"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Search/BoostingWithWeights")]
        public IHttpActionResult BoostingWithWeights(
            string query,
            bool useBoostingWithWeights,
            double searchTitle,
            double searchText,
            double searchSummary,
            double searchAttachment)
        {
            if (!useBoostingWithWeights)
            {
                var result = _client.UnifiedSearchFor(query).GetResult();

                return Json(result);
            }
            else
            {
                var weights = new UnifiedWeightsValues()
                {
                    SearchTitle = searchTitle,
                    SearchText = searchText,
                    SearchSummary = searchSummary,
                    SearchAttachment = searchAttachment
                };

                var result =
                    _client.UnifiedSearch().For(query)
                        .UsingUnifiedWeights(weights)
                        .GetResult();

                return Json(result);
            }
        }

        #endregion

        #region Boosting with filters

        /// <summary>
        /// Boosting with filters
        /// http://world.episerver.com/documentation/Items/Developers-Guide/EPiServer-Find/10/DotNET-Client-API/Searching/Boosting-with-filters/
        /// </summary>
        /// <param name="query"></param>
        /// <param name="useBoostingWithFilters"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="boostFactor"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Search/BoostingWithFilters")]
        public IHttpActionResult BoostingWithFilters(
            string query,
            bool useBoostingWithFilters,
            DateTime startDate,
            DateTime endDate,
            double boostFactor)
        {
            if (!useBoostingWithFilters)
            {
                var result = _client.UnifiedSearch().For(query).GetResult();

                return Json(result);
            }
            else
            {
                var result = _client.UnifiedSearchFor(query)
                                .BoostMatching(p => p.SearchUpdateDate.InRange(startDate, endDate), boostFactor)
                                .GetResult();

                return Json(result);
            }
        }

        #endregion

        #region More like/related

        /// <summary>
        /// Related articles
        /// http://world.episerver.com/documentation/Items/Developers-Guide/EPiServer-Find/10/DotNET-Client-API/Searching/More-LikeRelated/
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="minimumDocumentFrequency"></param>
        /// <param name="maximumDocumentFrequency"></param>
        /// <param name="percentTermsToMatch"></param>
        /// <param name="minimumTermFrequency"></param>
        /// <param name="minimumWordLength"></param>
        /// <param name="maximumWordLength"></param>
        /// <param name="maximumQueryTerms"></param>
        /// <param name="stopWords"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Search/RelatedArticles")]
        public IHttpActionResult RelatedArticles(
            int articleId,
            int minimumDocumentFrequency,
            int maximumDocumentFrequency,
            int percentTermsToMatch,
            int minimumTermFrequency,
            int minimumWordLength,
            int maximumWordLength,
            int maximumQueryTerms,
            string stopWords)
        {
            _client.Search<ArticlePage>()
                .MoreLike("Enjoy A Vacation In Phuket With Best Phuket Golf Courses")
                .GetContentResult();

            var article = _client.Search<ArticlePage>()
                                .Filter(x => x.ContentLink.ID.Match(articleId))
                                .GetContentResult().FirstOrDefault();

            if (article != null)
            {
                var result = _client.Search<ArticlePage>()
                .MoreLike(article.PageName)
                .MinimumDocumentFrequency(minimumDocumentFrequency)
                .MaximumDocumentFrequency(maximumDocumentFrequency)
                .PercentTermsToMatch(percentTermsToMatch)
                .MinimumTermFrequency(minimumTermFrequency)
                .MinimumWordLength(minimumWordLength)
                .MaximumWordLength(maximumWordLength)
                .MaximumQueryTerms(maximumQueryTerms)
                .StopWords((!string.IsNullOrEmpty(stopWords) ? stopWords.Split(',') : Enumerable.Empty<string>()))
                .Filter(x => !x.ContentLink.ID.Match(articleId))
                .Select(a => new
                {
                    Title = a.PageName,
                    Url = _urlResolver.GetUrl(a.ContentLink)
                })
                .Take(100)
                .GetResult();

                return Json(result);
            }
            return Json(string.Empty);
        }

        #endregion

        #region Highlighting unified search

        /// <summary>
        /// Highlight
        /// http://world.episerver.com/documentation/Items/Developers-Guide/EPiServer-Find/10/DotNET-Client-API/Searching/Highlightning/
        /// </summary>
        /// <param name="query"></param>
        /// <param name="numberOfFragments"></param>
        /// <param name="fragmentSize"></param>
        /// <param name="preTag"></param>
        /// <param name="postTag"></param>
        /// <param name="concatentation"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Search/HighlightUnifiedSearch")]
        public IHttpActionResult HighlightUnifiedSearch(
            string query,
            int numberOfFragments,
            int fragmentSize,
            string preTag,
            string postTag, 
            string concatentation)
        {
            var hitSpecification = new HitSpecification();
            hitSpecification.HighlightExcerpt = true;
            hitSpecification.HighlightTitle = true;
            if (!string.IsNullOrEmpty(preTag))
            {
                hitSpecification.PreTagForAllHighlights = preTag;
            }
            if (!string.IsNullOrEmpty(postTag))
            {
                hitSpecification.PostTagForAllHighlights = postTag;
            }
            hitSpecification.ExcerptHighlightSpecAction = x =>
            {
                if (fragmentSize > 0)
                {
                    x.FragmentSize = fragmentSize;
                }
                if (numberOfFragments > 0)
                {
                    x.NumberOfFragments = numberOfFragments;
                }
                if (!string.IsNullOrEmpty(concatentation))
                {
                    x.Concatenation = (IEnumerable<string> strings) => string.Join(concatentation, strings);
                }
            };

            var result = _client.UnifiedSearchFor(query)
                .GetResult(hitSpecification);

            return Json(result);
        }

        #endregion

        #region Highlighting typed search

        /// <summary>
        /// Highlight typed search
        /// http://world.episerver.com/documentation/Items/Developers-Guide/EPiServer-Find/10/DotNET-Client-API/Searching/Highlightning/
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Search/HighlightingTypedSearch")]
        public IHttpActionResult HighlightingTypedSearch(string query)
        {
            var result = _client.Search<ArticlePage>()
                .For(query)
                .Select(x => new
                {
                    Title = x.PageName.AsHighlighted(new HighlightSpec
                    {
                        PostTag = "</em>",
                        PreTag = "<em>",
                    }),
                    Text = x.BodyText.AsHighlighted(new HighlightSpec
                    {
                        FragmentSize = 30,
                        NumberOfFragments = 2,
                        PostTag  = "</em>",
                        PreTag = "<em>",
                        Concatenation = frag => frag.Concatenate(" ___ ")
                    })
                })
                .GetResult();

            return Json(result);
        }

        #endregion

        #region Nested queries

        [HttpGet]
        [Route("api/Search/CompanyCountryFacet")]
        public IHttpActionResult CompanyCountryFacet()
        {
            var result = _client.Search<Company>()
                .TermsFacetFor(c => c.Employees, c => c.Country)
                .HistogramFacetFor(c => c.Employees, c => c.Age, 10)
                .Take(100)
                .GetResult();

            var termsFacet = result.TermsFacetFor(c => c.Employees, c => c.Country);
            var histogramFacet = result.HistogramFacetFor(c => c.Employees, c => c.Age);
            
            return Json(new { Companies = result, Countries = termsFacet.Terms, Ages = histogramFacet.Entries});
        }

        [HttpGet]
        [Route("api/Search/SearchCompanies")]
        public IHttpActionResult SearchCompanies(string country, int? age)
        {
            var query = _client.Search<Company>();

            if (!string.IsNullOrEmpty(country))
            {
                query = query.Filter(c => c.Employees, c => c.Country.MatchCaseInsensitive(country));
                query = query.OrderBy(c => c.Employees, c => c.Name, c => c.Country.MatchCaseInsensitive(country));
            }
            if (age.HasValue)
            {
                var startNumber = age.Value;
                var endNumber = age.Value + 10;
                query = query.Filter(c => c.Employees, c => c.Age.InRange(startNumber, endNumber));

                if (!string.IsNullOrEmpty(country))
                {
                    query = query.ThenBy(c => c.Employees, c => c.Age, c => c.Age.InRange(startNumber, endNumber));
                }
                else
                {
                    query = query.OrderBy(c => c.Employees, c => c.Age, c => c.Age.InRange(startNumber, endNumber));
                }
            }
            var result = query.GetResult();

            return Json(result);
        }

        #endregion

        #region Others

        /// <summary>
        /// Get all article types
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Search/AllArticles")]
        public IHttpActionResult AllArticles()
        {
            var result = _client.Search<ArticlePage>()
                .Select(a => new
                {
                    Id = a.ContentLink.ID,
                    Title = a.PageName
                })
                .Take(100)
                .GetResult();

            return Json(result);
        }

        /// <summary>
        /// Get all companies
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Search/AllCompanies")]
        public IHttpActionResult AllCompanies()
        {
            var result = _client.Search<Company>()
                .Select(c => new
                {
                    c.Name
                })
                .Take(100)
                .GetResult();

            return Json(result);
        }

        #endregion
    }
}