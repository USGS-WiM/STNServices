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
    public class StatesController : STNControllerBase
    {
        public StatesController(ISTNServicesAgent sa) : base(sa)
        {}

        #region METHODS
        
        #region GET
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                //sm(agent.Messages);
                return Ok(agent.Select<states>());
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
                return Ok(await agent.Find<states>(id));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }
        
        // returns a list of all the states we have sites in
        [HttpGet("/Sites/[controller]")]
        public async Task<IActionResult> GetSiteStates()
        {
            try
            {
                IQueryable<sites> siteList = agent.Select<sites>();
                var uniqueSites = siteList.GroupBy(p => p.state).Select(g => g.FirstOrDefault()).Select(x => x.state);
                var objectsRequested = agent.Select<states>().Where(e => uniqueSites.Contains(e.state_abbrev));
                                
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
        
        #endregion

        #region POST
        [HttpPost][Authorize(Policy = "CanModify")]
        public async Task<IActionResult> Post([FromBody]states entity)
        {
            try
            {
                if (!isValid(entity)) return new BadRequestResult();
                //sm(agent.Messages);
                return Ok(await agent.Add<states>(entity));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }
                
        [HttpPost][Authorize(Policy = "CanModify")]
        [Route("Batch")]
        public async Task<IActionResult> Batch([FromBody]List<states> entities)
        {
            try
            {
                if (!isValid(entities)) return new BadRequestObjectResult("Object is invalid");
                //sm(agent.Messages);
                return Ok(await agent.Add<states>(entities));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }
        #endregion
       
        #region PUT
        [HttpPut("{id}")][Authorize(Policy = "CanModify")]
        public async Task<IActionResult> Put(int id, [FromBody]states entity)
        {
            try
            {
                if (id < 0 || !isValid(entity)) return new BadRequestResult();
                return Ok(await agent.Update<states>(id, entity));
            }
            catch (Exception ex)
            {
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
                var entity = await agent.Find<states>(id);
                if (entity == null) return new NotFoundResult();

                await agent.Delete<states>(entity);
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
