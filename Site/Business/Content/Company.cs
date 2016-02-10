using System;
using System.Collections.Generic;
using EPiServer.Find.UnifiedSearch;

namespace Site.Business.Content
{
    public class Company
    {
        public Guid Key { get; set; }
        public string Name { get; set; }
        public string CatchPhrase { get; set; }
        public string Bs { get; set; }
        public string Phone { get; set; }
        public string Street { get; set; }
        public string Zipcode { get; set; }
        public string City { get; set; }
        public List<Employee> Employees { get; set; } 

        public virtual string SearchTitle { get { return Name; } }
        public virtual string SearchText
        {
            get
            {
                return string.Format("{0} {1} {2} {3} {4} {5}", CatchPhrase, Bs, Phone, Street, Zipcode, City);
            }
        }
        public virtual string SearchSection { get { return "Company"; } }
    }
}