using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Find;

namespace FindDemo
{
    public class Product
    {
        [Id]
        public string VariantCode { get; set; }
        public string StyleCode { get; set; }
        public string Name { get; set; }
        public object Description { get; set; }
        public double Price { get; set; }
        public double OfferedPrice { get; set; }
        public bool IsOnSale { get; set; }
        public object CampaignLabel { get; set; }
        public bool Available { get; set; }
        public Sku[] Skus { get; set; }
        public bool NewArrival { get; set; }
        public Productimages ProductImages { get; set; }
        public Collection Collection { get; set; }
        public Gender Gender { get; set; }

        public string SearchTitle
        {
            get
            {
                return string.Format("{0} {1}", VariantCode, Name);
            }
        }

        public bool InStock
        {
            get
            {
                return Skus.Any(s => s.Stock > 0);
            }
        }

        public IEnumerable<string> Sizes
        {
            get
            {
                return Skus.Select(s => s.Size).ToList();
            }
        }
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


    public class Productimages
    {
        public string ListImage { get; set; }
        public string MainImage { get; set; }
        public string[] AdditionalImages { get; set; }
    }

    public enum Gender
    {
        Womens, Mens
    }

    public enum Collection
    {
        Outerwear, Tops, Jeans, Shorts, Pants, Knitwear, Accessories, Shirts, Polos, Sweaters, Tees, Dresses, Jackets
    }
}
