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
    public class AgenciesController : STNControllerBase
    {
        public AgenciesController(ISTNServicesAgent sa) : base(sa)
        {}

        #region METHODS
        
        #region GET
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
               // //sm(agent.Messages);
                return Ok(agent.Select<agency>().OrderBy(e=>e.agency_name));
            }
            catch (Exception ex)
            {
              //  //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id < 0) return new BadRequestObjectResult("Invalid input parameters"); 
               // //sm(agent.Messages); //X-USGSWiM-Messages
                return Ok(await agent.Find<agency>(id));
            }
            catch (Exception ex)
            {
             //   //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpGet("/Members/{memberId}/[controller]")]
        public async Task<IActionResult> GetMemberAgency(int memberId)
        {
            try
            {
                if (memberId < 0) return new BadRequestObjectResult("Invalid input parameters");
                var objectRequested = agent.Select<members>().Include(m => m.agency).FirstOrDefault(x => x.member_id == memberId).agency;
                if (objectRequested == null) return new BadRequestObjectResult(new Error(errorEnum.e_notFound)); // This returns HTTP 400

              //  //sm(agent.Messages);
                return Ok(objectRequested);
            }
            catch (Exception ex)
            {
              //  //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpGet("/Sources/{sourceId}/[controller]")]
        public async Task<IActionResult> GetSourceAgency(int sourceId)
        {
            try
            {
                if (sourceId < 0) return new BadRequestObjectResult("Invalid input parameters"); // This returns HTTP 404

                var objectRequested = agent.Select<sources>().Include(m => m.agency).FirstOrDefault(x => x.source_id == sourceId).agency;
                if (objectRequested == null) return new BadRequestObjectResult(new Error(errorEnum.e_notFound));

              //  //sm(agent.Messages);
                return Ok(objectRequested);
            }
            catch (Exception ex)
            {
              //  //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }
        #endregion

        #region POST
        [HttpPost][Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Post([FromBody]agency entity)
        {
            try
            {
                if (!isValid(entity)) return new BadRequestObjectResult("Invalid input parameters"); // This returns HTTP 404
                //sm(agent.Messages);
                return Ok(await agent.Add<agency>(entity));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpPost][Authorize(Policy = "AdminOnly")]
        [Route("Batch")]
        public async Task<IActionResult> Batch([FromBody]List<agency> entities)
        {
            try
            {
                if (!isValid(entities)) return new BadRequestObjectResult("Object is invalid");

                //sm(agent.Messages);
                return Ok(await agent.Add<agency>(entities));
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
        public async Task<IActionResult> Put(int id, [FromBody]agency entity)
        {
            try
            {
                if (id < 0 || !isValid(entity)) return new BadRequestObjectResult("Invalid input parameters");

                //sm(agent.Messages);
                return Ok(await agent.Update<agency>(id, entity));
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
                if (id < 1) return new BadRequestObjectResult("Invalid input parameters");
                var entity = await agent.Find<agency>(id);
                if (entity == null) return new NotFoundResult();

                await agent.Delete<agency>(entity);
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
