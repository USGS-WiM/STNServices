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

    public partial class status_type
    {
        public status_type() {}

        [Key]
        public int status_type_id { get; set; }
        [Required]
        public string status { get; set; }
    
        public virtual ICollection<instrument_status> instrument_status { get; set; }
    }
}