
using System;
using System.ComponentModel.DataAnnotations;

namespace IEE.ViewModel
{
    public class RoleViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Bạn phải nhập tên nhóm quyền")]
        public string Name { get; set; }
        public string Summarize { get; set; }
    }
}
