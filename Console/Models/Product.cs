using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Find;

namespace FindDemo.Models
{
    public class Product
    {
        [EPiServer.Find.Id]
        public string ProductId { get; set; }

        public string StyleCode { get; set; }
        public string Name { get; set; }
        public object Description { get; set; }
        public double Price { get; set; }
        public double OfferedPrice { get; set; }
        public bool IsOnSale { get; set; }
        public object CampaignLabel { get; set; }
        public string Color { get; set; }
        
        [Newtonsoft.Json.JsonIgnore]
        public bool Available { get; set; }
        
        public IEnumerable<Sku> Skus { get; set; }
        public bool NewArrival { get; set; }
        public CategoryEnum CategoryEnum { get; set; }
        public Gender Gender { get; set; }

        public bool InStock
        {
            get
            {
                return Skus.Any(s => s.Stock > 0);
            }
        }

        public string SearchTitle
        {
            get
            {
                return string.Format("{0}: {1}, {2}", ProductId, Name, Color);
            }
        }


        public DateTime LastUpdated { get { return DateTime.Now; } }
    }

    /// <summary>
    /// SKU: Product Stock Keeping Unit
    /// </summary>
    public class Sku
    {
        public string ID { get; set; }
        public int Stock { get; set; }
        public string Size { get; set; }
    }

    public enum Gender
    {
        Womens, Mens
    }

    public enum CategoryEnum
    {
        Underwear, Tops, Jeans, Shorts, Pants, Knitwear, Accessories, Shirts, Polos, Sweaters, Tees, Dresses, Jackets
    }
}
