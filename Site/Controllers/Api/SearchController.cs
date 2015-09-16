using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Http;
using EPiServer.Find;
using EPiServer.Find.Api;
using EPiServer.Find.Cms;
using EPiServer.Find.Framework;
using EPiServer.Find.Framework.Statistics;
using EPiServer.Find.Helpers.Text;
using EPiServer.Find.Statistics;
using EPiServer.Find.UnifiedSearch;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
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

        public SearchController()
        {
            _client = SearchClient.Instance;
            _urlResolver = ServiceLocator.Current.GetInstance<UrlResolver>();
        }

        // GET api/<controller>
        [HttpGet]
        [Route("api/Search/")]
        public IHttpActionResult Search(string query)
        {
            var hitSpecification = new HitSpecification();

            var result = _client.UnifiedSearchFor(query)
                .Track()
                .Take(100)
                .GetResult(hitSpecification, false);
            
            return Json(result);
        }

        // GET api/<controller>
        [HttpGet]
        [Route("api/TypedSearch/")]
        public IHttpActionResult TypedSearch(string query)
        {
            var trackQueryResult = SearchClient.Instance.Statistics().TrackQuery(query);

            var result = _client.Search<ArticlePage>()
                .For(query)
                .GetContentResult();

            return Json(result.Items.Select((x, i) => new
            {
                Title = x.PageName,
                Url = GetTrackingUrl(x, query, trackQueryResult.TrackId, i)
            }));
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
            var tags = string.Format("language:{0},siteid:{1}",
                Thread.CurrentThread.CurrentUICulture,
                SiteDefinition.Current.Id);

            return string.Format("{0}?_t_id={1}&_t_q={2}&_t_tags={3}&_t_ip={4}&_t_hit.id={5}&_t_hit.pos={6}",
                _urlResolver.GetUrl(page),
                trackId,
                query,
                tags,
                string.Empty,
                _client.Conventions.TypeNameConvention.GetTypeName(page.GetType()) + "/" + _client.Conventions.IdConvention.GetId(page),
                (index + 1));
        }

        #region Searching statistics

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

        #region Influence search results

        [HttpGet]
        [Route("api/Search/BestBets")]
        public IHttpActionResult BestBets(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return Json(string.Empty);
            }
            var result = _client.UnifiedSearchFor(query).ApplyBestBets().GetResult(new HitSpecification(), false);

            return Json(result);
        }

        [HttpGet]
        [Route("api/Search/Synonyms")]
        public IHttpActionResult Synonyms(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return Json(string.Empty);
            }
            var result = _client.UnifiedSearchFor(query).UsingSynonyms().GetResult(new HitSpecification(), false);

            return Json(result);
        }

        //[HttpGet]
        //[Route("api/Search/MoreLike")]
        //public IHttpActionResult MoreLike(int articleId)
        //{
        //    var articlePage = _client.Search<ArticlePage>().Filter(x => x.ContentLink.ID.Match(articleId)).GetContentResult().FirstOrDefault();

        //    if (articlePage != null)
        //    {
        //        var result = _client.Search<ArticlePage>()
        //            .MoreLike(articlePage.PageName)
        //            .MinimumTermFrequency(1)
        //            //The minimum term frequency below which the terms will be ignored from the input document. Defaults to 2
        //            .MaximumQueryTerms(12)
        //            //The maximum number of query terms that will be selected. Increasing this value gives greater accuracy at the expense of query execution speed. Defaults to 25.
        //            .Select(a => new
        //            {
        //                Title = a.PageName
        //            })
        //            .GetResult();

        //        return Json(result);
        //    }
        //    return Json(string.Empty);
        //}

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
            var article = _client.Search<ArticlePage>().Filter(x => x.ContentLink.ID.Match(articleId)).GetContentResult().FirstOrDefault();

            if (article != null)
            {
                var result = _client.Search<ArticlePage>()
                .MoreLike(article.BodyText.ToString())
                .MinimumDocumentFrequency(minimumDocumentFrequency)
                .MaximumDocumentFrequency(maximumDocumentFrequency)
                .PercentTermsToMatch(percentTermsToMatch)
                .MinimumTermFrequency(minimumTermFrequency)
                .MinimumWordLength(minimumWordLength)
                .MaximumWordLength(maximumWordLength)
                .MaximumQueryTerms(maximumQueryTerms)
                .StopWords((!string.IsNullOrEmpty(stopWords) ? stopWords.Split(',') : Enumerable.Empty<string>()))

                //.MinimumTermFrequency(4) //The minimum term frequency below which the terms will be ignored from the input document. Defaults to 2
                //.MaximumQueryTerms(12) //The maximum number of query terms that will be selected. Increasing this value gives greater accuracy at the expense of query execution speed. Defaults to 25.
                .Filter(x => !x.ContentLink.ID.Match(articleId))
                .Select(a => new
                {
                    Title = a.PageName
                })
                .GetResult();

                return Json(result);
            }
            return Json(string.Empty);
        }

        [HttpGet]
        [Route("api/Search/Boosting")]
        public IHttpActionResult Boosting(string query)
        {
            var result = _client.UnifiedSearch().For(query)
                .UsingUnifiedWeights(new UnifiedWeightsValues
                {
                    SearchTitle = 1,
                    
                }).GetResult();

            return Json(result);
        }

        #endregion

        #region Highlight

        [HttpGet]
        [Route("api/Search/Highlight")]
        public IHttpActionResult Highlight(
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
            hitSpecification.ExcerptLength = 100;
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

        [HttpGet]
        [Route("api/Search/HighlightTypedSearch")]
        public IHttpActionResult HighlightTypedSearch(string query)
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
                .GetResult();

            return Json(result);
        }
    }
}