using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Find;
using FindDemo.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            var productsToIndex = new List<Product>();

            #region Parsing Json to Product and Store items

            // Product
            var womenKnitwear = ParseJsonAsProducts("knitwear.json").ToList();
            productsToIndex.AddRange(AddCollection(womenKnitwear, Collection.Knitwear));

            var womenJeans = ParseJsonAsProducts("jeans.json").ToList();
            productsToIndex.AddRange(AddCollection(womenJeans, Collection.Jeans));

            var womenShirts = ParseJsonAsProducts("shirts.json").ToList();
            productsToIndex.AddRange(AddCollection(womenShirts, Collection.Shirts));

            var menBoxers = ParseJsonAsProducts("boxers.json").ToList();
            productsToIndex.AddRange(AddCollection(menBoxers, Collection.Underwear));

            var menTShirts = ParseJsonAsProducts("thirts.json").ToList();
            productsToIndex.AddRange(AddCollection(menTShirts, Collection.Tees));

            // Stores
            var stores = ParseJsonAsStores("stores.json").ToList();

            #endregion

            // we are bulk indexing
            //IndexBulks(client, productsToIndex, 100);

            IndexBulks(client, stores, 100);
        }

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

        private static List<Product> AddCollection(List<Product> products, Collection collection)
        {
            foreach (var product in products)
            {
                product.Collection = collection;
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
                client.Delete<Product>(product => product.VariantCode.Exists());
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
