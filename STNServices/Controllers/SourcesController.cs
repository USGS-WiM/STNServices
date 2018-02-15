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
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace STNServices.Controllers
{
    [Route("[controller]")]
    public class SourcesController : STNControllerBase
    {
        public SourcesController(ISTNServicesAgent sa):base(sa)
        {}
        #region METHODS
        #region GET
        [HttpGet]
        [Authorize(Policy = "CanModify")]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(agent.Select<sources>());
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
                if (id < 0) return new BadRequestResult();

                return Ok(await agent.Find<sources>(id));
            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex);
            }
        }
        
        [HttpGet("/Agencies/{agencyId}/[controller]")]
        [Authorize(Policy = "CanModify")]
        public async Task<IActionResult> GetAgencySources(int agencyId)
        {
            try
            {
                if (agencyId < 0) return new BadRequestResult();

                var objectRequested = agent.Select<sources>().Where(x => x.agency_id == agencyId);
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

        [HttpGet("/Files/{fileId}/Source")]
        [Authorize(Policy = "CanModify")]
        public async Task<IActionResult> GetFileSource(int fileId)
        {
            try
            {
                if (fileId < 0) return new BadRequestResult();

                var objectRequested = agent.Select<file>().Include(m => m.sources).FirstOrDefault(x => x.file_id == fileId).sources;
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
        [HttpPost]
        [Authorize(Policy = "CanModify")]
        public async Task<IActionResult> Post([FromBody]sources entity)
        {
            try
            {
                if (!isValid(entity)) return new BadRequestObjectResult("Object is invalid");
                var loggedInMember = LoggedInUser();
                if (loggedInMember == null) return new BadRequestObjectResult("Invalid input parameters");
                entity.last_updated = DateTime.Now;
                entity.last_updated_by = loggedInMember.member_id;
                return Ok(await agent.Add<sources>(entity));
            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpPost]
        [Authorize(Policy = "CanModify")]
        [Route("Batch")]
        public async Task<IActionResult> Batch([FromBody]List<sources> entities)
        {
            try
            {
                if (!isValid(entities)) return new BadRequestObjectResult("Object is invalid");
                var loggedInMember = LoggedInUser();
                if (loggedInMember == null) return new BadRequestObjectResult("Invalid input parameters");
                entities.ForEach(e =>
                {
                    e.last_updated = DateTime.Now;
                    e.last_updated_by = loggedInMember.member_id;
                });
                return Ok(await agent.Add<sources>(entities));
            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex);
            }
        }
        #endregion

        #region PUT
        [HttpPut("{id}")]
        [Authorize(Policy = "CanModify")]
        public async Task<IActionResult> Put(int id, [FromBody]sources entity)
        {
            try
            {
                if (!isValid(entity) || id < 1) return new BadRequestResult();
                var loggedInMember = LoggedInUser();
                if (loggedInMember == null) return new BadRequestObjectResult("Invalid input parameters");
                entity.last_updated = DateTime.Now;
                entity.last_updated_by = loggedInMember.member_id;
                return Ok(await agent.Update<sources>(id, entity));
            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex);
            }
        }
        #endregion

        #region DELETE
        // no delete. sources are shared by other files
        #endregion
        #endregion

        #region HELPER METHODS
        #endregion
    }
}
