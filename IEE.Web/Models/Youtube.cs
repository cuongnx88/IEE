using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IEE.Web.Models
{
    public class Youtube
    {
        public string Title { get; set; }
        public string Comments { get; set; }
        public string Views { get; set; }
        public string PublishedDate { get; set; }
        public string Link { get; set; }
        public Youtube(string title, string comments, string views, string publishedDate)
        {
            this.Title = title;
            this.Comments = comments;
            this.Views = views;
            this.PublishedDate = publishedDate;
        }
    }

}