using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindDemo.Models
{
    public static class Extentions
    {
        public static IEnumerable<string> Sizes(this Product product)
        {
             return product.Skus.Select(s => s.Size).ToList();
        }
    }
}
