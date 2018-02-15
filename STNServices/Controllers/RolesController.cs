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
    public class RolesController : STNControllerBase
    {
        public RolesController(ISTNServicesAgent sa) : base(sa)
        {}
        #region METHODS
        #region GET
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(agent.Select<roles>());   
            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex);
            }                 
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if(id<0) return new BadRequestResult(); 

                return Ok(await agent.Find<roles>(id));
            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex);
            }            
        }

        //(memberResource+"/{memberId}/Role")
        [HttpGet("/Members/{memberId}/Role")]
        public async Task<IActionResult> GetMemberRole(int memberId)
        {
            try
            {
                if (memberId < 0) return new BadRequestResult();

                var objectRequested = agent.Select<members>().Include(m => m.roles).FirstOrDefault(x => x.member_id == memberId).roles;
                if (objectRequested == null) return new BadRequestObjectResult(new Error(errorEnum.e_notFound));
                //sm(agent.Messages);
                return Ok(objectRequested);
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
        public async Task<IActionResult> Post([FromBody]roles entity)
        {
            try
            {
                if (!isValid(entity)) return new BadRequestObjectResult("Object is invalid");

                return Ok(await agent.Add<roles>(entity));
            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex);
            }            
        }

        [HttpPost][Authorize(Policy = "AdminOnly")]
        [Route("Batch")]
        public async Task<IActionResult> Batch([FromBody]List<roles> entities) {
            try
            {
                if (!isValid(entities)) return new BadRequestObjectResult("Object is invalid");

                return Ok(await agent.Add<roles>(entities));
            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex);
            }
        }
        #endregion

        #region PUT
        [HttpPut("{id}")][Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Put(int id, [FromBody]roles entity)
        {
            try
            {
                if (!isValid(entity) || id < 1) return new BadRequestResult();
                return Ok(await agent.Update<roles>(id,entity));
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
                var role = await agent.Find<roles>(id);
                if (role == null) return new NotFoundResult();

                await agent.Delete<roles>(role);
                return Ok();

            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex);
            }
            
        }
        #endregion
        #endregion

        #region HELPER METHODS
        #endregion
    }
}
