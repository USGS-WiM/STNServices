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
    public class NetworkTypesController : STNControllerBase
    {
        public NetworkTypesController(ISTNServicesAgent sa) : base(sa)
        { }

        #region METHODS

        #region GET
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                //sm(agent.Messages);
                return Ok(agent.Select<network_type>());
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
                return Ok(await agent.Find<network_type>(id));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpGet("/Sites/{siteId}/[controller]")]
        public async Task<IActionResult> GetSiteNetworkTypes(int siteId)
        {
            try
            {
                if (siteId < 0) return new BadRequestResult();

                var objectRequested = agent.Select<network_type_site>().Include(m => m.network_type).Where(m => m.site_id == siteId).Select(nn => nn.network_type);
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
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Post([FromBody]network_type entity)
        {
            try
            {
                if (!isValid(entity)) return new BadRequestResult();
                //sm(agent.Messages);
                return Ok(await agent.Add<network_type>(entity));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpPost("/Sites/{siteId}/addNetworkType")] //siteResource+"/{siteId}/addNetworkType?NetworkTypeId={networkTypeId}"
        [Authorize(Policy = "CanModify")]
        public async Task<IActionResult> addSiteNetworkType(int siteId, [FromQuery]int NetworkTypeId)
        {
            try
            {
                if (siteId <= 0 || NetworkTypeId <= 0) return new BadRequestResult();
                //sm(agent.Messages);
                if (agent.Select<sites>().First(s => s.site_id == siteId) == null)
                    return new NotFoundResult();
                if (agent.Select<network_type>().First(n => n.network_type_id == NetworkTypeId) == null)
                    return new NotFoundResult();

                if (agent.Select<network_type_site>().FirstOrDefault(nt => nt.network_type_id == NetworkTypeId && nt.site_id == siteId) == null)
                {
                    network_type_site anEntity = new network_type_site();
                    anEntity.site_id = siteId;
                    anEntity.network_type_id = NetworkTypeId;
                    anEntity = await agent.Add<network_type_site>(anEntity);
                    //sm(sa.Messages);
                }
                //return list of network types
                var networkNameList = agent.Select<network_type>().Where(nn => nn.network_type_site.Any(nns => nns.site_id == siteId));

                return Ok(networkNameList);
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [Route("Batch")]
        public async Task<IActionResult> Batch([FromBody]List<network_type> entities)
        {
            try
            {
                if (!isValid(entities)) return new BadRequestObjectResult("Object is invalid");
                //sm(agent.Messages);
                return Ok(await agent.Add<network_type>(entities));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }
        #endregion

        #region PUT
        [HttpPut("{id}")]
        [Authorize(Policy = "CanModify")]
        public async Task<IActionResult> Put(int id, [FromBody]network_type entity)
        {
            try
            {
                if (id < 0 || !isValid(entity)) return new BadRequestResult();
                return Ok(await agent.Update<network_type>(id, entity));
            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex);
            }
        }
        #endregion

        #region DELETE
        [HttpDelete("{id}")]
        [Authorize(Policy = "Restricted")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id < 1) return new BadRequestResult();
                var entity = await agent.Find<network_type>(id);
                if (entity == null) return new NotFoundResult();

                await agent.Delete<network_type>(entity);
                //sm(agent.Messages);
                return Ok();
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpDelete("/Sites/{siteId}/removeNetworkType")] //siteResource + "/{siteId}/removeNetworkType?NetworkTypeId={networkTypeId}"
        [Authorize(Policy = "CanModify")]
        public async Task<IActionResult> removeSiteNetworkType(int siteId, [FromQuery] int NetworkTypeId)
        {
            try
            {
                if (siteId <= 0 || NetworkTypeId <= 0) return new BadRequestResult();
                var entity = agent.Select<network_type_site>().SingleOrDefault(nns => nns.site_id == siteId && nns.network_type_id == NetworkTypeId);
                if (entity == null) return new NotFoundResult();

                await agent.Delete<network_type_site>(entity);
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
