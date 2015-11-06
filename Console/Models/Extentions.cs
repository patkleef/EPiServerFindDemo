using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Find;
using EPiServer.Find.Api.Querying.Filters;

namespace FindDemo.Models
{
    public static class Extentions
    {
        public static IEnumerable<string> Sizes(this Product product)
        {
             return product.Skus.Select(s => s.Size).ToList();
        }



        public static SearchResults<TResult> GetCachedResults<TResult>(this ISearch<TResult> search)
        {
            return search.StaticallyCacheFor(TimeSpan.FromHours(1)).GetResult();
        }

        public static ITypeSearch<Product> FindTeesForMen(this ITypeSearch<Product> search)
        {
            return search.Filter(p => p.CategoryEnum.Match(CategoryEnum.Tees))
                .Filter(p => p.Gender.Match(Gender.Mens));
        }

        public static ITypeSearch<Store> FindStoresForMenCloseToMe(this ITypeSearch<Store> search, GeoLocation yourLocation)
        {
            return search.Filter(s => s.Location.WithinDistanceFrom(yourLocation, new Kilometers(10)))
                .Filter(s => s.Departments.Match(Gender.Mens));
        }
    }
}
