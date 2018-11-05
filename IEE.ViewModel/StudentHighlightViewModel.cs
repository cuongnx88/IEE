using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEE.ViewModel
{
    public class StudentHighlightViewModel: BaseViewModel
    {
        [Required(ErrorMessage = "Bạn phải nhập tên học viên")]
        public string Name { get; set; }
        public string Photo { get; set; }
        public string Title { get; set; }
        public string Summarize { get; set; }
        public Nullable<int> Program { get; set; }
        public string Score { get; set; }
        public string SAT { get; set; }
        public string TOEFL { get; set; }
        public string IELTS { get; set; }
        public string ApplySchool { get; set; }
        public string Scholarship { get; set; }


    }
}
