using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Find;
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
            var productsToIndex = new List<Product>();

            #region Parsing Json to Product and Store items

            // Product
            var bbKnitwear = ParseJsonAsProducts("knitwear.json").ToList();
            productsToIndex.AddRange(AddCollectionAndGender(bbKnitwear, Collection.Knitwear, Gender.Womens));

            var bbJeans = ParseJsonAsProducts("jeans.json").ToList();
            productsToIndex.AddRange(AddCollectionAndGender(bbJeans, Collection.Jeans, Gender.Womens));

            var bbShirts = ParseJsonAsProducts("shirts.json").ToList();
            productsToIndex.AddRange(AddCollectionAndGender(bbShirts, Collection.Shirts, Gender.Womens));

            var dmJackets = ParseJsonAsProducts("jackets.json").ToList();
            productsToIndex.AddRange(AddCollectionAndGender(dmJackets, Collection.Jackets, Gender.Mens));

            var dmShirts = ParseJsonAsProducts("cashualshirts.json").ToList();
            productsToIndex.AddRange(AddCollectionAndGender(dmShirts, Collection.Shirts, Gender.Mens));

            // Stores
            var stores = ParseJsonAsStores("stores.json").ToList();
            foreach (var store in stores)
            {
                store.Location = new GeoLocation(store.Latitude, store.Longitude);
            }

            #endregion

            // we are bulk indexing
            IndexBulks(client, productsToIndex, 100);

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

            var jsonReader = new JsonTextReader(new StreamReader(path));
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

            var jsonReader = new JsonTextReader(new StreamReader(path));
            var jsonSerializer = new JsonSerializer();
            return jsonSerializer.Deserialize<IEnumerable<Store>>(jsonReader);
        }

        private static List<Product> AddCollectionAndGender(List<Product> products, Collection collection, Gender gender)
        {
            foreach (var product in products)
            {
                product.Collection = collection;
                product.Gender = gender;
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
