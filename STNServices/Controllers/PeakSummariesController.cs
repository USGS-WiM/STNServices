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
    public class PeakSummariesController : STNControllerBase
    {
        public PeakSummariesController(ISTNServicesAgent sa) : base(sa)
        {}

        #region METHODS
        
        #region GET
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                //sm(agent.Messages);
                return Ok(agent.Select<peak_summary>());
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
                return Ok(await agent.Find<peak_summary>(id));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpGet("/HWMs/{hwmId}/PeakSummary")]
        public async Task<IActionResult> GetHWMPeakSummary(int hwmId)
        {
            try
            {
                if (hwmId < 0) return new BadRequestResult();

                var objectRequested = agent.Select<hwm>().Include(h => h.peak_summary).FirstOrDefault(h => h.hwm_id == hwmId).peak_summary;

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

        [HttpGet("/DataFiles/{dataFileId}/PeakSummary")]
        public async Task<IActionResult> GetDataFilePeakSummary(int dataFileId)
        {
            try
            {
                if (dataFileId < 0) return new BadRequestResult();

                var objectRequested = agent.Select<data_file>().Include(d => d.peak_summary).FirstOrDefault(h => h.data_file_id == dataFileId).peak_summary;

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
        
        [HttpGet("/Sites/{siteId}/PeakSummaryView")]
        public async Task<IActionResult> GetPeakSummaryViewBySite(int siteId)
        {
            try
            {
                if (siteId < 0) return new BadRequestResult();

                //TODO sql statements are not working
                var objectRequested = agent.getTable<peak_view>(new Object[1] { null }).Where(p => p.site_id == siteId).ToList();

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

        [HttpGet("/Events/{eventId}/[controller]")]
        public async Task<IActionResult> GetEventPeakSummaries(int eventId)
        {
            try
            {
                if (eventId < 0) return new BadRequestResult();

                var objectsRequested = agent.Select<peak_summary>().Include(ps => ps.hwms).Include(ps => ps.data_file).ThenInclude(df => df.instrument)
                                .Where(ps => ps.hwms.Any(hwm => hwm.event_id == eventId) || ps.data_file.Any(d => d.instrument.event_id == eventId));
               
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

        [HttpGet("/Sites/{siteId}/[controller]")]
        public async Task<IActionResult> GetSitePeakSummaries(int siteId)
        {
            try
            {
                if (siteId < 0) return new BadRequestResult();

                var objectsRequested = agent.Select<peak_summary>().Include(ps => ps.hwms).Include(ps => ps.data_file).ThenInclude(df => df.instrument)
                                .Where(ps => ps.hwms.Any(hwm => hwm.site_id == siteId) || ps.data_file.Any(d => d.instrument.site_id == siteId));

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

        //@"/PeakSummaries/FilteredPeaks?Event={eventIds}&EventType={eventTypeIDs}&EventStatus={eventStatusID}&States={states}&County={counties}&StartDate={startDate}&EndDate={endDate}"
        [HttpGet("FilteredPeaks")]
        public async Task<IActionResult> GetFilteredPeaks([FromQuery] string Event = "", [FromQuery] string EventType = "", [FromQuery] string EventStatus = "", [FromQuery] string States = "", [FromQuery] string County = "", [FromQuery] string StartDate = "", [FromQuery] string EndDate = "")
        {
            try
            {
                //sm(agent.Messages);
                return Ok(agent.GetFiltedPeaks(Event, EventType, EventStatus, States, County, StartDate, EndDate));
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
        public async Task<IActionResult> Post([FromBody]peak_summary entity)
        {
            try
            {
                if (!isValid(entity)) return new BadRequestResult();
                var loggedInMember = LoggedInUser();
                if (loggedInMember == null) return new BadRequestObjectResult("Invalid input parameters");

                entity.last_updated = DateTime.Now;
                entity.last_updated_by = loggedInMember.member_id;
                //sm(agent.Messages);
                return Ok(await agent.Add<peak_summary>(entity));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }
                
        [HttpPost][Authorize(Policy = "CanModify")]
        [Route("Batch")]
        public async Task<IActionResult> Batch([FromBody]List<peak_summary> entities)
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
                return Ok(await agent.Add<peak_summary>(entities));
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
        public async Task<IActionResult> Put(int id, [FromBody]peak_summary entity)
        {
            try
            {
                if (id < 0 || !isValid(entity)) return new BadRequestResult();
                var loggedInMember = LoggedInUser();
                if (loggedInMember == null) return new BadRequestObjectResult("Invalid input parameters");

                entity.last_updated = DateTime.Now;
                entity.last_updated_by = loggedInMember.member_id;
                return Ok(await agent.Update<peak_summary>(id, entity));
            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex);
            }           
        }
        #endregion
        
        #region DELETE
        [HttpDelete("{id}")][Authorize(Policy = "Restricted")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id < 1) return new BadRequestResult();
                var entity = await agent.Find<peak_summary>(id);
                if (entity == null) return new NotFoundResult();

                await agent.Delete<peak_summary>(entity);
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
