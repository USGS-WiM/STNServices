using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using WiM.Codecs.CSV;
using STNAgent.Resources;

namespace STNServices.Codecs.CSV
{
    public class STNCSVOutputFormater: CSVOutputFormatter
    {
        protected override bool CanWriteType(Type type)
        {

            if (type == null)
                throw new ArgumentNullException("type");

            return type == typeof(Dictionary<string, Wateruse>);
        }

        public STNCSVOutputFormater():base()
        { }
        protected override void writeStream(Type type, object value, Stream stream)
        {
            StringWriter sWriter = null;
            try
            {

                sWriter = this.getSourceWateruseCSV((IDictionary<string, Wateruse>)value);              
                
                var streamWriter = new StreamWriter(stream);
                streamWriter.Write(sWriter.ToString());
                streamWriter.Flush();
            }
            catch (Exception ex)
            {
                var x = ex;
                throw;
            }
        }

        private StringWriter getSourceWateruseCSV(IDictionary<string, Wateruse> value) {
            try
            {
                //Type itemType = type.GetGenericArguments()[0];

                StringWriter stringWriter = new StringWriter();


                stringWriter.WriteLine(string.Join<string>(",", new string[] { "Time", value.FirstOrDefault().Value.ProcessDate.ToString() }));
                stringWriter.WriteLine(string.Join<string>(",", new string[] { "StartYear", value.FirstOrDefault().Value.StartYear.ToString() }));
                stringWriter.WriteLine(string.Join<string>(",", new string[] { "EndYear", value.FirstOrDefault().Value.EndYear.HasValue? value.FirstOrDefault().Value.EndYear.Value.ToString(): value.FirstOrDefault().Value.StartYear.ToString() }));
                stringWriter.WriteLine(string.Join<string>(",", new string[] { "Units", "Million Gallons per Day" }));
                stringWriter.WriteLine();//blank line

                //headers
                var headers = new List<string>() { "FacilityCode", "Facility_Type", "Use_Type", "ANN_Total" }.Concat( value.Where(w =>
                             w.Value.Return != null).SelectMany(wu => wu.Value.Return.Monthly.SelectMany(mon =>
                             mon.Value.Month.Select(v => mon.Key.ToString("00")))).Concat(
                        value.Where(w =>
                             w.Value.Withdrawal != null).SelectMany(wwu => wwu.Value.Withdrawal.Monthly.SelectMany(mon =>
                             mon.Value.Month.Select(v => mon.Key.ToString("00"))))).Distinct().OrderBy(s => s)).ToList();               
                        
                stringWriter.WriteLine(String.Join(",", headers));

                foreach (var obj in (IDictionary<string, STNAgent.Resources.Wateruse>)value)
                {
                    if (obj.Value.Return != null)
                        stringWriter.WriteLine(string.Join(",", getStringLine(headers, obj.Value.Return, obj.Key, "Return")));

                    if (obj.Value.Withdrawal != null)
                        stringWriter.WriteLine(string.Join(",", getStringLine(headers, obj.Value.Withdrawal, obj.Key, "Withdrawal")));
                }//next obj

                return stringWriter;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private string[] getStringLine(List<string> headers, WateruseSummary summary, string name, string type) {
            string[] line = new string[headers.Count];
            try
            {
                if (summary == null || headers.Count < 1 )return line;

                line = new string[headers.Count];
                line[0] = name;
                line[1] = summary.Annual.First().Key;
                line[2] = type;
                summary.Annual.ToList().ForEach(ann => line[headers.IndexOf("ANN_Total")] = ann.Value.Value.ToString("N6"));

                summary.Monthly.Where(c => c.Value.Month != null).SelectMany(m => m.Value.Month.Select(wu =>
                    new { key = m.Key.ToString("00"), value = wu.Value.Value }))
                    .ToList().ForEach(item => line[headers.IndexOf(item.key)] = item.value.ToString("N6"));
                return line;
            }
            catch (Exception ex)
            {

                return line;
            }


        }
        private string join(string str, string _val) {
            if (!string.IsNullOrEmpty(_val))
            {

                //Check if the value contans a comma and place it in quotes if so
                if (_val.Contains(","))
                    _val = string.Concat("\"", _val, "\"");

                //Replace any \r or \n special characters from a new line with a space
                if (_val.Contains("\r"))
                    _val = _val.Replace("\r", " ");
                if (_val.Contains("\n"))
                    _val = _val.Replace("\n", " ");

                return string.Concat(str, _val, ",");
            }
            else
            {

                return string.Concat(str, string.Empty, ",");
            }//end if
        }
        
    }   
}
