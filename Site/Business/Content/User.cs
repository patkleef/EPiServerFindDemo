using EPiServer.Find;

namespace Site.Business.Content
{
    public class User
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string EmailAddress { get; set; }
        public string Phone { get; set; }
        public string Street { get; set; }
        public string Zipcode { get; set; }
        public string City { get; set; }

        public virtual string SearchTitle { get { return Name; } }
        public virtual string SearchText 
        {
            get
            {
                return string.Format("{0} {1} {2} {3} {4} {5}", Username, EmailAddress, Phone, Street, Zipcode, City);
            } 
        }
        public virtual string SearchSection { get { return "User"; } }
    }
}