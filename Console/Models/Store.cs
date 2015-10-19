using System.Diagnostics;
using EPiServer.Find;

namespace FindDemo.Models
{
    public class Store
    {
        [Id]
        public int StoreId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string FullPostalCode { get; set; }
        public string Country { get; set; }
        public float Lat { get; set; }
        public float Lng { get; set; }
        public string Hours { get; set; }
        public string HoursAmPm { get; set; }
        public string Phone { get; set; }
        public string PostalCode { get; set; }
        public Gender[] Departments { get; set; }

        public GeoLocation Location
        {
            get
            {
                return new GeoLocation(Lat, Lng);
            }
        }


        //public string SearchTitle
        //{
        //    get
        //    {
        //        return string.Format("{0}, {1}", Region, Name);
        //    }
        //}

    }
}
