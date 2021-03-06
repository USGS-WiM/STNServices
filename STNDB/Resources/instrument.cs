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

    public partial class instrument
    {
        public instrument() {}

        [Key]
        public int instrument_id { get; set; }
        [Required]
        public Nullable<int> sensor_type_id { get; set; }
        public Nullable<int> deployment_type_id { get; set; }
        public string location_description { get; set; }
        public string serial_number { get; set; }
        public string housing_serial_number { get; set; }
        public Nullable<double> interval { get; set; }
        [Required]
        public Nullable<int> site_id { get; set; }
        public Nullable<int> event_id { get; set; }
        public Nullable<int> inst_collection_id { get; set; }

        [ForeignKey("inst_collection_id")]
        public virtual instr_collection_conditions instr_collection_conditions { get; set; }

        public Nullable<int> housing_type_id { get; set; }
        public Nullable<int> sensor_brand_id { get; set; }
        public string vented { get; set; }
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> last_updated { get; set; }
        public Nullable<int> last_updated_by { get; set; }
    
        
        public virtual sensor_brand sensor_brand { get; set; }
        public virtual sensor_type sensor_type { get; set; }
        public virtual housing_type housing_type { get; set; }
        public virtual deployment_type deployment_type { get; set; }
        public virtual ICollection<instrument_status> instrument_status { get; set; }
        public virtual ICollection<data_file> data_files { get; set; }
        public virtual ICollection<file> files { get; set; }
        public virtual events @event { get; set; }
        public virtual sites site { get; set; }
    }
}
