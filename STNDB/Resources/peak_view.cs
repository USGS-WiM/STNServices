using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace STNDB.Resources
{
    public partial class peak_view
    {
        public peak_view() {} 

        [Key]
        public Int32 peak_summary_id { get; set; }
        [Required]
        public Nullable<double> peak_stage { get; set; }
        public Nullable<DateTime> peak_date { get; set; }
        public string datum_name { get; set; }
        public Nullable<Int32> site_id { get; set; }
        public Nullable<double> latitude { get; set; }
        public Nullable<double> longitude { get; set; }
        public string event_name { get; set; }
    }

}