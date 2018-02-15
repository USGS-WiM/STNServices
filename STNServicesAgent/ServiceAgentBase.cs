//------------------------------------------------------------------------------
//----- Service Agent Base ---------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2017 WiM - USGS

//    authors:  Jeremy K. Newson USGS Web Informatics and Mapping
//              
//  
//   purpose:   The service agentbase is responsible for initiating the service call, 
//              capturing the data that's returned and forwarding the data back to 
//              the requestor.
//
//discussion:   delegated hunting and gathering responsibilities.   
//              Primary responsibility is for http requests
// 
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using System.IO;

namespace WiM.Utilities.ServiceAgent
{
    public abstract class ServiceAgentBase
    {
        #region "Events"

        #endregion

        #region Properties & Fields
        private HttpClient client;
        #endregion

        #region Constructors
        public ServiceAgentBase(string BaseUrl)
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(BaseUrl);
        }
        #endregion

        #region Methods
        public async Task<T> ExecuteAsync<T>(RequestInfo request) where T : new()
        {
            try
            {
                HttpResponseMessage response=null;
                T result = default(T);
                switch (request.Method)
                {
                    case methodType.e_GET:
                        response = await client.GetAsync(request.RequestURI);
                        break;
                    case methodType.e_POST:
                        response = await client.PostAsync(request.RequestURI, request.Content);
                        break;
                    case methodType.e_PUT:
                        response = await client.PutAsync(request.RequestURI, request.Content);
                        break;
                    case methodType.e_DELETE:
                        response = await client.DeleteAsync(request.RequestURI);
                        break;
                }//end switch

                if (response == null) throw new Exception("http request invalid");
                //throws an exception if not 200
                response.EnsureSuccessStatusCode();

                var stream = await response.Content.ReadAsStreamAsync();
                if (stream != null)
                    result = DeserializeStream<T>(stream);

                return result;
            }
            catch (HttpRequestException ex)
            {
                throw;
            }

        }//end ExecuteAsync<T>

        #endregion
        #region Helper Methods
        private T DeserializeStream<T>(Stream stream)
        {
            using (var sr = new StreamReader(stream))
            {
                using (var jsonTextReader = new JsonTextReader(sr))
                {
                    var serializer = new JsonSerializer();
                    return serializer.Deserialize<T>(jsonTextReader);
                }//end using
            }//end using
        }
        private static void Serialize<T>(T value, Stream s)
        {
            using (StreamWriter writer = new StreamWriter(s))
            using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
            {
                JsonSerializer ser = new JsonSerializer();
                ser.Serialize(jsonWriter, value);
                jsonWriter.Flush();
            }
        }
        #endregion

    }//end class ServiceAgentBase

    public class RequestInfo
    {
        public methodType Method { get; set; }
        public string RequestURI { get; set; }
        public string DataType { get; set; }
        public HttpContent Content { get; set; }

        public RequestInfo(string uri, methodType mtd = methodType.e_GET) {
            this.RequestURI = uri;
            this.Method = mtd;            
        }
        public RequestInfo(string uri, Object data,contentType type = contentType.JSON,  methodType mtd = methodType.e_GET)
        {
            this.RequestURI = uri;
            this.Method = mtd;
            switch (type)
            {
                case contentType.JSON:
                    this.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, GetMediaType(type));
                    break;
                case contentType.XML:
                    break;
            }
            
        }
        private string GetMediaType(contentType type) {
            switch (type)
            {
                case contentType.JSON:
                    return "application/json";
                case contentType.XML:
                    return "application/xml";
            }

            return "application/json";
        }
    }
    public enum methodType
    {
        e_GET,
        e_POST,
        e_PUT,
        e_DELETE
    }
    public enum contentType
    {
        JSON,
        XML
    }
}
