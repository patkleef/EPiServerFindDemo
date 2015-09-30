using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace Site.Models.Pages
{
    [ContentType(DisplayName = "Article page", GUID = "{E14715FD-72F8-4826-9581-D7E3015BF7DB}", Description = "")]
    public class ArticlePage : PageData
    {
        [Display(
            Name = "Body text",
            GroupName = SystemTabNames.Content,
            Order = 10)]
        public virtual XhtmlString BodyText { get; set; }

        [Display(
            Name = "Date",
            GroupName = SystemTabNames.Content,
            Order = 20)]
        public virtual DateTime Date { get; set; }

        public virtual DateTime? SearchUpdateDate
        {
            get { return Date; }
        }
    }
}