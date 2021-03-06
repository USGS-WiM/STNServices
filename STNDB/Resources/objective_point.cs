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

    public partial class objective_point
    {
        public objective_point() {}

        [Key]
        public int objective_point_id { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public string description { get; set; }
        public Nullable<double> elev_ft { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> date_established { get; set; }
        public Nullable<double> op_is_destroyed { get; set; }
        public string op_notes { get; set; }
        [Required]
        public Nullable<int> site_id { get; set; }
        [Required]
        public Nullable<int> vdatum_id { get; set; }

        [ForeignKey("vdatum_id")]
        public virtual vertical_datums vertical_datums { get; set; }
        [Required]
        public Nullable<double> latitude_dd { get; set; }
        [Required]
        public Nullable<double> longitude_dd { get; set; }
        public Nullable<int> hdatum_id { get; set; }

        [ForeignKey("hdatum_id")]
        public virtual horizontal_datums horizontal_datums { get; set; }

        public Nullable<int> hcollect_method_id { get; set; }
        public Nullable<int> vcollect_method_id { get; set; }
        [Required]
        public int op_type_id { get; set; }

        [ForeignKey("op_type_id")]
        public virtual objective_point_type objective_point_type { get; set; }
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> date_recovered { get; set; }
        public Nullable<double> uncertainty { get; set; }
        public string unquantified { get; set; }
        public Nullable<int> op_quality_id { get; set; }
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> last_updated { get; set; }
        public Nullable<int> last_updated_by { get; set; }
    
        public virtual op_quality op_quality { get; set; }
        public virtual sites site { get; set; }
        public virtual ICollection<op_measurements> op_measurements { get; set; }
        public virtual ICollection<op_control_identifier> op_control_identifier { get; set; }        
        public virtual ICollection<file> files { get; set; }
    }
}
