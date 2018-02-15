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
    public class DeploymentTypesController : STNControllerBase
    {
        public DeploymentTypesController(ISTNServicesAgent sa) : base(sa)
        {}

        #region METHODS
        
        #region GET
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                //sm(agent.Messages);
                return Ok(agent.Select<deployment_type>());
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
                return Ok(await agent.Find<deployment_type>(id));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpGet("/Instruments/{instrumentId}/DeploymentType")]
        public async Task<IActionResult> GetInstrumentDeploymentType(int instrumentId)
        {
            try
            {
                if (instrumentId < 0) return new BadRequestResult(); 

                var objectRequested = agent.Select<instrument>().Include(m => m.deployment_type).FirstOrDefault(x => x.instrument_id == instrumentId).deployment_type;
                if (objectRequested == null) return new BadRequestObjectResult(new Error(errorEnum.e_notFound)); // This returns HTTP 400
                //sm(agent.Messages);
                return Ok(objectRequested);
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpGet("/SensorTypes/{sensorTypeId}/[controller]")]
        public async Task<IActionResult> GetSensorDeploymentTypes(int sensorTypeId)
        {
            try
            {
                if (sensorTypeId < 0) return new BadRequestResult();

                var objectsRequested = agent.Select<sensor_deployment>().Include(m => m.deployment_type).Where(x => x.sensor_type_id == sensorTypeId).Select(s => s.deployment_type);
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
        [HttpPost][Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Post([FromBody]deployment_type entity)
        {
            try
            {
                if (!isValid(entity)) return new BadRequestResult();
                //sm(agent.Messages);
                return Ok(await agent.Add<deployment_type>(entity));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }
                
        [HttpPost("SensorTypes/{sensorTypeId}/addDeploymentType")] //(sensorTypeResource+"/{sensorTypeId}/addDeploymentType?DeploymentTypeId={deploymentTypeId}"
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> AddSensorDeployment(int sensorTypeId, [FromQuery] int deploymentTypeId)
        {            
            try
            {
                if (sensorTypeId < 0 || deploymentTypeId < 0) return new BadRequestResult(); 
                if (await agent.Find<sensor_type>(sensorTypeId) == null)
                    return new BadRequestObjectResult(new Error(errorEnum.e_notFound));
                if (await agent.Find<deployment_type>(deploymentTypeId) == null)
                    return new BadRequestObjectResult(new Error(errorEnum.e_notFound));

                // create new sensor_deployment if it doesnt exist already
                if (agent.Select<sensor_deployment>().FirstOrDefault(sd => sd.sensor_type_id == sensorTypeId && sd.deployment_type_id == deploymentTypeId) == null)
                {
                    var anEntity = new sensor_deployment();
                    anEntity.sensor_type_id = sensorTypeId;
                    anEntity.deployment_type_id = deploymentTypeId;
                    anEntity = await agent.Add<sensor_deployment>(anEntity);
                    //sm(agent.Messages);
                }
                var deploymentTYpeList = agent.Select<deployment_type>().Include(dt => dt.sensor_deployment).Where(dt => dt.sensor_deployment.Any(sd => sd.sensor_type_id == sensorTypeId));
                //sm(agent.Messages);
                return Ok(deploymentTYpeList);
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpPost][Authorize(Policy = "AdminOnly")]
        [Route("Batch")]
        public async Task<IActionResult> Batch([FromBody]List<deployment_type> entities)
        {
            try
            {
                if (!isValid(entities)) return new BadRequestObjectResult("Object is invalid");
                //sm(agent.Messages);
                return Ok(await agent.Add<deployment_type>(entities));
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
        public async Task<IActionResult> Put(int id, [FromBody]deployment_type entity)
        {
            try
            {
                if (id < 0 || !isValid(entity)) return new BadRequestResult();
                return Ok(await agent.Update<deployment_type>(id, entity));
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
                var entity = await agent.Find<deployment_type>(id);
                if (entity == null) return new NotFoundResult();

                await agent.Delete<deployment_type>(entity);
                //sm(agent.Messages);
                return Ok();
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpDelete("/SensorTypes/{sensorTypeId}/removeDeploymentType")] //(sensorTypeResource + "/{sensorTypeId}/removeDeploymentType?DeploymentTypeId={deploymentTypeId}"
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> RemoveSensorDeployment(int sensorTypeId, [FromQuery] int deploymentTypeId)
        {
            try
            {
                if (sensorTypeId < 1 || deploymentTypeId < 1) return new BadRequestResult();

                var sensorType = await agent.Find<sensor_type>(sensorTypeId);
                if (sensorType == null) return new NotFoundResult();
                var entity = agent.Select<sensor_deployment>().SingleOrDefault(sd => sd.sensor_type_id == sensorTypeId && sd.deployment_type_id == deploymentTypeId);
                await agent.Delete<sensor_deployment>(entity);
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
