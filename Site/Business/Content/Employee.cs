using System;
using EPiServer.Find;

namespace Site.Business.Content
{
    public class Employee
    {
        public Guid Key { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string EmailAddress { get; set; }
        public string Phone { get; set; }
        public string Street { get; set; }
        public string Zipcode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public int Age { get; set; }

        public virtual string SearchTitle { get { return Name; } }
        public virtual string SearchText 
        {
            get
            {
                return string.Format("{0} {1} {2} {3} {4} {5}", Username, EmailAddress, Phone, Street, Zipcode, City);
            } 
        }
        public virtual string SearchSection { get { return "Employee"; } }
    }
}