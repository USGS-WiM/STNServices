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
    public class peak_view
    {
        public Int32 peak_summary_id { get; set; }
        public Nullable<double> peak_stage { get; set; }
        public Nullable<DateTime> peak_date { get; set; }
        public string datum_name { get; set; }
        public Nullable<Int32> site_id { get; set; }
        public Nullable<double> latitude { get; set; }
        public Nullable<double> longitude { get; set; }
        public string event_name { get; set; }

    }//end class peak_view

    public class PeakResource : peak_summary
    {
        public string vdatum { get; set; }
        public string member_name { get; set; }
        public decimal site_id { get; set; }
        public String site_no { get; set; }
        public double latitude_dd { get; set; }
        public double longitude_dd { get; set; }
        public String description { get; set; }
        public string networks { get; set; }
        public String state { get; set; }
        public String county { get; set; }
        public String waterbody { get; set; }
        public String horizontal_datum { get; set; }
        public string priority { get; set; }
        public String zone { get; set; }
        public String horizontal_collection_method { get; set; }
        public String perm_housing_installed { get; set; }
    }
}
