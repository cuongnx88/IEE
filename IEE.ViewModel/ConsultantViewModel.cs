
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEE.ViewModel
{
    public class ConsultantViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Bạn phải nhập họ và tên")]
        public string Name { get; set; }
        public string School { get; set; }
        public string Class { get; set; }
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^(\d{10}|\d{11})$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Bạn phải nhập email")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Địa chỉ email không chính xác")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Bạn phải nhập nội dung")]
        public string Message { get; set; }
        public Nullable<int> Program { get; set; }
        public bool IsComplate { get; set; }
    }
}
