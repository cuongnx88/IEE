using IEE.Infrastructure.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IEE.Web.Areas.ttn_content.Models
{
    public class PartialViewModel
    {
        public Category Cate { get; set; }
        public List<Post> ListPost { get; set; }
    }
}