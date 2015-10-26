using System;
using System.Linq;
using EPiServer.Find;
using EPiServer.Find.Api.Facets;
using EPiServer.Find.Api.Querying.Filters;
using EPiServer.Sample.Hotels;

namespace Lab
{
    class Program
    {
        private static readonly GeoLocation COSMOPOLITAN_HOTEL_GEO = new GeoLocation(36.109816, -115.173695);

        static void Main(string[] args)
        {
            var client = HotelHelpers.HotelClient;

            var q = client.Search<Hotel>();
            //q = FilterOnPrice(q);
            //q = FilterOnRating(q);
            //q = FilterOnLocation(q);
            //q = AdvancedFiltering(q);
            //q = RangeFacets(q);
            //q = FacetForCountry(q);
            //q = BasicTextSearch(q);
            
            var result = q.GetResult();

            OutputResults(result);
        }

        /// <summary>
        /// Exercise 1: Filter on price -> Get all hotels that have price between 100 and 200
        /// </summary>
        /// <param name="q"></param>
        static ITypeSearch<Hotel> FilterOnPrice(ITypeSearch<Hotel> q)
        {
            return q.Filter(x => x.PriceUSD.InRange(100, 200));
        }

        /// <summary>
        /// Exercise 2: Filter on rating/review -> Find all hotels that have a star rating of either 4 o 5, OR
        /// review rate of 9 or 10 with more than 50 reviews
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        static ITypeSearch<Hotel> FilterOnRating(ITypeSearch<Hotel> q)
        {
            return q.Filter(x => x.StarRating.InRange(4, 5))
                .OrFilter(x => x.ReviewCount.GreaterThan(50) & x.Ratings.Overall.InRange(8, 9));
        }

        /// <summary>
        /// Exercise 3: Filter on location -> Get all hotels within 5 km of the cosmopolitan hotel,
        /// order by distance from cosmopolitan hotel (closest first)
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        static ITypeSearch<Hotel> FilterOnLocation(ITypeSearch<Hotel> q)
        {
            return q.Filter(x => x.GeoCoordinates.WithinDistanceFrom(COSMOPOLITAN_HOTEL_GEO, 5.Kilometer()))
                    .OrderBy(x => x.GeoCoordinates).DistanceFrom(COSMOPOLITAN_HOTEL_GEO);
        }

        /// <summary>
        /// Exercise 4: Advanced filtering -> Show hotels with more than 2 stars within 10 km of the cosmopolitan hotel
        /// that offer room service, have air condition and are of chain specified by user
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        static ITypeSearch<Hotel> AdvancedFiltering(ITypeSearch<Hotel> q)
        {
            return q.Filter(x => x.StarRating.GreaterThan(2))
                .Filter(x => x.GeoCoordinates.WithinDistanceFrom(COSMOPOLITAN_HOTEL_GEO, 10.Kilometer()))
                .Filter(x => x.Features.MatchCaseInsensitive("Air conditioned"))
                .Filter(x => x.Features.MatchCaseInsensitive("Room service"));
        }

        /// <summary>
        /// Exercise 5: Range facets -> create range facets for price ranges 20-50, 51-100 and 101-150 USD
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        static ITypeSearch<Hotel> RangeFacets(ITypeSearch<Hotel> q)
        {
            var ranges = new[] {new NumericRange(20, 50), new NumericRange(51, 100), new NumericRange(101, 150)};

            return q.RangeFacetFor(x => x.PriceUSD, ranges);
        }

        /// <summary>
        /// Exercise 6: Facet for country -> List name of all countries that have hotels
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        static ITypeSearch<Hotel> FacetForCountry(ITypeSearch<Hotel> q)
        {
            return q.TermsFacetFor(x => x.Location.Country.Title, request => request.Size = 500).Take(0);
        }

        /// <summary>
        /// Exercise 7: Basic text search, query entered by user, in fields Name and Description
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        static ITypeSearch<Hotel> BasicTextSearch(ITypeSearch<Hotel> q)
        {
            Console.Write("What should we search for? ");
            string query = Console.ReadLine();
            return q.For(query).InFields(x => x.Name, x => x.Description);
        }

        /// <summary>
        /// Helper method that outputs both facets and results
        /// </summary>
        /// <param name="res"></param>
        static void OutputResults(SearchResults<Hotel> res)
        {
            Console.WriteLine("RESULTS Showing {0} out of {1}. Search took {2} ms", res.Hits.Count(), res.TotalMatching, res.ProcessingInfo.ServerDuration);
            Console.WriteLine("--------------------------------------------------------------");
            if (res.Facets != null)
            {
                foreach (var f in res.Facets)
                {
                    Console.WriteLine("Facet: {0}", f.Name);
                    if (f is TermsFacet)
                    {
                        foreach (var t in (f as TermsFacet).Terms)
                        {
                            Console.WriteLine("\t{0} ({1})", t.Term, t.Count);
                        }
                    }
                    else if (f is FilterFacet)
                    {
                        Console.WriteLine("\tMatching: {0}", (f as FilterFacet).Count);
                    }
                    else if (f is StatisticalFacet)
                    {
                        var sf = f as StatisticalFacet;
                        Console.WriteLine("\tMean: {0}\n\tMax: {1}\n\tMin: {2}\n\tTotal: {3}\n\tVariance: {4}\n", sf.Mean, sf.Max, sf.Min, sf.Total, sf.Variance);
                    }
                    else if (f is NumericRangeFacet)
                    {
                        foreach (var r in (f as NumericRangeFacet).Ranges)
                        {
                            Console.WriteLine("\t{0}-{1} ({2})", r.From, r.To, r.Count);
                        }
                    }
                    else if (f is HistogramFacet)
                    {
                        foreach (var r in (f as HistogramFacet).Entries)
                        {
                            Console.WriteLine("\t{0} ({1})", r.Key, r.Count);
                        }
                    }
                    else if (f is GeoDistanceFacet)
                    {
                        foreach (var r in (f as GeoDistanceFacet).Ranges)
                        {
                            Console.WriteLine("\t{0}-{1} ({2})", r.From.Value, r.To.Value, r.TotalCount);
                        }
                    }
                    Console.WriteLine();
                }
            }
            Console.WriteLine();
            Console.WriteLine("Hits: ");
            foreach (var h in res.Hits)
            {
                Console.WriteLine("\t{0}", h.Document.Title.ToUpper());
                Console.WriteLine("\t{0}", h.Document.LocationsString);
                Console.WriteLine("\tPrice: {0}", h.Document.PriceUSD);
                Console.WriteLine("\tRating: {0}", h.Document.StarRating);
                Console.WriteLine("\tReview count: {0}", h.Document.ReviewCount);
                Console.WriteLine("\tOverall rating: {0}", h.Document.Ratings.Overall);
                Console.WriteLine("\tCountry: {0}", h.Document.Location.Country.Title);
                Console.WriteLine("\tType: {0}", h.Document.PropertyType);
                Console.WriteLine("\t{0}", string.Join(",", h.Document.Features.ToArray()));
                Console.WriteLine("\n");
            }
            Console.ReadLine();
        }
    }
}
