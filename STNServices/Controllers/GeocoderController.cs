//------------------------------------------------------------------------------
//----- HttpController ---------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2017 WiM - USGS

//    authors:  Tonia Roddick USGS Web Informatics and Mapping
//              
//  
//   purpose:   Handles resources through the HTTP uniform interface.
//
//discussion:   Controllers are objects which handle all interaction with resources. 
//              
//
// 

using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using STNDB.Resources;
using STNAgent;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace STNServices.Controllers
{
    [Route("[controller]")]
    public class GeocodeController : STNControllerBase
    {
        public GeocodeController(ISTNServicesAgent sa) : base(sa)
        {}

        #region METHODS

        #region GET
        //Geocode/location?Latitude={latitude}&Longitude={longitude}")
        [HttpGet("location")]
        public async Task<IActionResult> Get([FromQuery] float Latitude, [FromQuery] float Longitude)
        {
            try
            {
                if (Latitude < 0 || Longitude > 0) return new BadRequestResult();
                //sm(agent.Messages);
                using (var client = new HttpClient())
                {
                    try
                    {
                        //without this, get ssl/tsl error
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                        client.BaseAddress = new Uri("https://geocoding.geo.census.gov");
                        var response = await client.GetAsync($"/geocoder/geographies/coordinates?x={Longitude}&y={Latitude}&benchmark=4&vintage=4&format=json");
                        response.EnsureSuccessStatusCode();

                        var stringResult = await response.Content.ReadAsStringAsync();
                        var rawResponse = JsonConvert.DeserializeObject<object>(stringResult);
                        return Ok(rawResponse);
                    }
                    catch (HttpRequestException httpRequestException)
                    {
                        return BadRequest($"Error getting location from census.gov: {httpRequestException.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
              //  //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }
        
        
        #endregion

     
        #endregion
        
        #region HELPER METHODS
        #endregion
    }
}
