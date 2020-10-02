namespace STNDB.Resources
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    //[Keyless] ///add after upgrade to 2.1
    public partial class sensor_event:Isensor
    {
        public sensor_event() { }

        [Key]
        public int site_id { get; set; }
        public string site_no { get; set; }
        public string site_name { get; set; }
        public double latitude_dd { get; set; }
        public double longitude_dd { get; set; }
        public string county { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public int instrument_id { get; set; }
        public Nullable<int> event_id { get; set; }
        public string event_name { get; set; }
        public string status { get; set; }
        public DateTime time_stamp { get; set; }
        public Nullable<int> event_type_id { get; set; }
        public Nullable<int> event_status_id { get; set; }
        public Nullable<int> inst_collection_id { get; set; }
        public Nullable<int> status_type_id { get; set; }
        public Nullable<int> deployment_type_id { get; set; }
        public DateTime event_start_date {get; set; }
        public Nullable <DateTime> event_end_date {get; set; }
        public Nullable <int> sensor_type_id {get; set; }
        public string sensor {get; set; }
        public string method {get; set; }

    }
}
