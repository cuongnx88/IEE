using IEE.Infrastructure.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IEE.Web.Areas.ttn_content.Models
{
    public class ScoreViewModel
    {
        public List<SATScore> Reading { get; set; }
        public List<SATScore> Writing { get; set; }
        public List<SATScore> Math { get; set; }
        public List<SATScore> MathWithCalculator { get; set; }


        public ScoreViewModel(List<SATScore> listModel)
        {
            Reading = listModel.Where(r => r.SATType.TypeName == "Reading").OrderBy(r=>r.RawValue).ToList();
            Writing= listModel.Where(r => r.SATType.TypeName == "Writing").OrderBy(r => r.RawValue).ToList();
            Math = listModel.Where(r => r.SATType.TypeName == "Math").OrderBy(r => r.RawValue).ToList();
            MathWithCalculator = listModel.Where(r => r.SATType.TypeName == "Math (Calculator)").OrderBy(r => r.RawValue).ToList();
        }
    }
}