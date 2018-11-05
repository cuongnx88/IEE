using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEE.ViewModel
{
    public class CalendarViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "Bạn phải nhập tên lịch học/khai giảng")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Bạn phải nhập ngày bắt đầu")]
        public Nullable<DateTime> FromDate { get; set; }
        [Required(ErrorMessage = "Bạn phải nhập tên ngày kết thúc")]
        public Nullable<DateTime> ToDate { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }
        public string Summarize { get; set; }
        
    }
}
