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
    public class HWMDownloadable : hwm
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string eventName { get; set; }
        public string hwmTypeName { get; set; }
        public string hwmQualityName { get; set; }
        public string verticalDatumName { get; set; }
        public string verticalMethodName { get; set; }
        public string approvalMember { get; set; }
        public string markerName { get; set; }
        public string horizontalMethodName { get; set; }
        public string horizontalDatumName { get; set; }
        public string flagMemberName { get; set; }
        public string surveyMemberName { get; set; }
        public string site_no { get; set; }
        public string siteDescription { get; set; }
        public string sitePriorityName { get; set; }
        public string networkNames { get; set; }
        public string stateName { get; set; }
        public string countyName { get; set; }
        public string siteZone { get; set; }
        public string sitePermHousing { get; set; }
        public double site_latitude { get; set; }
        public double site_longitude { get; set; }
    }//end class HWMDownloadable
}
