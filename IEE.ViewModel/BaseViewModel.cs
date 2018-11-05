using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IEE.ViewModel
{
    public class BaseViewModel
    {
        public int Id { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public Nullable<DateTime> CreatedDate
        {
            get; set;
        }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}