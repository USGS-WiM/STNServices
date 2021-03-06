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
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class instrument_status
    {
        public instrument_status() { }

        [Key]
        public int instrument_status_id { get; set; }
        [Required]
        public Nullable<int> status_type_id { get; set; }
        [Required]
        public Nullable<int> instrument_id { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> time_stamp { get; set; }
        public string notes { get; set; }
        [Required]
        public string time_zone { get; set; }
        [Required]
        public Nullable<int> member_id { get; set; }
        public Nullable<double> sensor_elevation { get; set; }
        public Nullable<double> ws_elevation { get; set; }
        public Nullable<double> gs_elevation { get; set; }
        public Nullable<int> vdatum_id { get; set; }

        [ForeignKey("vdatum_id")]
        public virtual vertical_datums vertical_datums { get; set; }

        public Nullable<System.DateTime> last_updated { get; set; }
        public Nullable<int> last_updated_by { get; set; }
    
        public virtual status_type status_type { get; set; }
        public virtual instrument instrument { get; set; }
        
        public virtual members members { get; set; }
    }
}
