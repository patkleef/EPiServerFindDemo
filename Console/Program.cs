using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Find;
using EPiServer.Find.Api.Facets;
using Newtonsoft.Json;

namespace FindDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = Client.CreateFromConfig();

            #region Indexing

            /************* INDEXING *****************/

            //Importer.ClearIndex(client);

            //Importer.AddDemoContentFromFiles(client);

            #endregion

            #region Filtering

            /************* FILTERING *****************/
            var query = client.Search<Product>();

            //query = FilterString(query);

            //query = FilterNumerical(query);

            //query = FilteringComplexCollections(query);

            query = FiltersCombinedExample(query);


            //Console.WriteLine("What sizes should we filter on? (Use ',' to separate.) ");
            //string sizes = Console.ReadLine();
            //if (string.IsNullOrEmpty(sizes) == false)
            //{
            //    query = FilterUsingBuildFilter(client, query, sizes.Split(',').ToList<string>());
            //}

            /************* FILTERING END *****************/

            #endregion

            #region Facets

            query = FacetsExample(query);

            #endregion

            var result = query.GetResult();

            ShowResults(result);
        }


        #region Filter methods

        /// <summary>
        /// Example: Starts with "Blizzard", use Name field
        /// Match, Prefix, AnyWordBeginsWith
        /// The AnyWordBeginsWith method matches strings which contains any word that starts with a given string,
        /// making it suitable for autocomplete. It does not case about casing.
        /// NOTE: While AnyWordBeginsWith is powerful it is not optimal in terms of performance when used for large strings.
        /// Be careful!
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        private static ITypeSearch<Product> FilterString(ITypeSearch<Product> q)
        {
            return q.Filter(p => p.Name.PrefixCaseInsensitive("Blizzard"));
        }

        /// <summary>
        /// Example: Products in price range  10-50
        /// Match, InRange, Exists
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        private static ITypeSearch<Product> FilterNumerical(ITypeSearch<Product> q)
        {
            return q.Filter(p => p.Price.InRange(10, 50));
        }

        /// <summary>
        /// Example: This year and next years collection, use AvailableFrom field and OrFilter
        /// Match, InRange, Exists, MatchYear and more
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        private static ITypeSearch<Product> FilterDateTime(ITypeSearch<Product> q)
        {
            return q;
        }


        /// <summary>
        /// Example: All products in Size XL
        /// Using MatchContained on the complex object OR using Sizes collection
        /// Note: While it is possible to filter on several fields in objects in collections using MatchContained
        /// several times it is not possible to specify that those conditions should apply to the same object in the list.
        /// That is: We can find size S and items with stock larger than 2, we cannot require that those conditions should apply to the same object.
        /// In practice this limitation can often be worked around by filtering on some unique value of the objects in the list.
        /// Also note that while MatchContained works for filtering on complex objects it's best practice to "denormalize" indexed objects by including
        /// fields at indexing time to make querying less complex.
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        private static ITypeSearch<Product> FilteringComplexCollections(ITypeSearch<Product> q)
        {
            return q.Filter(p => p.Skus.MatchContained(sku => sku.Size, "XL"));
            //return q.Filter(p => p.Sizes.Match("XL"));
        }




        /// <summary>
        /// Example: All womens jeans that are not sold out, order by price, then by name
        /// Using filters on Enum, collections and boolean
        /// When using Match on a list of strings we require that at least one of the strings in the list matches the value specified.
        /// Note: OrderBy orders null values last while OrderByDecending orders them first. 
        /// Use the second argument of type SortMissing to override the default behavior
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        private static ITypeSearch<Product> FiltersCombinedExample(ITypeSearch<Product> q)
        {
            return q
                .Filter(p => p.Collection.Match(Collection.Jeans))
                .Filter(p => p.Gender.Match(Gender.Womens))
                .Filter(p => p.InStock.Match(true))
                .OrderBy(p => p.Price)
                .ThenBy(p => p.Name);
        }

        /// <summary>
        /// Sometimes, especially when reacting to user input filter has to be dynamically composed.
        /// For this we can use the BuildFilter method.        
        /// </summary>
        /// <param name="client">The search client</param>
        /// <param name="q">Our query</param>
        /// <param name="sizesToFilter">The color we would like to filter on</param>
        /// <returns></returns>
        private static ITypeSearch<Product> FilterUsingBuildFilter(IClient client, ITypeSearch<Product> q, IEnumerable<string> sizesToFilter)
        {
            var sizeFilter = client.BuildFilter<Product>();
            foreach (var filterSize in sizesToFilter)
            {
                string size = filterSize;
                sizeFilter = sizeFilter.Or(x => x.Sizes.Match(size));
            }

            return q.Filter(sizeFilter);
        }

        #endregion

        #region Facet method
        /// <summary>
        /// Adds facets for specific parameters that gets aggregated across all results
        /// NB: Notice the difference in use of Filter and FilterHits. 
        /// Filters added using Filter are applied BEFORE calculating facets, while FilterHits are added AFTER calculating facets
        /// Filter is better performance wise.
        /// 
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        private static ITypeSearch<Product> FacetsExample(ITypeSearch<Product> q)
        {
            return q
                .TermsFacetFor(p => p.Sizes) //Size
                .TermsFacetFor(p => p.Color) //Color
                .RangeFacetFor(p => p.Price, new NumericRange(20, 50), new NumericRange(51, 100), new NumericRange(101, 500)) //Price
                .FilterFacet("Womens", p => p.Gender.Match(Gender.Womens))
                .FilterFacet("Jeans", p => p.Collection.Match(Collection.Jeans))
                .FilterFacet("Sold out", p => p.InStock.Match(false)); //Filterfacet
        }
        #endregion


        /// <summary>
        /// Helper method that outputs both facets and results
        /// </summary>
        /// <param name="res"></param>
        static void ShowResults(SearchResults<Product> res)
        {
            Console.WriteLine("RESULTS Showing {0} out of {1}. Search took {2} ms", res.Hits.Count(), res.TotalMatching, res.ProcessingInfo.ServerDuration);
            Console.WriteLine("--------------------------------------------------------------");

            #region print facet data

            if (res.Facets != null)
            {
                foreach (var f in res.Facets)
                {
                    Console.WriteLine("Facet: {0}", f.Name);
                    if (f is TermsFacet)
                    {
                        foreach (var t in (f as TermsFacet).Terms.OrderBy(t => t.Term))
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

            #endregion

            Console.WriteLine();
            Console.WriteLine("Hits: ");
            foreach (var p in res.Hits)
            {
                Console.WriteLine("\t{0} ({1})", p.Document.Name.ToUpper(), p.Document.VariantCode);
                Console.WriteLine("\tCollection: {0}", p.Document.Collection);
                Console.WriteLine("\tPrice: {0}", p.Document.Price);
                Console.WriteLine("\tSizes: {0}", string.Join(",", p.Document.Sizes.ToArray()));
                Console.WriteLine("\n");
            }

        }


        
    }
}
