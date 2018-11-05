using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IEE.ViewModel
{
    public class CouncilViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "Bạn phải nhập họ và tên")]
        public string Name { get; set; }
        public string University { get; set; }
        [Required(ErrorMessage = "Bạn phải nhập chức danh")]
        public string Title { get; set; }
        public string Summarize { get; set; }
        public string Photo { get; set; }
    }
}