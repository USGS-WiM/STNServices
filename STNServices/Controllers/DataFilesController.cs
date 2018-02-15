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
    public class DataFilesController : STNControllerBase
    {
        public DataFilesController(ISTNServicesAgent sa) : base(sa)
        {}

        #region METHODS

        #region GET
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                if (LoggedInUser() == null)
                {
                    //general public
                    //sm(agent.Messages);
                    return Ok(agent.Select<data_file>().Where(d=>d.approval_id > 0));
                } else
                {
                    //sm(agent.Messages);
                    return Ok(agent.Select<data_file>());
                }                
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
                if (id < 0) return new BadRequestResult(); // This returns HTTP 404

                if (LoggedInUser() == null)
                {
                    //sm(agent.Messages);
                    return Ok(agent.Select<data_file>().FirstOrDefault(d => d.data_file_id == id && d.approval_id > 0));
                } else
                {
                    //sm(agent.Messages);
                    return Ok(await agent.Find<data_file>(id));
                }
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }
        
        [HttpGet("/Instruments/{instrumentId}/[controller]")]
        public async Task<IActionResult> GetInstrumentDataFiles(int instrumentId)
        {
            try
            {                
                if (instrumentId < 0) return new BadRequestResult();
                var entities = agent.Select<data_file>().Where(d=>d.instrument_id == instrumentId);

                if (LoggedInUser() == null)
                {
                    //general public..only approved ones
                    entities = entities.Where(df => df.approval_id > 0);
                }
                //sm(agent.Messages);
                return Ok(entities);
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpGet("/PeakSummaries/{peakSummaryId}/[controller]")]
        public async Task<IActionResult> GetPeakSummaryDatafiles(int peakSummaryId)
        {
            try
            {
                var entities = agent.Select<data_file>().Where(d => d.peak_summary_id == peakSummaryId);             
                if (peakSummaryId <= 0) return new BadRequestResult();

                if (LoggedInUser() == null)
                {
                    //they are the general public..only approved ones
                    entities = entities.Where(df => df.approval_id > 0);                    
                }
                //sm(agent.Messages);
                return Ok(entities);                
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }
                
        [HttpGet("/Approvals/{approvalId}/[controller]")]
        public async Task<IActionResult> GetApprovedDataFiles(int approvalId)
        {            
            try
            {
                if (approvalId < 0) return new BadRequestResult(); 

                var objectsRequested = agent.Select<data_file>().FirstOrDefault(x => x.approval_id == approvalId);
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
        
        [HttpGet("FilteredDataFiles")] //(datafileResource + "?IsApproved={approved}&Event={eventId}&State={state}&Counties={counties}"
        public async Task<IActionResult> GetFilteredDataFiles([FromQuery] string IsApproved, [FromQuery] string Event = "", [FromQuery] string State = "", [FromQuery] string Counties = "")
        {
            try
            {
                if (string.IsNullOrEmpty(IsApproved)) return new BadRequestResult();
                                
                //sm(agent.Messages);
                return Ok(agent.GetFilterDataFiles(IsApproved, Event, State, Counties));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpGet("/Files/{fileId}/DataFile")]
        public async Task<IActionResult> GetFileDataFile(int fileId)
        {
            try
            {
                if (fileId < 0) return new BadRequestResult(); // This returns HTTP 404
                var objectRequested = agent.Select<file>().Include(f => f.data_file).FirstOrDefault(f => f.file_id == fileId).data_file;
                //general public..only approved ones
                if (LoggedInUser() == null)
                {
                    objectRequested = objectRequested.approval_id > 0 ? objectRequested : null;
                }
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
                
        [HttpGet("/DataFileView")] //("/DataFileView?EventId={eventId}")
        public async Task<IActionResult> GetEventDataFileView([FromQuery]int EventId)
        {            
            try
            {
                if (EventId < 0) return new BadRequestResult();
                //sm(agent.Messages);
                //sm(agent.Messages);
                return Ok(agent.getTable<dataFile_view>(new Object[1] { null }).Where(e => e.event_id == EventId));                
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        //(datafileResource + "/RunStormScript?SeaDataFileID={seaDataFileId}&AirDataFileID={airDataFileId}&Hertz={hertz}&Username={username}")
       /* [HttpGet("RunStormScript")]
        public async Task<IActionResult> RunStormScript([FromQuery] Int32 seaDataFileId, [FromQuery] Int32 airDataFileId, [FromQuery] string hertz, [FromQuery] string username)
        {            
            try
            {
                //Return BadRequest if there is no ID
                bool isHertz = false;
                Boolean.TryParse(hertz, out isHertz);
                if (airDataFileId <= 0 || seaDataFileId <= 0 || string.IsNullOrEmpty(username))
                    return new BadRequestResult();

                var stnsa = new STNScriptAgent(airDataFileId, seaDataFileId, username);
                if (stnsa.initialized)
                    stnsa.RunStormScript(isHertz);
                else
                    return new BadRequestObjectResult("Error initializing python script.");
                //sm(agent.Messages);
                return Ok();
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }*/

        //(datafileResource + "/RunAirScript?AirDataFileID={airDataFileId}&Username={username}")
      /*  [HttpGet("RunAirScript")]
        public async Task<IActionResult> RunAirDataFileScript([FromQuery] Int32 airDataFileId, [FromQuery] string username)
        {
            try
            {
                if (airDataFileId <= 0 || string.IsNullOrEmpty(username))
                    return new BadRequestResult();

                var stnsa = new STNScriptAgent(airDataFileId, username);
                if (stnsa.pressureInitialized)
                    stnsa.RunAirScript();
                else
                    return new BadRequestObjectResult("Error initializing python script.");

                //sm(agent.Messages);
                return Ok();
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }*/

        #endregion

        #region POST
        [HttpPost][Authorize(Policy = "CanModify")]
        public async Task<IActionResult> Post([FromBody]data_file entity)
        {            
            try
            {
                if (!isValid(entity)) return new BadRequestResult();

                var loggedInMember = LoggedInUser();
                if (loggedInMember == null) return new BadRequestObjectResult("Invalid input parameters");
                entity.last_updated = DateTime.Now;
                entity.last_updated_by = loggedInMember.member_id;
                //sm(agent.Messages);
                return Ok(await agent.Add<data_file>(entity));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpPost][Authorize(Policy = "CanModify")]
        [Route("Batch")]
        public async Task<IActionResult> Batch([FromBody]List<data_file> entities)
        {
            try
            {
                if (!isValid(entities)) return new BadRequestObjectResult("Object is invalid");
                var loggedInMember = LoggedInUser();
                if (loggedInMember == null) return new BadRequestObjectResult("Invalid input parameters");

                entities.ForEach(df =>
                {

                    df.last_updated_by = loggedInMember.member_id;
                   df.last_updated = DateTime.Now;
                });
                //sm(agent.Messages);
                return Ok(await agent.Add<data_file>(entities));
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
        public async Task<IActionResult> Put(int id, [FromBody]data_file entity)
        {            
            try
            {
                if (id < 0 || !isValid(entity)) return new BadRequestResult();
                var loggedInMember = LoggedInUser();
                if (loggedInMember == null) return new BadRequestObjectResult("Invalid input parameters");

                entity.last_updated = DateTime.Now;
                entity.last_updated_by = loggedInMember.member_id;
                //sm(agent.Messages);
                return Ok(await agent.Update<data_file>(id, entity));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
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
                var entity = await agent.Find<data_file>(id);
                if (entity == null) return new NotFoundResult();

                await agent.Delete<data_file>(entity);
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
