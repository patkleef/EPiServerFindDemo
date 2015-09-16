using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Web.Mvc;

namespace Site.Controllers
{
    [TemplateDescriptor(Inherited = true)]
    public class DefaultPageController : PageController<PageData>
    {
        public ViewResult Index(PageData currentPage)
        {
            return View(string.Format("~/Views/{0}/Index.cshtml", currentPage.GetOriginalType().Name), currentPage);
        }
    }
}