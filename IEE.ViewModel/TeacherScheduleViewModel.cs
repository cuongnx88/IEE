
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEE.ViewModel
{
    public class TeacherScheduleViewModel:BaseViewModel
    {
        public int TeacherId { get; set; }
        [Required(ErrorMessage = "Bạn phải nhập ngày bắt đầu")]
        public DateTime FromDate { get; set; }
        [Required(ErrorMessage = "Bạn phải nhập ngày kết thúc")]
        public DateTime ToDate { get; set; }
        [Required(ErrorMessage = "Bạn phải nhập thời gian bắt đầu")]
        public string FromTime { get; set; }
        [Required(ErrorMessage = "Bạn phải nhập kết thúc kết thúc")]
        public string ToTime { get; set; }
        public int Repeat { get; set; }
        public string RepeatOn { get; set; }
        public bool IsVerify { get; set; }   
    }
}
