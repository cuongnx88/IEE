using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEE.ViewModel
{
    public class MediaViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public string FileSource { get; set; }
        public string Link { get; set; }
        public int Type { get; set; }
    }
}
