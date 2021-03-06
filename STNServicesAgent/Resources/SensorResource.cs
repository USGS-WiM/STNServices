﻿//------------------------------------------------------------------------------
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
    public class FullInstrument : instrument
    {
        public string sensorType { get; set; }
        public string deploymentType { get; set; }
        public string instCollection { get; set; }
        public string housingType { get; set; }
        public string sensorBrand { get; set; }

    }

    public class Instrument_Status : instrument_status
    {
        public string status { get; set; }
        public string vdatum { get; set; }

    }

    public class InstrumentDownloadable : instrument
    {
        public string sensorType { get; set; }
        public string deploymentType { get; set; }
        public string eventName { get; set; }
        public string collectionCondition { get; set; }
        public string housingType { get; set; }
        public string sensorBrand { get; set; }
        public Nullable<Int32> statusId { get; set; }
        public Nullable<DateTime> timeStamp { get; set; }
        public string site_no { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string siteDescription { get; set; }
        public string networkNames { get; set; }
        public string stateName { get; set; }
        public string countyName { get; set; }
        public string siteWaterbody { get; set; }
        public string siteHDatum { get; set; }
        public string sitePriorityName { get; set; }
        public string siteZone { get; set; }
        public string siteHCollectMethod { get; set; }
        public string sitePermHousing { get; set; }
    }//end class InstrumentDownloadable

    public class sensor_view
    {
        public Int32 site_id { get; set; }
        public string site_no { get; set; }
        public string site_name { get; set; }
        public double latitude_dd { get; set; }
        public double longitude_dd { get; set; }
        public string county { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public Nullable<Int32> instrument_id { get; set; }
        public Nullable<Int32> sensor_type_id { get; set; }
        public Nullable<Int32> event_id { get; set; }
        public string event_name { get; set; }
        public Nullable<Int32> status_type_id { get; set; }
        public Nullable<Int32> deployment_type_id { get; set; }
        public string status { get; set; }
        public Nullable<DateTime> time_stamp { get; set; }
        public Nullable<Int32> inst_collection_id { get; set; }
        public Nullable<Int32> event_type_id { get; set; }
        public Nullable<Int32> event_status_id { get; set; }
    }
}
