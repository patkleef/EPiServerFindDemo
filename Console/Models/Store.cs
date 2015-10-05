using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Find;
using Newtonsoft.Json;

namespace FindDemo
{
    public class Store
    {
        [Id]
        public string Id { get; set; }

        public string Name { get; set; }
        public string CountryName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public GeoLocation Location { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string PostalPlace { get; set; }

        public string SearchTitle
        {
            get
            {
                return string.Format("{0}, {1}", CountryName, Name);
            }
        }
    }
}
