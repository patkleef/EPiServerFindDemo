using System;
using System.Linq;
using EPiServer.Find;
using EPiServer.Find.Api.Facets;
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
            //q = TextSearchExample(q);
            //q = FilterOnPrice(q);
            //q = FilterOnRating(q);
            //q = FilterOnCoordinates(q);

            q = FilterFacetOnPrice(q);

            var result = q.GetResult();
            OutputResults(result);

            Console.ReadLine();
        }

        /// <summary>
        /// Basic text search in all fields for a given query
        /// </summary>
        /// <param name="cli"></param>
        static ITypeSearch<Hotel> TextSearchExample(ITypeSearch<Hotel> q)
        {
            Console.Write("What should we search for? ");
            string query = Console.ReadLine();
            return q.For(query);
        }

        /// <summary>
        /// Get hotels that have price (PriceUSD) between 100 and 200
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        static ITypeSearch<Hotel> FilterOnPrice(ITypeSearch<Hotel> q)
        {
            return q.Filter(x => x.PriceUSD.InRange(100, 200));
        }

        /// <summary>
        /// Get hotels that have price (PriceUSD) between 100 and 200
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        static ITypeSearch<Hotel> FilterFacetOnPrice(ITypeSearch<Hotel> q)
        {
            return q.FilterFacet("Price filter", x => x.PriceUSD.InRange(100, 200));
        }

        /// <summary>
        /// Get hotels that have rating (StarRating) greater than 4
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        static ITypeSearch<Hotel> FilterOnRating(ITypeSearch<Hotel> q)
        {
            return q.Filter(x => x.StarRating.GreaterThan(4));
        }

        /// <summary>
        /// Get hotels within 5 kilometers (GeoCoordinates) of the geo coordinates of the Cosmopolitan hotel
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        static ITypeSearch<Hotel> FilterOnCoordinates(ITypeSearch<Hotel> q)
        {
            return q.Filter(x => x.GeoCoordinates.WithinDistanceFrom(
                COSMOPOLITAN_HOTEL_GEO, 5.Kilometers()));
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
                Console.WriteLine("\tType: {0}", h.Document.PropertyType);
                Console.WriteLine("\t{0}", string.Join(",", h.Document.Features.Take(5).ToArray()));
                Console.WriteLine("\n");
            }

        }
    }
}
