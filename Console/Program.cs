using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text;
using EPiServer.Find;
using EPiServer.Find.Api;
using EPiServer.Find.Api.Facets;
using EPiServer.Find.Api.Ids;
using EPiServer.Find.Api.Querying.Filters;
using EPiServer.Find.ClientConventions;
using FindDemo.Models;

namespace FindDemo
{
    class Program
    {
        
        static void Main(string[] args)
        {
            var client = Client.CreateFromConfig();

            #region Conventions

            /************** Conventions *******************/
            
            // This is how you can specify Id property to Find
            //client.Conventions.ForType<Product>().IdIs(p => p.VariantCode);

            // Normally, customizing client conventions should be part of the initialization, 
            // typically in a initializable module in an EPiServer CMS website 
            client.Conventions.ForType<Product>().IncludeField(p => p.Sizes());

            #endregion

            #region Indexing

            /************* INDEXING *****************/

            //Importer.ClearIndex(client);

            //Importer.AddDemoContentFromFiles(client);

            #endregion

            #region Filtering

            /************* FILTERING *****************/
          
            //FilterSimple(client);

            //FilteringComplexCollections(client);

            //FiltersCombinedExample(client);

            //FilterUsingBuildFilter(client);

            //ProjectIfYouCan(client);

            /************* FILTERING END *****************/

            #endregion

            #region Facets

            // Product facets
            //ProductFacetsExample(client);

            // Store facets using GeoLocation
            //StoresCloseToOurLocation(client);

            #endregion

            #region Multi search

            FindProductsAndStores(client);

            #endregion

            // Cache demo
            //ShowCachingWithDateTimeInQuery(client);
        }


        #region Filter methods

        /// <summary>
        /// String: Match, Prefix, AnyWordBeginsWith, MatchFuzzy
        /// Number: Range
        /// DateTime: MatchYear etc
        /// The AnyWordBeginsWith method matches strings which contains any word that starts with a given string,
        /// making it suitable for autocomplete. It does not care about casing.
        /// NOTE: While AnyWordBeginsWith is powerful it is not optimal in terms of performance when used for large strings.
        /// Be careful!
        /// </summary>
        private static void FilterSimple(IClient client)
        {
            //Example: Products named "Ribbs polo" OR Fuzzy match "Lucy pncho"
            // AND Price range 10-50
            var result = client.Search<Product>()
                .Filter(p => p.Name.MatchCaseInsensitive("Ribbs polo")  |
                    p.Name.MatchFuzzy("Lucy pncho"))
                 .Filter(p => p.Price.InRange(10,50))
                .GetCachedResults();

            ShowProductResults(result);
        }
        

        /// <summary>
        /// Using MatchContained on the complex object OR using Sizes collection
        /// Note: While it is possible to filter on several fields in objects in collections using MatchContained
        /// several times it is not possible to specify that those conditions should apply to the same object in the list.
        /// That is: We can find size S and items with stock larger than 2, we cannot require that those conditions should apply to the same object.
        /// In practice this limitation can often be worked around by filtering on some unique value of the objects in the list.
        /// Also note that while MatchContained works for filtering on complex objects it's best practice to "denormalize" indexed objects by including
        /// fields at indexing time to make querying less complex. 
        /// </summary>
        private static void FilteringComplexCollections(IClient client)
        {
            // Example: All products in Size XL
            var result = client.Search<Product>()
               //.Filter(p => p.Skus.MatchContained(s => s.Size, "XL"))
               .Filter(p => p.Sizes().Match("XL"))
                .GetCachedResults();

            ShowProductResults(result);
        }


        /// <summary>
        /// Using filters on Enum, collections and boolean
        /// When using Match on a list of strings we require that at least one of the strings in the list matches the value specified.
        /// Note: OrderBy orders null values last while OrderByDecending orders them first. 
        /// Use the second argument of type SortMissing to override the default behavior
        /// </summary>
        private static void FiltersCombinedExample(IClient client)
        {
            // Example: All womens jeans that are not sold out, 
            // order by price, then by name

            var result = client.Search<Product>()
                .Filter(p => p.Gender.Match(Gender.Womens))
                .Filter(p => p.Collection.Match(Collection.Jeans))
                .Filter(p => p.InStock.Match(true))
                .OrderBy(p => p.Price)
                .ThenBy(p => p.Name, SortMissing.Last)
                .GetCachedResults();

            ShowProductResults(result);
        }


        /// <summary>
        /// Sometimes, especially when reacting to user input filter has to be dynamically composed.
        /// For this we can use the BuildFilter method.        
        /// </summary>
        private static void FilterUsingBuildFilter(IClient client)
        {
            Console.WriteLine("What colors should we filter on? (Use ',' to separate.) ");
            string colors = Console.ReadLine();

            if (string.IsNullOrEmpty(colors) == false)
            {
                FilterBuilder<Product> colorFilter = client.BuildFilter<Product>();

                foreach (var filterColor in colors.Split(',').ToList())
                {
                    string c = filterColor;
                    colorFilter = colorFilter.Or(x => x.Color.MatchCaseInsensitive(c));
                }

                var result = client.Search<Product>()
                            .Filter(colorFilter)
                            .GetCachedResults();
                
                ShowProductResults(result);
            }
        }

        /// <summary>
        /// Example: Find sizes and stock for product named 'Mio cardigan'
        /// Tailor the objects to your need: Less data transfered = smaller response
        /// </summary>
        private static void ProjectIfYouCan(IClient client)
        {
            var result = client.Search<Product>()
                .Filter(p => p.Name.Match("Mio cardigan"))
                .Select(r => new { Id = r.VariantCode,Skus = r.Skus})
                .GetCachedResults();

            foreach (var hit in result)
            {
                Console.WriteLine(hit.Id);
                foreach (var sku in hit.Skus)
                {
                    Console.WriteLine("\t Size {0} Stock {1}", sku.Size, sku.Stock);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        private static void ShowCachingWithDateTimeInQuery(IClient client)
        {
            int timerBatchOne = 0;

            Console.WriteLine("String first batch");

            for (int i = 0; i < 50; i++)
            {
                var resultOne = client.Search<Product>()
                .Filter(p => p.Collection.Match(Collection.Jeans))
                .Filter(p => p.Gender.Match(Gender.Womens))
                .Filter(p => p.InStock.Match(true))
                .StaticallyCacheFor(TimeSpan.FromMinutes(10))
                .GetResult();
                timerBatchOne += resultOne.ProcessingInfo.ServerDuration;
                Console.Write(".");
            }

            Console.WriteLine("\nTotal time {0} ms", timerBatchOne);

            int timerBatchTwo = 0;
            Console.WriteLine("********************");

            Console.WriteLine("String second batch");

            for (int i = 0; i < 50; i++)
            {
                var result = client.Search<Product>()
                .Filter(p => p.Collection.Match(Collection.Jeans))
                .Filter(p => p.Gender.Match(Gender.Womens))
                .Filter(p => p.InStock.Match(true))
                .Filter(p => p.LastUpdated.LessThan(DateTime.Now))
                .StaticallyCacheFor(TimeSpan.FromMinutes(10))
                .GetResult();

                timerBatchTwo += result.ProcessingInfo.ServerDuration;
                Console.Write(".");
            }

            Console.WriteLine("\nTotal time {0} ms\n", timerBatchTwo);

        }

        #endregion

        #region Facet method

        /// <summary>
        /// Adds facets for specific parameters that gets aggregated across all results
        /// NB: Make sure you know the difference in use of Filter and FilterHits. 
        /// Filters added using Filter are applied BEFORE calculating facets, while FilterHits are added AFTER calculating facets
        /// Filter is better performance wise.
        /// </summary>
        private static void ProductFacetsExample(IClient client)
        {
            var query = client.Search<Product>()
                .FilterHits(p => p.Name.Prefix("Lucy"))
                .TermsFacetFor(p => p.Sizes()) //Size
                .TermsFacetFor(p => p.Color, p => p.Size = 50) //Color
                .RangeFacetFor(p => p.Price, new NumericRange(20, 50), new NumericRange(51, 100), new NumericRange(101, 500)) //Price
                .FilterFacet("Womens", p => p.Gender.Match(Gender.Womens))
                .FilterFacet("Jeans", p => p.Collection.Match(Collection.Jeans))
                .FilterFacet("Sold out", p => p.InStock.Match(false)); //Filterfacet

            var result = query.GetCachedResults();

            ShowProductResults(result);
        }

        /// <summary>
        /// Geographic distance facets: grouping documents with a GeoLocation type property by distance from a location.
        /// Request the number of stores within 1, 5, and 10 kilometers of a location via the GeoDistanceFacetFor method
        /// </summary>
        /// <returns></returns>
        private static void StoresCloseToOurLocation(IClient client)
        {
            // Location of The Cosmopolitan Hotel, Las Vegas, Nevada
            var theCosmopolitanHotelLasVegas = new GeoLocation(36.109308, -115.175291); 

            var ranges = new List<NumericRange> {
                new NumericRange {From = 0, To = 1},
                new NumericRange {From = 0, To = 5},
                new NumericRange {From = 0, To = 10}
            };

            var result = client.Search<Store>()
                .Filter(s => s.Location.WithinDistanceFrom(theCosmopolitanHotelLasVegas, new Miles(10)))
                .GeoDistanceFacetFor(s => s.Location, theCosmopolitanHotelLasVegas, ranges.ToArray())
                .GetCachedResults();

            ShowStoreResults(result);

        }

        #endregion

        #region Multi search

        /// <summary>
        /// Find all men t-shirts and stores within 10 km radius that sell mens clothing
        /// </summary>
        /// <param name="client"></param>
        private static void FindProductsAndStores(IClient client)
        {
            // Location of The Cosmopolitan Hotel, Las Vegas, Nevada
            var theCosmopolitanHotelLasVegas = new GeoLocation(36.109308, -115.175291); 
            
            var multiResults = client.MultiSearch<string>()
                .Search<Product, string>((search => search.Filter(p => p.Collection.Match(Collection.Tees))
                           .Filter(p => p.Gender.Match(Gender.Mens))
                           .Select(p => (string.Format("{0} {1}",p.Name, p.Color) ))))
                    .Search<Store, string>((search =>
                        search.Filter(s => s.Location.WithinDistanceFrom(theCosmopolitanHotelLasVegas, new Kilometers(10)))
                            .Filter(s => s.Departments.Match(Gender.Mens))
                            .Select(p => p.Name)))
                    .GetResult().ToList();

            var products = multiResults[0];
            Console.WriteLine("Found {0} hits for Product ", products.TotalMatching);
            foreach (var productNameAndColor in products.Hits)
            {
                Console.WriteLine("\t{0}", productNameAndColor.Document);
            }


            var stores = multiResults[1];
            Console.WriteLine("Found {0} hits for Store ", stores.TotalMatching);
            foreach (var storeName in stores.Hits)
            {
                Console.WriteLine("\t{0}", storeName.Document);
            }
        }

        #endregion
        
        #region Helper methods

        /// <summary>
        /// Helper method that outputs both facets and results for Product search results
        /// </summary>
        /// <param name="res"></param>
        static void ShowProductResults(SearchResults<Product> res)
        {
            Console.WriteLine("RESULTS Showing {0} out of {1}. Search took {2} ms", res.Hits.Count(), res.TotalMatching, res.ProcessingInfo.ServerDuration);
            Console.WriteLine("--------------------------------------------------------------");

            PrintFacets(res.Facets);

            Console.WriteLine();
            Console.WriteLine("Products found: ");
            foreach (var p in res.Hits)
            {
                Console.WriteLine("\t{0} ({1})", p.Document.Name.ToUpper(), p.Document.VariantCode);
                Console.WriteLine("\tCollection: {0}", p.Document.Collection);
                Console.WriteLine("\tPrice: {0}", p.Document.Price);
                Console.WriteLine("\tSizes: {0}", string.Join(",", p.Document.Sizes().ToArray()));
                Console.WriteLine("");
            }

        }

        /// <summary>
        /// Helper method that outputs both facets and results for Store search results
        /// </summary>
        /// <param name="results"></param>
        static void ShowStoreResults(SearchResults<Store> results)
        {
            Console.WriteLine("RESULTS Showing {0} out of {1}. Search took {2} ms", results.Hits.Count(), results.TotalMatching, results.ProcessingInfo.ServerDuration);
            Console.WriteLine("--------------------------------------------------------------");

            var facet = results.GeoDistanceFacetFor(x => x.Location);

            foreach (var range in facet)
            {
                Console.WriteLine("There are " + range.TotalCount + " stores within " + range.To + " km radius of The Cosmopolitan Hotel.");
            }

            Console.WriteLine();
            Console.WriteLine("Stores found: ");
            foreach (var store in results)
            {
                Console.WriteLine("\tName: {0}", store.Name);
                Console.WriteLine("\tAddress: {0}", store.Address);
                Console.WriteLine("");
            }

        }

        private static void PrintFacets(FacetResults facets)
        {
            if (facets != null)
            {
                foreach (var f in facets)
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
                        Console.WriteLine("\tMean: {0}\n\tMax: {1}\n\tMin: {2}\n\tTotal: {3}\n\tVariance: {4}\n", sf.Mean,
                            sf.Max, sf.Min, sf.Total, sf.Variance);
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
        }

        #endregion
    }
}
