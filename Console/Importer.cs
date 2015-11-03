using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EPiServer.Find;
using FindDemo.Models;
using Newtonsoft.Json;

namespace FindDemo
{
    public class Importer
    {
        /// <summary>
        /// Reads json from sample test files, parses it to Product/Store items and adds them to the index
        /// </summary>
        /// <param name="client"></param>
        public static void AddDemoContentFromFiles(IClient client)
        {
            #region Parsing Json to Product and Store items

            var productsToIndex = new List<Product>();

            // Product
            var womenKnitwear = ParseJsonAsProducts("knitwear.json").ToList();
            productsToIndex.AddRange(AddCollection(womenKnitwear, CategoryEnum.Knitwear));

            var womenJeans = ParseJsonAsProducts("jeans.json").ToList();
            productsToIndex.AddRange(AddCollection(womenJeans, CategoryEnum.Jeans));

            var womenShirts = ParseJsonAsProducts("shirts.json").ToList();
            productsToIndex.AddRange(AddCollection(womenShirts, CategoryEnum.Shirts));

            var menBoxers = ParseJsonAsProducts("boxers.json").ToList();
            productsToIndex.AddRange(AddCollection(menBoxers, CategoryEnum.Underwear));

            var menTShirts = ParseJsonAsProducts("thirts.json").ToList();
            productsToIndex.AddRange(AddCollection(menTShirts, CategoryEnum.Tees));

            // Stores
            var stores = ParseJsonAsStores("stores.json").ToList();

            #endregion

            #region Indexing region
            
            // Adding objects using Index method

            IndexBulks(client, productsToIndex, 100);

            IndexBulks(client, stores, 100);

            #endregion
        }

        /// <summary>
        /// Remember to always bulk index - to increase performance and reduse nr of trips to the server
        /// </summary>
        /// <param name="client"></param>
        /// <param name="objects"></param>
        /// <param name="bulkSize"></param>
        private static void IndexBulks(IClient client, IEnumerable<object> objects, int bulkSize)
        {
            while (objects.Any())
            {
                var indexingResult = client.Index(objects.Take(bulkSize));
                Console.WriteLine("{0} items indexed. Took {1} ms", indexingResult.Items.Count(), indexingResult.Took);
                objects = objects.Skip(bulkSize);   
            }
        }

        private static IEnumerable<Product> ParseJsonAsProducts(string fileName)
        {
            string path = GetFilePath(fileName);
            if (!File.Exists(path))
            {
                Console.WriteLine("Unable to locate file {0}", fileName);
                return new List<Product>();
            }

            var jsonReader = new JsonTextReader(new StreamReader(path, new UTF8Encoding()));
            var jsonSerializer = new JsonSerializer();
            var products = jsonSerializer.Deserialize<IEnumerable<Product>>(jsonReader);

            return products;
        }
        
        private static IEnumerable<Store> ParseJsonAsStores(string fileName)
        {
            string path = GetFilePath(fileName);
            if (!File.Exists(path))
            {
                Console.WriteLine("Unable to locate file {0}", fileName);
                return new List<Store>();
            }

            var jsonReader = new JsonTextReader(new StreamReader(path, new UTF8Encoding()));
            var jsonSerializer = new JsonSerializer();
            return jsonSerializer.Deserialize<IEnumerable<Store>>(jsonReader);
        }

        private static List<Product> AddCollection(List<Product> products, CategoryEnum categoryEnum)
        {
            foreach (var product in products)
            {
                product.CategoryEnum = categoryEnum;
            }
            return products;
        }

        private static string GetFilePath(string fileName)
        {
            string path = Directory.GetCurrentDirectory().Replace(@"\bin\Debug", @"\JsonSampleData\");
            return path + fileName;
        }


        public static void ClearIndex(IClient client)
        {
            Console.WriteLine("Are you REALLY sure you want to clear the index? Y=YES, N=NO.");
            string answer = Console.ReadLine();
            if (answer != null && answer.ToLower() == "y")
            {
                client.Delete<Product>(product => product.ProductId.Exists());
                client.Delete<Store>(store => store.Name.Exists());
                Console.WriteLine("Index is empty. You're starting clean");
            }
            else
            {
                Console.WriteLine("Phhewww. The data is still intact. Go on querying");
            }
        }
    }

   
}
