
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEE.ViewModel
{
    public class UserViewModel :BaseViewModel
    {
        [Required(ErrorMessage = "Bạn phải nhập họ và tên")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Bạn phải nhập email")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Địa chỉ email không chính xác")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Bạn phải nhập mật khẩu")]
        [StringLength(100, ErrorMessage = "Mật khẩu phải nhiều hơn 6 ký tự và tối đa 100 ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^(\d{10}|\d{11})$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Photo { get; set; }
        public Nullable<DateTime> Birthday { get; set; }
        public bool IsLocked { get; set; }
        public string Salt { get; set; }
        public bool IsSystem { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp.")]
        public string ConfirmPassword { get; set; }

    }

    public class TeacherViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Bạn phải nhập họ và tên")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Bạn phải nhập email")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Địa chỉ email không chính xác")]
        public string Email { get; set; }
        public string Address { get; set; }

        [Required(ErrorMessage = "Bạn phải nhập số di động")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^(\d{10}|\d{11})$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string Phone { get; set; }
        public string Photo { get; set; }
        [DataType(DataType.Date)]
        public Nullable<DateTime> Birthday { get; set; }

    }

    public class SettingPasswordViewModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

}
