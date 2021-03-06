//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace STNDB.Resources
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class landownercontact
    {
        public landownercontact() {}

        [Key]
        public int landownercontactid { get; set; }
        [Required]
        public string fname { get; set; }
        [Required]
        public string lname { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        [Phone]
        public string primaryphone { get; set; }
        [Phone]
        public string secondaryphone { get; set; }
        [EmailAddress]
        public string email { get; set; }
        public string title { get; set; }
        public Nullable<System.DateTime> last_updated { get; set; }
        public Nullable<int> last_updated_by { get; set; }
    
        public virtual ICollection<sites> sites { get; set; }
    }
}
