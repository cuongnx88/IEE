using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEE.ViewModel
{
    public class TestimonialViewModel: BaseViewModel
    {
        
        [Required(ErrorMessage = "Bạn phải nhập thông tin người đánh giá")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Bạn phải nhập thông tin người học")]
        public string Subtitle { get; set; }
        [Required(ErrorMessage = "Bạn phải nhập cảm nhận")]
        public string Summarize { get; set; }
        public string Photo { get; set; }
        public string Class { get; set; }
        public string SATScore { get; set; }
    }
}
