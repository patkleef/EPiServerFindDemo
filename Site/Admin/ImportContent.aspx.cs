using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Xml.Linq;
using System.Xml.XPath;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.Find;
using EPiServer.PlugIn;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using Faker;
using Site.Business.Content;
using Site.Models.Pages;
using Address = Faker.Address;
using Company = Site.Business.Content.Company;

namespace Site.Admin
{
    /// <summary>
    /// Content migration
    /// </summary>
    [GuiPlugIn(DisplayName = "Import content", Description = "", Area = PlugInArea.AdminMenu, Url = "~/Admin/ImportContent.aspx")]
    public partial class ContentMigration : Page
    {
        private readonly IContentRepository _contentRepository;
        private readonly IClient _client;
        private int _postPageContainerId;

        /// <summary>
        /// Public constructor
        /// </summary>
        public ContentMigration()
        {
            _contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
            _client = Client.CreateFromConfig();
        }

        /// <summary>
        /// Reindex object button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ReindexExternObjectButton_Click(object sender, EventArgs e)
        {
            _client.Delete<User>(x => x.GetType().Name.Exists() | !x.GetType().Name.Exists());
            _client.Delete<Company>(x => x.GetType().Name.Exists() | !x.GetType().Name.Exists());

            ImportUsers();
            ImportCompanies();
        }

        /// <summary>
        /// Import button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ImportButton_Click(object sender, EventArgs e)
        {
            // Clear index
            _client.Delete<object>(x => x.GetType().Name.Exists() | !x.GetType().Name.Exists());

            //Remove pages first
            _contentRepository.DeleteChildren(ContentReference.StartPage, true, AccessLevel.Publish);

            var articlesPage = _contentRepository.GetDefault<ArticlesPage>(ContentReference.StartPage);
            articlesPage.PageName = "Articles";
            _contentRepository.Save(articlesPage, SaveAction.Publish);

            // Import article pages
            var numberOfArticles = ImportArticles(articlesPage.ContentLink.ID);

            var numberOfUsers = ImportUsers();
            var numberOfCompanies = ImportCompanies();

            ImportResult.Text = string.Format("{0} article imported, {1} users imported, {2} companies imported", numberOfArticles, numberOfUsers, numberOfCompanies);
        }

        /// <summary>
        /// Import company
        /// </summary>
        /// <returns></returns>
        private int ImportCompanies()
        {
            var number = 0;
            for (var i = 0; i < 20; i++)
            {
                var company = new Company();
                company.Name = Faker.Company.Name();
                company.Bs = Faker.Company.BS();
                company.CatchPhrase = Faker.Company.CatchPhrase();
                company.Street = Address.StreetAddress();
                company.Zipcode = Address.UkPostCode();
                company.City = Address.City();
                company.Phone = Phone.Number();

                _client.Index(company);
                number++;
            }
            return number;
        }

        /// <summary>
        /// Import users
        /// </summary>
        /// <returns></returns>
        private int ImportUsers()
        {
            var number = 0;
            for (var i = 0; i < 20; i++)
            {
                var user = new User();
                user.Name = Name.FullName();
                user.Username = Internet.UserName();
                user.EmailAddress = Internet.Email();
                user.Phone = Phone.Number();
                user.Street = Address.StreetAddress();
                user.Zipcode = Address.UkPostCode();
                user.City = Address.City();
                
                _client.Index(user);
                number++;
            }
            return number;
        }

        /// <summary>
        /// Import article pages
        /// </summary>
        /// <param name="articlesPageId"></param>
        /// <returns></returns>
        private int ImportArticles(int articlesPageId)
        {
            var number = 0;
            var articleList = GetDemoArticlePages(articlesPageId);

            // Save all articles
            foreach (var articlePage in articleList)
            {
                _contentRepository.Save(articlePage, SaveAction.Publish);
                number++;
            }
            return number;
        }

        /// <summary>
        /// Get demo article pages
        /// </summary>
        /// <param name="parentPageId"></param>
        /// <returns></returns>
        private IEnumerable<ArticlePage> GetDemoArticlePages(int parentPageId)
        {
            //http://www.articlesbase.com/technology-articles/6/
            var pages = new List<ArticlePage>();

            var xmlFile = HttpContext.Current.Server.MapPath("~/App_Data/articles.xml");

            var xml = XDocument.Load(xmlFile);

            foreach (var item in xml.XPathSelectElements("/articles/article"))
            {
                var articlePage = _contentRepository.GetDefault<ArticlePage>(new ContentReference(parentPageId));
                articlePage.PageName = item.Element("title").Value;
                articlePage.BodyText = new XhtmlString(item.Element("text").Value);
                articlePage.Date = DateTime.ParseExact(item.Element("date").Value, "dd-MM-yyyy",
                    CultureInfo.InvariantCulture);
                pages.Add(articlePage);
            }
            return pages;
        }
    }
}