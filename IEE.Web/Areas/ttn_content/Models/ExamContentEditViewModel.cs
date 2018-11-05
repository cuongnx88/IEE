using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IEE.Infrastructure.DbContext;

namespace IEE.Web.Areas.ttn_content.Models
{
    public class ExamContentEditViewModel
    {
        public SATExamContent ExamContent { get; set; }
        public List<SATContentLine> ContentLines { get; set; }

        public ExamContentEditViewModel()
        {

        }
        public ExamContentEditViewModel(SATExamContent model)
        {
            ExamContent = model;
            using (var db= new SATEntities())
            {
                ContentLines = ExamContent.SATContentLines.ToList();
                //ContentLines = db.SATExamContents.Include("SATContentLine").Where(e => e.ID == model.ID).FirstOrDefault().SATContentLines.ToList();
            }
        }
    }
}