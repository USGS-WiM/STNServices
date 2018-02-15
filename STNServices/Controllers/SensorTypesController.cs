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
using STNAgent.Resources;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace STNServices.Controllers
{
    [Route("[controller]")]
    public class SensorTypesController : STNControllerBase
    {
        public SensorTypesController(ISTNServicesAgent sa) : base(sa)
        {}

        #region METHODS
        
        #region GET
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var sensorTypes = agent.Select<sensor_type>().Include("sensor_deployment.deployment_type");

                //sm(agent.Messages);

                return Ok(sensorTypes.Select(st => new SensorType()
                {
                    sensor_type_id = st.sensor_type_id,
                    sensor = st.sensor,
                    deploymenttypes = st.sensor_deployment.Select(x => x.deployment_type).ToList()
                }));
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
                return Ok(await agent.Find<sensor_type>(id));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }
        //(deploymenttypeResource+"/{deploymentTypeId}/SensorType")
        [HttpGet("/DeploymentTypes/{deploymentTypeId}/SensorType")]
        public async Task<IActionResult> GetDeploymentSensorType(int deploymentTypeId)
        {
            try
            {
                if (deploymentTypeId < 0) return new BadRequestResult();

                var objectRequested = agent.Select<sensor_deployment>().Include(m => m.sensor_type).FirstOrDefault(x => x.deployment_type_id == deploymentTypeId).sensor_type;
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

        //(instrumentsResource+"/{instrumentId}/SensorType")
        [HttpGet("/Instruments/{instrumentId}/SensorType")]
        public async Task<IActionResult> GetInstrumentSensorType(int instrumentId)
        {
            try
            {
                if (instrumentId < 0) return new BadRequestResult(); 

                var objectRequested = agent.Select<instrument>().Include(m => m.sensor_type).FirstOrDefault(x => x.instrument_id == instrumentId).sensor_type;
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
        public async Task<IActionResult> Post([FromBody]sensor_type entity)
        {
            try
            {
                if (!isValid(entity)) return new BadRequestResult();
                //sm(agent.Messages);
                return Ok(await agent.Add<sensor_type>(entity));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }
                
        [HttpPost][Authorize(Policy = "AdminOnly")]
        [Route("Batch")]
        public async Task<IActionResult> Batch([FromBody]List<sensor_type> entities)
        {
            try
            {
                if (!isValid(entities)) return new BadRequestObjectResult("Object is invalid");
                //sm(agent.Messages);
                return Ok(await agent.Add<sensor_type>(entities));
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
        public async Task<IActionResult> Put(int id, [FromBody]sensor_type entity)
        {
            try
            {
                if (id < 0 || !isValid(entity)) return new BadRequestResult();
                return Ok(await agent.Update<sensor_type>(id, entity));
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
                var entity = await agent.Find<sensor_type>(id);
                if (entity == null) return new NotFoundResult();

                await agent.Delete<sensor_type>(entity);
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
