
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEE.ViewModel
{
    public class BannerViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "Bạn phải nhập tên banner")]
        public string Name { get; set; }
        public string Summarize { get; set; }
        public string Photo { get; set; }
        public string Detail { get; set; }
        public bool IsLock { get; set; }
        public bool JustPhoto { get; set; }
        public bool IsHeader { get; set; }
        public string LinkYoutube { get; set; }
        public int? BannerIndex { get; set; }
        public string Link { get; set; }
        public string Position { get; set; }

    }
}
