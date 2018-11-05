using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEE.ViewModel
{
    public class UniversityHighlightViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public string Photo { get; set; }
        public string Address { get; set; }
        public string Summarize { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }
        public int Ranking { get; set; }
    }
}
