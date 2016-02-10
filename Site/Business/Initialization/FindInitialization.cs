using EPiServer.Find.Cms.Module;
using EPiServer.Find.Framework;
using EPiServer.Find.UnifiedSearch;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
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
            SearchClient.Instance.Conventions.UnifiedSearchRegistry.Add<Employee>();
            SearchClient.Instance.Conventions.UnifiedSearchRegistry.Add<Company>();

            SearchClient.Instance.Conventions.NestedConventions.Add<Company>(company => company.Employees);
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