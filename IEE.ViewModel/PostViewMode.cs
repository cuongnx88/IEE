
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEE.ViewModel
{
    public class PostViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "Bạn phải nhập tên tiêu đề")]
        public string Title { get; set; }
        public string Summarize { get; set; }
        public string Detail { get; set; }
        public string Photo { get; set; }
        public bool Status { get; set; }
        public Nullable<DateTime> PublishedDate { get; set; }
        public Nullable<int> OrderNumber { get; set; }
        public string MetaKey { get; set; }
        public string MetaDescription { get; set; }
        public string AttachFiles { get; set; }
        public long? ViewCount { get; set; }
    }
}
