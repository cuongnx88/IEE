using IEE.Infrastructure.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IEE.Web.Areas.ttn_content.Models
{
    public class TestViewModel
    {
        public SATTest Test { get; set; }
        public List<SATUserAnswer> ListAnswer { get; set; }
        public int Section { get; set; }
        public int Content { get; set; }
        public string TypeName { get; set; }
        public TestViewModel()
        {

        }
        public TestViewModel(SATTest model)
        {
            Test = model;
        }
    }
}