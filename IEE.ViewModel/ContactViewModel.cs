using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IEE.ViewModel
{
    public class ContactViewModel
    {
        [Required(ErrorMessage = "Bạn phải nhập họ và tên")]
        public string Name { get; set; }
        public string Address { get; set; }
        [Required(ErrorMessage = "Bạn phải nhập email")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Địa chỉ email không chính xác")]
        public string Email { get; set; }
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^(\d{10}|\d{11})$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Bạn phải nhập tiêu đề email")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Bạn phải nhập nội dung")]
        public string Content { get; set; }
    }
}