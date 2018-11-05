using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IEE.ViewModel
{
    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Thư điện tử")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Display(Name = "Ghi nhớ")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Bạn phải nhập họ và tên")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Bạn phải nhập email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Bạn phải nhập mật khẩu")]
        [StringLength(100, ErrorMessage = "Mật khẩu phải nhiều hơn 6 ký tự và tối đa 100 ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp.")]
        public string ConfirmPassword { get; set; }
        public string Address { get; set; }

        [Required(ErrorMessage = "Bạn phải nhập số di động")]
        [Range(10,11, ErrorMessage = "Số điện thoại là 10 số hoặc 11 số")]
        public string Phone { get; set; }
        public string Photo { get; set; }
        public string Parent { get; set; }

        [Range(10, 11, ErrorMessage = "Số điện thoại là 10 số hoặc 11 số")]
        public string ParentPhone { get; set; }

        [EmailAddress]
        public string ParentEmail { get; set; }

        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }
        public int Type { get; set; }
        
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
