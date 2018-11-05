using IEE.Infrastructure.DbContext;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static IEE.Infrastructure.DbContext.SATQuestion;

namespace IEE.Web.Areas.ttn_content.Models
{
    [MetadataType(typeof(QuestionMetadata))]
    public class QuestionCreateViewModel
    {
        public string QuestionContent { get; set; }
        public string Explanation { get; set; }
        public bool Status { get; set; }
        public Nullable<int> TypeID { get; set; }
        public string Title { get; set; }
        public Nullable<int> ExamContentID { get; set; }
        public int QuestionNo { get; set; }
        public Nullable<bool> HasInputAnswer { get; set; }

        public List<SATAnswer> Answers { get; set; }

        public QuestionCreateViewModel()
        {
            Answers = new List<SATAnswer>();
            for (int i = 0; i < 4; i++)
            {
                Answers.Add(new SATAnswer { AnswerContent = String.Empty, AnswerType = 0, IsRightAnswer = false, Mark = null, Status = true, QuestionID = 0 });
            }
        }

    }
    [MetadataType(typeof(QuestionMetadata))]
    public class QuestionEditViewModel
    {
        public int ID { get; set; }
        public string QuestionContent { get; set; }
        public string Explanation { get; set; }
        public bool Status { get; set; }
        public Nullable<int> TypeID { get; set; }
        public string Title { get; set; }
        public Nullable<int> ExamContentID { get; set; }
        public int QuestionNo { get; set; }
        public Nullable<bool> HasInputAnswer { get; set; }

        public List<SATAnswer> Answers { get; set; }

        public QuestionEditViewModel()
        {

        }
        public QuestionEditViewModel(int questionId)
        {
            Answers = new List<SATAnswer>();
            using (var db= new SATEntities())
            {
                Answers = db.SATAnswers.Where(a => a.QuestionID == questionId).ToList();
            }
            //for (int i = 0; i < 4; i++)
            //{
            //    Answers.Add(new SATAnswer { AnswerContent = String.Empty, AnswerType = 0, IsRightAnswer = false, Mark = null, Status = true, QuestionID = 0 });
            //}
        }

    }

    public class QuestionMetadata
    {
        [Display(Name = "Nội dung")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public string QuestionContent { get; set; }
        [Display(Name = "Giải thích")]
        public string Explanation { get; set; }
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
        
        [Display(Name = "Câu trả lời kiểu nhập vào")]
        public bool? HasInputAnswer { get; set; }
    }
}