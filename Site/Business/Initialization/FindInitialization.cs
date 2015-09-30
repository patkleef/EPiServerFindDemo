using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EPiServer.Find;
using EPiServer.Find.Cms;
using EPiServer.Find.Cms.Conventions;
using EPiServer.Find.Cms.Module;
using EPiServer.Find.Framework;
using EPiServer.Find.UnifiedSearch;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using Site.Business.Content;

namespace Site.Business.Initialization
{
    /// <summary>
    /// Rendering intialization
    /// </summary>
    [ModuleDependency(typeof(IndexingModule))]
    public class FindInitialization1 : IInitializableModule
    {

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="context"></param>
        public void Initialize(InitializationEngine context)
        {
            SearchClient.Instance.Conventions.UnifiedSearchRegistry.Add<User>();
            SearchClient.Instance.Conventions.UnifiedSearchRegistry
                .Add<Company>();
        }

        /// <summary>
        /// Uninitialize
        /// </summary>
        /// <param name="context"></param>
        public void Uninitialize(InitializationEngine context)
        {

        }
    }
}