
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace IEE.Infrastructure.DbContext
{
    [MetadataType(typeof(SATQuestionMetadata))]
    public partial class SATQuestion
    {
        public class SATQuestionMetadata
        {
            [Display(Name = "Nội dung")]
            [Required(ErrorMessage = "{0} không được để trống")]
            public string QuestionContent { get; set; }
            [Display(Name = "Trạng thái")]
            public bool Status { get; set; }
            [Display(Name = "Kiểu")]
            public int TypeID { get; set; }
            [Display(Name = "Tiêu đề ngắn gọn")]
            [Required(ErrorMessage = "{0} không được để trống")]
            public string Title { get; set; }
            [Display(Name = "Số thứ tự")]
            [Required(ErrorMessage = "{0} không được để trống")]
            public int QuestionNo { get; set; }
            [Display(Name = "Thuộc nội dung")]
            public int ExamContentID { get; set; }
            [Display(Name = "Câu hỏi")]
            public int ID { get; set; }
            [Display(Name = "Câu trả lời kiểu nhập vào")]
            public bool? HasInputAnswer { get; set; }
        }
    }

    [MetadataType(typeof(SATTypeMetadata))]
    public partial class SATType
    {
        public class SATTypeMetadata
        {
            [Display(Name = "Kiểu")]
            public int TypeName { get; set; }
        }
    }

    [MetadataType(typeof(SATAnswerMetadata))]
    public partial class SATAnswer
    {
        public class SATAnswerMetadata
        {
            [Display(Name = "Nội dung câu trả lời")]
            [Required(ErrorMessage = "{0} không được để trống")]
            public string AnswerContent { get; set; }
            [Display(Name = "Là câu trả lời đúng")]
            public bool IsRightAnswer { get; set; }
            [Display(Name = "Trạng thái")]
            public bool Status { get; set; }
            [Display(Name = "Câu hỏi")]
            public int QuestionID { get; set; }
            [Display(Name = "Kiểu câu trả lời")]
            public int AnswerType { get; set; }
            [Display(Name = "Thứ tự")]
            //[Required(ErrorMessage = "{0} không được để trống")]
            public string Mark { get; set; }
        }
    }

    [MetadataType(typeof(SATAcountMetadata))]
    public partial class SATAccount
    {
        public class SATAcountMetadata
        {
            public int ID { get; set; }
            [Display(Name = "Họ và tên")]
            [Required(ErrorMessage = "Họ và tên không được để trống")]
            public string UserName { get; set; }
            [Required(ErrorMessage = "Bạn phải nhập email")]
            [DataType(DataType.EmailAddress, ErrorMessage = "Địa chỉ email không chính xác")]
            [Display(Name = "Email")]
            [Remote("IsEmailAvaiable", "CmsBase", HttpMethod = "POST")]
            public string Email { get; set; }
            [Display(Name = "Số điện thoại")]
            [DataType(DataType.PhoneNumber)]
            [RegularExpression(@"^(\d{10}|\d{11})$", ErrorMessage = "Số điện thoại không hợp lệ.")]
            public string Phone { get; set; }
            [Display(Name = "Mật khẩu")]
            [Required(ErrorMessage = "Mật khẩu không được để trống")]
            public string Password { get; set; }
            public bool Status { get; set; }
        }
    }

    [MetadataType(typeof(UserMetadata))]
    public partial class User
    {
        public class UserMetadata
        {
            public int Id { get; set; }
            [Display(Name = "Họ và tên")]
            [Required(ErrorMessage = "Họ và tên không được để trống")]
            public string Name { get; set; }
            [Required(ErrorMessage = "Bạn phải nhập email")]
            [DataType(DataType.EmailAddress, ErrorMessage = "Địa chỉ email không chính xác")]
            [Display(Name = "Email")]
            [Remote("IsEmailAvaiable", "CmsBase", HttpMethod = "POST")]
            public string Email { get; set; }
            [Display(Name = "Số điện thoại")]
            [DataType(DataType.PhoneNumber)]
            [RegularExpression(@"^(\d{10}|\d{11})$", ErrorMessage = "Số điện thoại không hợp lệ.")]
            public string Phone { get; set; }
            [Display(Name = "Mật khẩu")]
            [Required(ErrorMessage = "Mật khẩu không được để trống")]
            public string Password { get; set; }

        }
    }

    public class SATAccountRegisterModel : User
    {
        [Required(ErrorMessage = "Xác nhận Mật khẩu không được để trống")]
        public string ConfirmPassword { get; set; }
    }

    [MetadataType(typeof(SATExamFormMetadata))]
    public partial class SATExamForm
    {
        public class SATExamFormMetadata
        {
            [Display(Name = "Phương hướng")]
            [Required(ErrorMessage = "{0} không được để trống")]
            public string Direction { get; set; }
            [Display(Name = "Tiêu đề")]
            [Required(ErrorMessage = "{0} không được để trống")]
            public string Title { get; set; }
            [Display(Name = "Thời lượng")]
            [Required(ErrorMessage = "{0} không được để trống")]
            public int Duration { get; set; }
            [Display(Name = "Số câu hỏi")]
            [Required(ErrorMessage = "{0} không được để trống")]
            public int NumberQuestion { get; set; }
            [Display(Name = "Kiểu bài")]
            public int TypeID { get; set; }
            [Display(Name = "Đề số")]
            [Required(ErrorMessage = "{0} không được để trống")]
            public int ExamCode { get; set; }
        }
    }

    [MetadataType(typeof(SATExamContentMetedata))]
    public partial class SATExamContent
    {
        public class SATExamContentMetedata
        {
            [Display(Name = "Tiêu đề")]
            public string Title { get; set; }
            [Display(Name = "Giới thiệu")]
            public string Intro { get; set; }
            [Display(Name = "Thứ tự")]
            public int ExamContentIndex { get; set; }
            [Display(Name = "Ảnh đính kèm")]
            public string AttachImage { get; set; }
            [Display(Name = "Nội dung")]
            public string Contents { get; set; }

            [Display(Name = "Mẫu bài thi")]
            public int ExamFormID { get; set; }
            [Display(Name = "Trạng thái")]
            public bool Status { get; set; }

        }
    }

    [MetadataType(typeof(SATScoreMetadata))]
    public partial class SATScore
    {
        public class SATScoreMetadata
        {
            [Display(Name = "Số câu trả lời đúng")]
            public int RawValue { get; set; }
            [Display(Name = "Số điểm")]
            public int ScoreValue { get; set; }
        }
    }
    [MetadataType(typeof(MediaMetadata))]
    public partial class Medium
    {
        public class MediaMetadata
        {

            public int Id { get; set; }
            public string Name { get; set; }
            [Display(Name = "Tên tài liệu")]
            [Required(ErrorMessage = "{0} không được để trống")]
            public string DisplayName { get; set; }
            public string FileSource { get; set; }
            public string Link { get; set; }
            public Nullable<int> Type { get; set; }
            public Nullable<System.DateTime> CreatedDate { get; set; }
            public Nullable<int> CreatedBy { get; set; }
            public Nullable<System.DateTime> ModifiedDate { get; set; }
            public Nullable<int> ModifiedBy { get; set; }
            public Nullable<bool> IsDeleted { get; set; }
        }
    }


    [MetadataType(typeof(TutorialVideoMetadata))]
    public partial class TutorialVideo
    {
        public class TutorialVideoMetadata
        {
            public int ID { get; set; }
            [Required(ErrorMessage = "{0} không được để trống")]
            [Display(Name = "Link")]
            public string Link { get; set; }
            [Required(ErrorMessage = "{0} không được để trống")]
            [Display(Name = "Tiêu đề")]
            public string Title { get; set; }
            [Required(ErrorMessage = "{0} không được để trống")]
            [Display(Name = "Trạng thái")]
            public bool Status { get; set; }
            public string Type { get; set; }
            [Display(Name = "Mô tả")]
            public string Description { get; set; }
            [Display(Name = "Danh mục")]
            public Nullable<int> CatId { get; set; }
            [Display(Name = "Ảnh đại diện")]
            public string Thumbnail { get; set; }
            [Display(Name = "Ngày đăng")]
            public Nullable<System.DateTime> CreateDate { get; set; }
        }

    }
}