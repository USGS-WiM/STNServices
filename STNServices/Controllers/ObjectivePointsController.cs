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
    public class ObjectivePointsController : STNControllerBase
    {
        public ObjectivePointsController(ISTNServicesAgent sa) : base(sa)
        {}

        #region METHODS
        
        #region GET
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                //sm(agent.Messages);
                return Ok(agent.Select<objective_point>());
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
                return Ok(await agent.Find<objective_point>(id));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }
                
        [HttpGet("/Sites/{siteId}/[controller]")] 
        public async Task<IActionResult> GetSiteObjectivePoints(int siteId)
        {
            try
            {
                if (siteId < 0) return new BadRequestResult();

                var objectsRequested = agent.Select<objective_point>().Where(o => o.site_id == siteId);
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

        [HttpGet("/VerticalDatums/{vdatumId}/[controller]")]
        public async Task<IActionResult> GetVDatumOPs(int vdatumId)
        {
            try
            {
                if (vdatumId < 0) return new BadRequestResult();

                var objectsRequested = agent.Select<objective_point>().Where(o => o.vdatum_id == vdatumId);
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
        public async Task<IActionResult> Post([FromBody]objective_point entity)
        {
            try
            {
                if (!isValid(entity)) return new BadRequestResult();
                var loggedInMember = LoggedInUser();
                if (loggedInMember == null) return new BadRequestObjectResult("Invalid input parameters");
                entity.last_updated = DateTime.Now;
                entity.last_updated_by = loggedInMember.member_id;
                //sm(agent.Messages);
                return Ok(await agent.Add<objective_point>(entity));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpPost][Authorize(Policy = "AdminOnly")]
        [Route("Batch")]
        public async Task<IActionResult> Batch([FromBody]List<objective_point> entities)
        {
            try
            {
                if (!isValid(entities)) return new BadRequestObjectResult("Object is invalid");
                var loggedInMember = LoggedInUser();
                if (loggedInMember == null) return new BadRequestObjectResult("Invalid input parameters");

                entities.ForEach(o =>
                {
                    o.last_updated = DateTime.Now;
                    o.last_updated_by = loggedInMember.member_id;

                });
                //sm(agent.Messages);
                return Ok(await agent.Add<objective_point>(entities));
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
        public async Task<IActionResult> Put(int id, [FromBody]objective_point entity)
        {
            try
            {
                if (id < 0 || !isValid(entity)) return new BadRequestResult();

                var loggedInMember = LoggedInUser();
                if (loggedInMember == null) return new BadRequestObjectResult("Invalid input parameters");

                entity.last_updated_by = loggedInMember.member_id;
                entity.last_updated = DateTime.Now;

                return Ok(await agent.Update<objective_point>(id, entity));
            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex);
            }           
        }
        #endregion
        
        #region DELETE
        [HttpDelete("{id}")][Authorize(Policy = "CanModify")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id < 1) return new BadRequestResult();

                //delete op_controlIdentifiers
                agent.Select<op_control_identifier>().Where(opc => opc.objective_point_id == id).ToList().ForEach(o => agent.Delete(o));

                //delete files
                var opFiles = agent.Select<file>().Where(f => f.objective_point_id == id).ToList();

                for (int i = 0; i < opFiles.Count; i++)
                {
                //    await agent.RemoveFileItem(opFiles[i]); TODO
                    await agent.Delete(opFiles[i]);
                }
                var entity = await agent.Find<objective_point>(id);
                if (entity == null) return new NotFoundResult();

                await agent.Delete<objective_point>(entity);
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
