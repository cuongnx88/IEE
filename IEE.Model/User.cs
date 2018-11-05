using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEE.Model
{
    [MetadataType(typeof(UserMetadata))]
    public partial class User 
    {
       public class UserMetadata
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
            public string Phone { get; set; }
            public string Address { get; set; }
            public string Photo { get; set; }
            public Nullable<DateTime> Birthday { get; set; }
            public bool IsLocked { get; set; }
            public string Salt { get; set; }
            public bool IsSystem { get; set; }
        }
        
    }
}
