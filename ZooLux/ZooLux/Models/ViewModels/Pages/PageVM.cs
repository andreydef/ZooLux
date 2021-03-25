using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ZooLux.Models.Data;

namespace ZooLux.Models.ViewModels.Pages
{
    public class PageVM
    {
        public PageVM() { }

        public PageVM(PagesDTO row)
        {
            Id = row.Id;
            Title = row.Title;
            Slug = row.Slug;
            Body = row.Body;
        }

        public int Id { get; set; }

        [Required]
        [StringLength(int.MaxValue, MinimumLength = 3)]
        public string Title { get; set; }

        public string Slug { get; set; }

        [Required]
        [StringLength(int.MaxValue, MinimumLength = 3)]
        [AllowHtml]
        public string Body { get; set; }

        public IEnumerable<SelectListItem> Pages { get; set; }
    }
}