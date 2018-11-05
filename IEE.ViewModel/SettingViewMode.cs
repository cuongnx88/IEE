
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEE.ViewModel
{
    public class SettingViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Bạn phải nhập key")]
        public string Key { get; set; }
        [Required(ErrorMessage = "Bạn phải nhập giá trị")]
        public string Value { get; set; }
    }
}
