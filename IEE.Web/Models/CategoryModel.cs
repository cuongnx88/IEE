using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IEE.Web.Models
{
    public class CategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Summarize { get; set; }
        public Nullable<int> ParentId { get; set; }
        public bool IsMenu { get; set; }
        public string Controller { get; set; }
        public Nullable<bool> IsSystem { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<int> OrderNumber { get; set; }
        public string keyword { get; set; }
        public string MetaKey { get; set; }
        public string MetaDescription { get; set; }
        public string Position { get; set; }
        public string SEOTitle { get; set; }
    }
}