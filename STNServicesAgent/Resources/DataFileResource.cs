//------------------------------------------------------------------------------
//----- Domestic ---------------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2017 WiM - USGS

//    authors:  Jeremy K. Newson USGS Web Informatics and Mapping
//              
//  
//   purpose:   Represents Wateruse Domestic wateruse addition
//
//discussion:   Simple POCO object class  
//
// 
using System;
using System.Collections.Generic;
using System.Text;
using STNDB.Resources;

namespace STNAgent.Resources
{
    public class dataFile_view
    {
        public Int32 site_id { get; set; }
        public string site_no { get; set; }
        public double latitude_dd { get; set; }
        public double longitude_dd { get; set; }
        public string county { get; set; }
        public string state { get; set; }
        public string waterbody { get; set; }
        public Nullable<Int32> instrument_id { get; set; }
        public Int32 event_id { get; set; }
        public Nullable<Int32> sensor_type_id { get; set; }
        public string sensor { get; set; }
        public Nullable<Int32> deployment_type_id { get; set; }
        public string method { get; set; }
        public string location_description { get; set; }
        public Nullable<Int32> sensor_brand_id { get; set; }
        public string brand_name { get; set; }
        public Int32 file_id { get; set; }
        public string name { get; set; }
        public string script_parent { get; set; }
        public Nullable<DateTime> file_date { get; set; }
        public Nullable<Int32> data_file_id { get; set; }
        public Nullable<DateTime> good_start { get; set; }
        public Nullable<DateTime> good_end { get; set; }
        public Nullable<DateTime> collect_date { get; set; }
    }
}
