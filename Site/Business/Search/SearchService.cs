using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EPiServer.Find;
using EPiServer.ServiceLocation;

namespace Site.Business.Search
{
    public class SearchService
    {
        private readonly IClient _client;

        public SearchService()
        {
            _client = ServiceLocator.Current.GetInstance<IClient>();
        }
    }
}