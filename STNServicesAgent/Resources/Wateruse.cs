//------------------------------------------------------------------------------
//----- Wateruse ---------------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2017 WiM - USGS

//    authors:  Jeremy K. Newson USGS Web Informatics and Mapping
//              
//  
//   purpose:   Represents Wateruse object
//
//discussion:   Simple POCO object class  
//
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STNAgent.Resources
{
    public class Wateruse
    {
        public Int32 StartYear { get; set; }
        public Int32? EndYear { get; set; }
        public bool ShouldSerializeEndYear()
        { return EndYear.HasValue; }
        public DateTime ProcessDate { get; set; }
        public WateruseSummary Return { get; set; }
        public bool ShouldSerializeReturn()
        { return Return != null; }
        public WateruseSummary Withdrawal { get; set; }
    }

    public class WateruseSummary {
        public IDictionary<string, WateruseValue> Annual { get; set; }
        public IDictionary<Int32, MonthlySummary> Monthly { get; set; }
        public IDictionary<string, WateruseValue> Permitted { get; set; }
        public bool ShouldSerializePermitted()
        { return Permitted != null && Permitted.Count > 0; }
    }

    public class MonthlySummary {
        public IDictionary<string, WateruseValue> Month { get; set; }
        public IDictionary<string,WateruseValue> Code { get; set; }
        public bool ShouldSerializeCode()
        { return Code != null && Code.Count > 0; }
    }

    public class WateruseValue {
        public string Name { get; set; }
        public double Value { get; set; }
        public object Unit { get; set; }
        public string Description { get; set; }
    }
}
