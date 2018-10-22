//------------------------------------------------------------------------------
//----- HttpController ---------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2017 WiM - USGS

//    authors:  Jeremy K. Newson USGS Web Informatics and Mapping
//              Tonia Roddick USGS Web Informatics and Mapping
//  
//   purpose:   Handles resources through the HTTP uniform interface.
//
//discussion:   Controllers are objects which handle all interaction with resources. 
//              
//
// 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using STNDB.Resources;
using STNAgent;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace STNServices.Controllers
{
    [Route("[controller]")]
    public class CountiesController : STNControllerBase
    {
        public CountiesController(ISTNServicesAgent sa) : base(sa)
        {}

        #region METHODS
        
        #region GET
        [HttpGet]
        [HttpGet("/Events/{eventid}/[controller]")]
        public async Task<IActionResult> Get(int? eventid = null)
        {

            IQueryable query = null;

            try
            {

                if (eventid.HasValue)
                {
                    query = agent.Select<sites>()
                                .Include(c => c.county)
                                .Include(s => s.hwms).Include(s => s.instruments)
                                .Where(s => s.hwms.Any(h => h.event_id == eventid) || s.instruments.Any(i => i.event_id == eventid))
                                .Select( ct => ct.county ).Distinct();
                
                
                //query = agent.Select<county>().Join(
                    //agent.Select<sites>()
                    // .Include(s => s.hwms).Include(s => s.instruments)
                    //  .Where(s => s.hwms.Any(h => h.event_id == eventid) || s.instruments.Any(i => i.event_id == eventid)),
                    //    c => c.county_fip, s => s.county_fips, (cty, ste) =>  cty.state_fip).Distinct();              
                } else
                {
                    query = agent.Select<county>();
                }

                //sm(agent.Messages);
                return Ok(query);
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id < 0) return new BadRequestResult(); 
                //sm(agent.Messages);
                return Ok(await agent.Find<county>(id));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpGet("/States/{statefips}/[controller]")]
        public async Task<IActionResult> GetStateCounties(int statefips)
        {
            try
            {
                if (statefips < 0) return new BadRequestResult(); // This returns HTTP 404

                var objectsRequested = agent.Select<states>().Include(s=> s.counties).FirstOrDefault(x => x.fips_code == statefips).counties;
                if (objectsRequested == null) return new BadRequestObjectResult(new Error(errorEnum.e_notFound));
                //sm(agent.Messages);
                return Ok(objectsRequested);
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }
                
        [HttpGet("/States/[controller]")] //(stateResource+"/"+countyResource+"?StateAbbrev={stateAbbrev}")
        public async Task<IActionResult> GetStateCountiesByAbbrev([FromQuery] string StateAbbrev)
        {
            try
            {
                if (string.IsNullOrEmpty(StateAbbrev)) return new BadRequestResult(); // This returns HTTP 404

                var objectsRequested = agent.Select<states>().Include(s=>s.counties).FirstOrDefault(x => x.state_abbrev == StateAbbrev).counties;
                if (objectsRequested == null) return new BadRequestObjectResult(new Error(errorEnum.e_notFound)); 

                //sm(agent.Messages);
                return Ok(objectsRequested);
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpGet("/Sites/CountiesByState")] //(siteResource+"/CountiesByState?StateAbbrev={stateAbbrev}")
        public async Task<IActionResult> GetStateSiteCounties([FromQuery] string StateAbbrev)
        {
            try
            {
                if (string.IsNullOrEmpty(StateAbbrev)) return new BadRequestResult();
                var siteCounties = agent.Select<sites>().Include(st => st.state).Include(c => c.county).Where(s => s.state.state_abbrev == StateAbbrev).Select(s => s.county);
                //sm(agent.Messages);
                return Ok(siteCounties);
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        #endregion

        #region POST
        [HttpPost][Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Post([FromBody]county entity)
        {
            try
            { 
                if (!isValid(entity)) return new BadRequestResult();
                // put it here until I can figure out the regex in the class
                if (!entity.county_name.Contains(" Parish") && !entity.county_name.Contains(" County") && !entity.county_name.Contains(" Municipio"))
                    return new BadRequestObjectResult("Invalid county name. County name must contain: 'Parish', 'County' or 'Municipio'.");
                //sm(agent.Messages);
                return Ok(await agent.Add<county>(entity));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpPost][Authorize(Policy = "AdminOnly")]
        [Route("Batch")]
        public async Task<IActionResult> Batch([FromBody]List<county> entities)
        {
            try
            {
                if (!isValid(entities)) return new BadRequestObjectResult("Object is invalid");
                //  figure out regex in class
                
                //sm(agent.Messages);
                return Ok(await agent.Add<county>(entities));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }
        #endregion
       
        #region PUT
        [HttpPut("{id}")][Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Put(int id, [FromBody]county entity)
        {
            try
            {
                if (id < 0 || !isValid(entity)) return new BadRequestResult(); // This returns HTTP 404
                //figure out regex
                if (!entity.county_name.Contains(" Parish") && !entity.county_name.Contains(" County") && !entity.county_name.Contains(" Municipio"))
                    new BadRequestObjectResult("Invalid county name. County name must contain: 'Parish', 'County' or 'Municipio'.");

                //sm(agent.Messages);
                return Ok(await agent.Update<county>(id, entity));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
           
        }
        #endregion
        
        #region DELETE
        [HttpDelete("{id}")][Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id < 1) return new BadRequestResult();
                var entity = await agent.Find<county>(id);
                if (entity == null) return new NotFoundResult();

                await agent.Delete<county>(entity);
                //sm(agent.Messages);
                return Ok();

            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }
        #endregion
        
        #endregion
        
        #region HELPER METHODS
        #endregion
    }
}
