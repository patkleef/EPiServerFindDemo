using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace Site.Models.Pages
{
    [ContentType(DisplayName = "CommentPage", GUID = "{846B9B8A-9B48-4AB5-A6B1-27408E1B361E}", Description = "")]
    public class CommentPage : PageData
    {
        [Editable(false)]
        [Display(
            Name = "Id",
            GroupName = SystemTabNames.Content,
            Order = 10)]
        public virtual int Id { get; set; }

        [CultureSpecific]
        [Display(
            Name = "Name",
            GroupName = SystemTabNames.Content,
            Order = 20)]
        public virtual string CommentName { get; set; }

        [CultureSpecific]
        [Display(
            Name = "E-mail",
            GroupName = SystemTabNames.Content,
            Order = 30)]
        public virtual string Email { get; set; }

        [CultureSpecific]
        [Display(
            Name = "E-mail",
            GroupName = SystemTabNames.Content,
            Order = 40)]
        public virtual string Body { get; set; }

        [Display(
            Name = "Date time",
            GroupName = SystemTabNames.Content,
            Order = 50)]
        public virtual DateTime Created { get; set; }
    }
}