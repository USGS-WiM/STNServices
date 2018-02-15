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

    public partial class reporting_metrics
    {
        public reporting_metrics() {}

        [Key]
        public int reporting_metrics_id { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public System.DateTime report_date { get; set; }
        [Required]
        public int event_id { get; set; }
        [Required]
        public string state { get; set; }
        public double sw_fieldpers_notacct { get; set; }
        public double wq_fieldpers_notacct { get; set; }
        public double gage_visit { get; set; }
        public double gage_down { get; set; }
        public double tot_discharge_meas { get; set; }
        public double plan_discharge_meas { get; set; }
        public double plan_indirect_meas { get; set; }
        public double rating_extens { get; set; }
        public double gage_peak_record { get; set; }
        public double plan_rapdepl_gage { get; set; }
        public double dep_rapdepl_gage { get; set; }
        public double rec_rapdepl_gage { get; set; }
        public double lost_rapdepl_gage { get; set; }
        public double plan_wtrlev_sensor { get; set; }
        public double dep_wtrlev_sensor { get; set; }
        public double rec_wtrlev_sensor { get; set; }
        public double lost_wtrlev_sensor { get; set; }
        public double plan_wv_sens { get; set; }
        public double dep_wv_sens { get; set; }
        public double rec_wv_sens { get; set; }
        public double lost_wv_sens { get; set; }
        public double plan_barometric { get; set; }
        public double dep_barometric { get; set; }
        public double rec_barometric { get; set; }
        public double lost_barometric { get; set; }
        public double plan_meteorological { get; set; }
        public double dep_meteorological { get; set; }
        public double rec_meteorological { get; set; }
        public double lost_meteorological { get; set; }
        public double hwm_flagged { get; set; }
        public double hwm_collected { get; set; }
        public double qw_discr_samples { get; set; }
        public double coll_sedsamples { get; set; }
        public string notes { get; set; }
        [Required]
        public double member_id { get; set; }
        public Nullable<double> complete { get; set; }
        public Nullable<double> yest_fieldpers { get; set; }
        public Nullable<double> tod_fieldpers { get; set; }
        public Nullable<double> tmw_fieldpers { get; set; }
        public Nullable<double> yest_officepers { get; set; }
        public Nullable<double> tod_officepers { get; set; }
        public Nullable<double> tmw_officepers { get; set; }
        public Nullable<System.DateTime> last_updated { get; set; }
        public Nullable<int> last_updated_by { get; set; }
    
        public virtual ICollection<reportmetric_contact> reportmetric_contact { get; set; }
    }
}
