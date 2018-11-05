using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEE.ViewModel
{
    public class CategoryViewModel: BaseViewModel
    {
       
        [Required(ErrorMessage = "Bạn phải nhập tên nhóm")]
        public string Name { get; set; }
        public string Summarize { get; set; }
        public Nullable<int> ParentId { get; set; }
        public bool IsMenu { get; set; }
        public string Controller { get; set; }
        public bool IsSystem { get; set; }
        public Nullable<int> OrderNumber { get; set; }
        public string keyword { get; set; }
        public string MetaKey { get; set; }
        public string MetaDescription { get; set; }
        public string Position { get; set; }
        public string DisplayType { get; set; }
    }
}
