using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STNDB.Resources
{
    public interface Isensor 
    {
        int site_id { get; set; }
        string site_no { get; set; }
        string site_name { get; set; }
        double latitude_dd { get; set; }
        double longitude_dd { get; set; }
        string county { get; set; }
        string state { get; set; }
        string city { get; set; }
        int instrument_id { get; set; }
        Nullable<int> event_id { get; set; }
        string event_name { get; set; }
        string status { get; set; }
        DateTime time_stamp { get; set; }
        Nullable<int> event_type_id { get; set; }
        Nullable<int> event_status_id { get; set; }
        Nullable<int> inst_collection_id { get; set; }
        Nullable<int> status_type_id { get; set; }
        Nullable<int> deployment_type_id { get; set; }
        Nullable<int> sensor_type_id { get; set; }
    }

    public class sensor:Isensor
    {
        public sensor() { }

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
        public Nullable<int> sensor_type_id { get; set; }
    }
}
