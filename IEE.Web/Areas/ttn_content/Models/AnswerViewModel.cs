using IEE.Infrastructure.DbContext;
using PagedList;
using System.Collections.Generic;
using System.Linq;

namespace IEE.Web.Areas.ttn_content.Models
{
    public class AnswerViewModel
    {
        public IPagedList<SATQuestion> ListQuestion { get; set; }

        public AnswerViewModel()
        {
           
        }
    }
}