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
    public class SensorType : sensor_type
    {
        public List<deployment_type> deploymenttypes { get; set; }
    }
}
