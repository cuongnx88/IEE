//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IEE.Infrastructure.DbContext
{
    using System;
    using System.Collections.Generic;
    
    public partial class SATWritingUnderlineText
    {
        public int ID { get; set; }
        public string UnderlineText { get; set; }
        public int QuestionID { get; set; }
        public int Number { get; set; }
    
        public virtual SATQuestion SATQuestion { get; set; }
    }
}
