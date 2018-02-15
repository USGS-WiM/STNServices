//------------------------------------------------------------------------------
//----- Configureation ---------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2017 WiM - USGS

//    authors:  Jeremy K. Newson USGS Web Informatics and Mapping
//              
//  
//   purpose:   Represents Wateruse Configureation
//
//discussion:   Simple POCO object class  
//
// 
using System;

namespace STNAgent.Resources
{
    public class Configuration
    {
        public bool HasPermits { get; set; }
        public Int32 MinYear { get; set; }
        public Int32 MaxYear { get; set; }
        public bool HasReturns { get; set; }
        public string Units { get; set; }
    }
}
