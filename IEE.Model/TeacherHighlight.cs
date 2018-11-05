using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEE.Model
{
    [MetadataType(typeof(TeacherHighlightMetadata))]
    public partial class TeacherHighlight
    {
        public class TeacherHighlightMetadata
        {
            public string Name { get; set; }
            public string Photo { get; set; }
            public string Title { get; set; }
            public string Position { get; set; }
            public string Experience { get; set; }
            public string Summarize { get; set; }
            public string Detail { get; set; }
            [Range(0, int.MaxValue, ErrorMessage = "Hãy nhập số nguyên dương")]
            public int DisplayIndex { get; set; }
        }
    }
}
