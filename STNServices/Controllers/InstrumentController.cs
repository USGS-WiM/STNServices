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
using STNAgent.Resources;

namespace STNServices.Controllers
{
    [Route("[controller]")]
    public class InstrumentsController : STNControllerBase
    {
        public InstrumentsController(ISTNServicesAgent sa) : base(sa)
        {}

        #region METHODS
        
        #region GET
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                //sm(agent.Messages);
                return Ok(agent.Select<instrument>());
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
                return Ok(await agent.Find<instrument>(id));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        // TODO ::: THIS DOESNT WORK //////
        //SensorViews?ViewType={view}&Event={eventIds}&EventType={eventTypeIDs}&EventStatus={eventStatusID}&States={states}&County={counties}&CurrentStatus={statusIDs}&CollectionCondition={collectionConditionIDs}&SensorType={sensorTypeIDs}&DeploymentType={deploymentTypeIDs}").Named("GetSensorViews")
        [HttpGet("/SensorViews")]
        public async Task<IActionResult> GetSensorViews([FromQuery] string ViewType, [FromQuery] string Event = "", [FromQuery] string EventType = "", [FromQuery] string EventStatus = "", [FromQuery] string States = "", [FromQuery] string County = "", [FromQuery] string CurrentStatus = "", [FromQuery] string CollectionCondition = "", [FromQuery] string SensorType = "", [FromQuery] string DeploymentType = "")
        {
            try
            {
                if (string.IsNullOrEmpty(ViewType) &&
                    (ViewType.ToLower() != "baro_view" || ViewType.ToLower() != "met_view" || ViewType.ToLower() != "rdg_view" || ViewType.ToLower() != "stormtide_view" || ViewType.ToLower() != "waveheight_view" ||
                    ViewType.ToLower() != "pressuretemp_view" || ViewType.ToLower() != "therm_view" || ViewType.ToLower() != "webcam_view" || ViewType.ToLower() != "raingage_view"))
                    return new BadRequestObjectResult("Invalid input parameters. ViewType must be either 'baro_view', 'met_view', 'rdg_view', 'stormTide_view' or 'waveheight_view'");

                //sm(agent.Messages);
                return Ok(agent.GetSensorView(ViewType, Event, EventType, EventStatus, States, County, CurrentStatus, CollectionCondition, SensorType, DeploymentType));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }

            
        }

        [HttpGet("{instrumentId}/FullInstrument")]
        public async Task<IActionResult> GetFullInstrument(int instrumentId)
        {
            try
            {
                if (instrumentId < 0) return new BadRequestResult();

                IQueryable<instrument> instrument = agent.Select<instrument>().Include(i => i.sensor_type).Include(i => i.deployment_type).Include(i => i.instr_collection_conditions)
                        .Include(i => i.housing_type).Include(i => i.sensor_brand).Include(i => i.instrument_status).ThenInclude(insT=> insT.status_type).Include(i => i.instrument_status).ThenInclude(insT => insT.vertical_datums)
                        .Where(inst => inst.instrument_id == instrumentId);

                //sm(agent.Messages);
                return Ok(instrument.Select(inst => new FullInstrument
                                {
                                    instrument_id = inst.instrument_id,
                                    sensor_type_id = inst.sensor_type_id,
                                    sensorType = inst.sensor_type_id != null ? inst.sensor_type.sensor : "",
                                    deployment_type_id = inst.deployment_type_id != null && inst.deployment_type_id != 0 ? inst.deployment_type_id.Value : 0,
                                    deploymentType = inst.deployment_type_id != null && inst.deployment_type_id != 0 ? inst.deployment_type.method : "",
                                    serial_number = inst.serial_number,
                                    housing_serial_number = inst.housing_serial_number,
                                    interval = inst.interval != null ? inst.interval.Value : 0,
                                    site_id = inst.site_id != null ? inst.site_id.Value : 0,
                                    event_id = inst.event_id != null ? inst.event_id.Value : 0,
                                    location_description = inst.location_description,
                                    inst_collection_id = inst.inst_collection_id != null && inst.inst_collection_id > 0 ? inst.inst_collection_id.Value : 0,
                                    instCollection = inst.inst_collection_id != null && inst.inst_collection_id > 0 ? inst.instr_collection_conditions.condition : "",
                                    housing_type_id = inst.housing_type_id != null && inst.housing_type_id > 0 ? inst.housing_type_id.Value : 0,
                                    housingType = inst.housing_type_id != null && inst.housing_type_id > 0 ? inst.housing_type.type_name : "",
                                    vented = inst.vented,
                                    sensor_brand_id = inst.sensor_brand_id != null ? inst.sensor_brand_id.Value : 0,
                                    sensorBrand = inst.sensor_brand_id != null ? inst.sensor_brand.brand_name : "",
                                    instrument_status = inst.instrument_status.OrderByDescending(instStat => instStat.time_stamp).Select(i => new Instrument_Status
                                        {
                                            instrument_status_id = i.instrument_status_id,
                                            status_type_id = i.status_type_id,
                                            status = i.status_type_id != null ? i.status_type.status : "",
                                            instrument_id = i.instrument_id,
                                            time_stamp = i.time_stamp.Value,
                                            time_zone = i.time_zone,
                                            notes = i.notes,
                                            member_id = i.member_id != null ? i.member_id.Value : 0,
                                            sensor_elevation = i.sensor_elevation,
                                            ws_elevation = i.ws_elevation,
                                            gs_elevation = i.gs_elevation,
                                            vdatum_id = i.vdatum_id,
                                            vdatum = i.vdatum_id.HasValue && i.vdatum_id > 0 ? i.vertical_datums.datum_name : ""
                                        }).ToList<instrument_status>()
                                }).FirstOrDefault()); 
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }
        
        [HttpGet("/Sites/{siteId}/SiteFullInstrumentList")]
        public async Task<IActionResult> GetSiteFullInstrumentList(int siteId)
        {
            // get list of instrument and all instrument_stats together for a site
            try
            {
                if (siteId < 0) return new BadRequestResult();

                IQueryable<instrument> instrument = agent.Select<instrument>().Include(i => i.sensor_type).Include(i => i.deployment_type).Include(i => i.instr_collection_conditions)
                        .Include(i => i.housing_type).Include(i => i.sensor_brand).Include(i => i.instrument_status).ThenInclude(insT => insT.status_type).Include(i => i.instrument_status).ThenInclude(insT => insT.vertical_datums)
                        .Where(inst => inst.site_id == siteId);

                //sm(agent.Messages);
                return Ok(instrument.Select(inst => new FullInstrument
                {
                    instrument_id = inst.instrument_id,
                    sensor_type_id = inst.sensor_type_id,
                    sensorType = inst.sensor_type_id != null ? inst.sensor_type.sensor : "",
                    deployment_type_id = inst.deployment_type_id != null && inst.deployment_type_id != 0 ? inst.deployment_type_id.Value : 0,
                    deploymentType = inst.deployment_type_id != null && inst.deployment_type_id != 0 ? inst.deployment_type.method : "",
                    serial_number = inst.serial_number,
                    housing_serial_number = inst.housing_serial_number,
                    interval = inst.interval != null ? inst.interval.Value : 0,
                    site_id = inst.site_id != null ? inst.site_id.Value : 0,
                    event_id = inst.event_id != null ? inst.event_id.Value : 0,
                    location_description = inst.location_description,
                    inst_collection_id = inst.inst_collection_id != null && inst.inst_collection_id > 0 ? inst.inst_collection_id.Value : 0,
                    instCollection = inst.inst_collection_id != null && inst.inst_collection_id > 0 ? inst.instr_collection_conditions.condition : "",
                    housing_type_id = inst.housing_type_id != null && inst.housing_type_id > 0 ? inst.housing_type_id.Value : 0,
                    housingType = inst.housing_type_id != null && inst.housing_type_id > 0 ? inst.housing_type.type_name : "",
                    vented = inst.vented,
                    sensor_brand_id = inst.sensor_brand_id != null ? inst.sensor_brand_id.Value : 0,
                    sensorBrand = inst.sensor_brand_id != null ? inst.sensor_brand.brand_name : "",
                    instrument_status = inst.instrument_status.OrderByDescending(instStat => instStat.time_stamp).Select(i => new Instrument_Status
                    {
                        instrument_status_id = i.instrument_status_id,
                        status_type_id = i.status_type_id,
                        status = i.status_type_id != null ? i.status_type.status : "",
                        instrument_id = i.instrument_id,
                        time_stamp = i.time_stamp.Value,
                        time_zone = i.time_zone,
                        notes = i.notes,
                        member_id = i.member_id != null ? i.member_id.Value : 0,
                        sensor_elevation = i.sensor_elevation,
                        ws_elevation = i.ws_elevation,
                        gs_elevation = i.gs_elevation,
                        vdatum_id = i.vdatum_id,
                        vdatum = i.vdatum_id.HasValue && i.vdatum_id > 0 ? i.vertical_datums.datum_name : ""
                    }).ToList<instrument_status>()
                }).FirstOrDefault());
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpGet("/DataFiles/{dataFileId}/Instrument")]
        public async Task<IActionResult> GetDataFileInstrument(int dataFileId)
        {
            try
            {
                if (dataFileId < 0) return new BadRequestResult();

                var objectRequested = agent.Select<data_file>().Include(m => m.instrument).FirstOrDefault(x => x.data_file_id == dataFileId).instrument;
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

        [HttpGet("/InstrumentStatus/{instrumentStatusId}/Instrument")]
        public async Task<IActionResult> GetInstrumentStatusInstrument(int instrumentStatusId)
        {
            try
            {
                if (instrumentStatusId < 0) return new BadRequestResult();

                var objectRequested = agent.Select<instrument_status>().Include(m => m.instrument).FirstOrDefault(x => x.instrument_status_id == instrumentStatusId).instrument;
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

        [HttpGet("/Files/{fileId}/Instrument")]
        public async Task<IActionResult> GetFileInstrument(int fileId)
        {
            try
            {
                if (fileId < 0) return new BadRequestResult();

                var objectRequested = agent.Select<file>().Include(m => m.instrument).FirstOrDefault(x => x.file_id == fileId).instrument;
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

        [HttpGet("/Sites/{siteId}/[controller]")]
        public async Task<IActionResult> GetSiteInstruments(int siteId)
        {
            try
            {
                if (siteId < 0) return new BadRequestResult();

                var objectsRequested = agent.Select<sites>()
                    .Include(e => e.instruments).FirstOrDefault(s => s.site_id == siteId).instruments;

                if (objectsRequested == null)
                    return new BadRequestObjectResult(new Error(errorEnum.e_notFound));

                //sm(agent.Messages);
                return Ok(objectsRequested);
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpGet("/SensorBrands/{sensorBrandId}/[controller]")]
        public async Task<IActionResult> GetSensorBrandInstruments(int sensorBrandId)
        {
            try
            {
                if (sensorBrandId < 0) return new BadRequestResult();

                var objectsRequested = agent.Select<sensor_brand>()
                    .Include(e => e.instruments).FirstOrDefault(s => s.sensor_brand_id == sensorBrandId).instruments;

                if (objectsRequested == null)
                    return new BadRequestObjectResult(new Error(errorEnum.e_notFound));

                //sm(agent.Messages);
                return Ok(objectsRequested);
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpGet("/SensorTypes/{sensorTypeId}/[controller]")]
        public async Task<IActionResult> GetSensorTypeInstruments(int sensorTypeId)
        {
            try
            {
                if (sensorTypeId < 0) return new BadRequestResult();

                var objectsRequested = agent.Select<sensor_type>()
                    .Include(e => e.instruments).FirstOrDefault(s => s.sensor_type_id == sensorTypeId).instruments;

                if (objectsRequested == null)
                    return new BadRequestObjectResult(new Error(errorEnum.e_notFound));

                //sm(agent.Messages);
                return Ok(objectsRequested);
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpGet("/DeploymentTypes/{deploymentTypeId}/[controller]")]
        public async Task<IActionResult> GetDeploymentTypeInstruments(int deploymentTypeId)
        {
            try
            {
                if (deploymentTypeId < 0) return new BadRequestResult();

                var objectsRequested = agent.Select<deployment_type>()
                    .Include(e => e.instruments).FirstOrDefault(s => s.deployment_type_id == deploymentTypeId).instruments;

                if (objectsRequested == null)
                    return new BadRequestObjectResult(new Error(errorEnum.e_notFound));

                //sm(agent.Messages);
                return Ok(objectsRequested);
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpGet("/Events/{eventId}/[controller]")]
        public async Task<IActionResult> GetEventInstruments(int eventId)
        {
            try
            {
                if (eventId < 0) return new BadRequestResult();

                var objectsRequested = agent.Select<events>()
                    .Include(e => e.instruments).FirstOrDefault(s => s.event_id == eventId).instruments;

                if (objectsRequested == null)
                    return new BadRequestObjectResult(new Error(errorEnum.e_notFound));

                //sm(agent.Messages);
                return Ok(objectsRequested);
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpGet("/Sites/{siteId}/[controller]/byEvent")]//siteResource + "/{siteId}/" + instrumentsResource + "?Event={eventId} ==> to siteResource + "/{siteId}/" + instrumentsResource + "/byEvent?Event={eventId}
        public async Task<IActionResult> GetSiteEventInstruments(int siteId, [FromQuery] string Event)
        {
            try
            {
                if (siteId < 0) return new BadRequestResult();

                var objectsRequested = agent.Select<instrument>().Where(i => i.site_id == siteId && i.event_id == Convert.ToInt32(Event));

                if (objectsRequested == null)
                    return new BadRequestObjectResult(new Error(errorEnum.e_notFound));

                //sm(agent.Messages);
                return Ok(objectsRequested);
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        //Instruments/FilteredInstruments?Event={eventIds}&EventType={eventTypeIDs}&EventStatus={eventStatusID}&States={states}&County={counties}&CurrentStatus={statusIDs}&CollectionCondition={collectionConditionIDs}&SensorType={sensorTypeIDs}&DeploymentType={deploymentTypeIDs}
        [HttpGet("FilteredInstruments")]
        public async Task<IActionResult> GetFilteredInstruments([FromQuery] string Event = "", [FromQuery] string EventType = "", [FromQuery] string EventStatus = "", [FromQuery] string States = "", [FromQuery] string County = "", [FromQuery] string CurrentStatus = "", [FromQuery] string CollectionCondition = "", [FromQuery] string SensorType = "", [FromQuery] string DeploymentType = "")
        {
            try
            {
                //sm(agent.Messages);
                return Ok(agent.GetFiltedInstruments(Event, EventType, EventStatus, States, County, CurrentStatus, CollectionCondition, SensorType, DeploymentType));
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
        public async Task<IActionResult> Post([FromBody] instrument entity)
        {
            try
            {
                if (!isValid(entity)) return new BadRequestResult();

                var loggedInMember = LoggedInUser();
                if (loggedInMember == null) return new BadRequestObjectResult("Invalid input parameters");

                entity.last_updated = DateTime.Now;
                entity.last_updated_by = loggedInMember.member_id;

                //sm(agent.Messages);
                return Ok(await agent.Add<instrument>(entity));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }
                
        [HttpPost][Authorize(Policy = "CanModify")]
        [Route("Batch")]
        public async Task<IActionResult> Batch([FromBody]List<instrument> entities)
        {
            try
            {
                if (!isValid(entities)) return new BadRequestObjectResult("Object is invalid");

                var loggedInMember = LoggedInUser();
                if (loggedInMember == null) return new BadRequestObjectResult("Invalid input parameters");

                entities.ForEach(entity =>
                {
                    entity.last_updated = DateTime.Now;
                    entity.last_updated_by = loggedInMember.member_id;
                });

                //sm(agent.Messages);
                return Ok(await agent.Add<instrument>(entities));
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
        public async Task<IActionResult> Put(int id, [FromBody] instrument entity)
        {
            try
            {
                if (id < 0 || !isValid(entity)) return new BadRequestResult();
                var loggedInMember = LoggedInUser();
                if (loggedInMember == null) return new BadRequestObjectResult("Invalid input parameters");

                entity.last_updated = DateTime.Now;
                entity.last_updated_by = loggedInMember.member_id;

                return Ok(await agent.Update<instrument>(id, entity));
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
                // TODO:::: NOT WORKING await or async ... 
                if (id < 1) return new BadRequestResult();
                
                List<instrument_status> insStatList = agent.Select<instrument_status>().Where(ins => ins.instrument_id == id).ToList();
                List<Int32> SIDs = new List<Int32>();
                //delete the statuses and get the id
                foreach (instrument_status i in insStatList)
                {
                    SIDs.Add(i.instrument_status_id);
                    await agent.Delete(i);
                }

                //get and delete all op_measurements
                foreach (Int32 sid in SIDs)
                {
                    var opMeasures = agent.Select<op_measurements>().Where(o => o.instrument_status_id == sid).ToList();
                    opMeasures.ForEach(op => agent.Delete(op));
                }

                //remove files
                //  entity.files.ToList().ForEach(f => agent.RemoveFileItem(f)); ///////TODO:::///////
                List<file> files = agent.Select<file>().Where(f => f.instrument_id == id).ToList();
                files.ForEach(f => agent.Delete(f)); //A second operation started on this context before a previous operation completed. Any instance members are not guaranteed to be thread safe.

                //remove datafile          
                List<data_file> datafiles = agent.Select<data_file>().Where(f => f.instrument_id == id).ToList();
                datafiles.ForEach(df => agent.Delete(df));
                
   //             var entity = agent.Select<instrument>().Include(i => i.files).Include(i=> i.data_files).FirstOrDefault(i => i.instrument_id == id);
                var entity = await agent.Find<instrument>(id);
                if (entity == null) return new NotFoundResult();

                await agent.Delete(entity);
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
