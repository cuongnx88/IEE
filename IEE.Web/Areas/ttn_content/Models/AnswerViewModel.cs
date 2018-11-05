using IEE.Infrastructure.DbContext;
using System.Collections.Generic;
using System.Linq;

namespace IEE.Web.Areas.ttn_content.Models
{
    public class AnswerViewModel
    {
        public List<SATQuestion> ListQuestion { get; set; }

        public AnswerViewModel()
        {
            using (var db= new SATEntities())
            {
                ListQuestion = db.SATQuestions.Include("SATAnswers").Where(q=>q.Status==true).ToList();
            }
        }
    }
}