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
    public class HWMsController : STNControllerBase
    {
        public HWMsController(ISTNServicesAgent sa) : base(sa)
        {}

        #region METHODS
        
        #region GET
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                //sm(agent.Messages);
                return Ok(agent.Select<hwm>());
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
                return Ok(await agent.Find<hwm>(id));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }
        
        [HttpGet("/Events/{eventId}/[controller]")]
        public async Task<IActionResult> GetEventHWMs(int eventId)
        {
            try
            {
                if (eventId < 0) return new BadRequestResult(); 

                var objectRequested = agent.Select<hwm>().Where(x => x.event_id == eventId);
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
                
        [HttpGet("/Events/{eventId}/stateHWMs")] //(eventsResource + "/{eventId}/stateHWMs?State={state}"
        public async Task<IActionResult> GetEventStateHWMs(string eventId, [FromQuery] string State)
        {
            try
            {
                if (string.IsNullOrEmpty(eventId) || string.IsNullOrEmpty(State)) return new BadRequestResult();

                Int32 filterEvent = (!string.IsNullOrEmpty(eventId)) ? Convert.ToInt32(eventId) : -1;

                IQueryable<hwm> query = agent.Select<hwm>().Include(h => h.site);
                if (filterEvent > 0)
                    query = query.Where(h => h.event_id == filterEvent);
                if (State != "")
                    query = query.Where(h => h.site.state.ToUpper() == State.ToUpper());

                if (query == null) return new BadRequestObjectResult(new Error(errorEnum.e_notFound));
                //sm(agent.Messages);
                return Ok(query);
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }
                
        [HttpGet("/Sites/{siteId}/EventHWMs")]//(siteResource+"/{siteId}/EventHWMs?Event={eventId}"
        public async Task<IActionResult> GetSiteEventHWMs(int siteId, [FromQuery] int Event)
        {
            try
            {
                if (Event < 1 || siteId < 1) return new BadRequestResult();

                var objectRequested = agent.Select<hwm>().Where(h => h.event_id == Event && h.site_id == siteId);

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
                
        [HttpGet("ByApprovalEventState")] //(hwmResource+"?IsApproved={approved}&Event={eventId}&State={state}&Counties={counties}")
        public async Task<IActionResult> GetApprovalHWMs([FromQuery] string IsApproved, [FromQuery] string Event = "", [FromQuery] string State = "", [FromQuery] string Counties = "")
        {
            try
            {
                if (string.IsNullOrEmpty(IsApproved)) return new BadRequestResult();
                List<hwm> entities = null;
                char[] countydelimiterChars = { ';', ',' };

                //set defaults
                bool isApprovedStatus = false;
                Boolean.TryParse(IsApproved, out isApprovedStatus);
                string filterState = State;
                List<String> countyList = !string.IsNullOrEmpty(Counties) ? Counties.ToUpper().Split(countydelimiterChars, StringSplitOptions.RemoveEmptyEntries).ToList() : null;
                Int32 filterEvent = (!string.IsNullOrEmpty(Event)) ? Convert.ToInt32(Event) : -1;
                IQueryable<hwm> query;
                if (isApprovedStatus)
                    query = agent.Select<hwm>().Include(h => h.site).Where(h => h.approval_id > 0);                
                else
                    query = agent.Select<hwm>().Include(h => h.site).Where(h => h.approval_id <= 0 || !h.approval_id.HasValue);

                if (filterEvent > 0)
                    query = query.Where(h => h.event_id == filterEvent);

                if (filterState != "")
                    query = query.Where(h => h.site.state == filterState);

                if (countyList != null && countyList.Count > 0)
                    query = query.Where(h => countyList.Contains(h.site.county.ToUpper()));

                entities = query.AsEnumerable().ToList();

                if (query == null) return new BadRequestObjectResult(new Error(errorEnum.e_notFound));
                //sm(agent.Messages);
                return Ok(entities);
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpGet("/Approvals/{ApprovalId}/[controller]")]
        public async Task<IActionResult> GetApprovedHWMs(int ApprovalId)
        {
            try
            {
                if (ApprovalId < 1) return new BadRequestResult();

                var objectRequested = agent.Select<approval>().Include(a => a.hwms).FirstOrDefault(a => a.approval_id == ApprovalId).hwms;

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

        [HttpGet("/Members/{memberId}/[controller]")]
        public async Task<IActionResult> GetMemberHWMs(int memberId)
        {
            try
            {
                if (memberId < 1) return new BadRequestResult();

                var objectsRequested = agent.Select<hwm>().Where(h => h.flag_member_id == memberId || h.survey_member_id == memberId);

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

        [HttpGet("/HWMQualities/{hwmQualityId}/[controller]")]
        public async Task<IActionResult> GetHWMQualityHWMs(int hwmQualityId)
        {
            try
            {
                if (hwmQualityId < 1) return new BadRequestResult();

                var objectsRequested = agent.Select<hwm>().Where(h => h.hwm_quality_id == hwmQualityId);

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

        [HttpGet("/HWMTypes/{hwmTypeId}/[controller]")]
        public async Task<IActionResult> GetHWMTypeHWMs(int hwmTypeId)
        {
            try
            {
                if (hwmTypeId < 1) return new BadRequestResult();

                var objectsRequested = agent.Select<hwm>().Where(h => h.hwm_type_id == hwmTypeId);

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

        [HttpGet("/HorizontalMethods/{hmethodId}/[controller]")]
        public async Task<IActionResult> GetHmethodHWMs(int hmethodId)
        {
            try
            {
                if (hmethodId < 1) return new BadRequestResult();

                var objectsRequested = agent.Select<hwm>().Where(h => h.hcollect_method_id == hmethodId);

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
        
        [HttpGet("/VerticalMethods/{vmethodId}/[controller]")]
        public async Task<IActionResult> GetVmethodHWMs(int vmethodId)
        {
            try
            {
                if (vmethodId < 1) return new BadRequestResult();

                var objectsRequested = agent.Select<hwm>().Where(h => h.vcollect_method_id == vmethodId);

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
        public async Task<IActionResult> GetSiteHWMs(int siteId)
        {
            try
            {
                if (siteId < 1) return new BadRequestResult();

                var objectsRequested = agent.Select<hwm>().Where(h => h.site_id == siteId);

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
        public async Task<IActionResult> GetVDatumHWMs(int vdatumId)
        {
            try
            {
                if (vdatumId < 1) return new BadRequestResult();

                var objectsRequested = agent.Select<hwm>().Where(h => h.vdatum_id == vdatumId);

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

        [HttpGet("/Markers/{markerId}/[controller]")]
        public async Task<IActionResult> GetMarkerHWMs(int markerId)
        {
            try
            {
                if (markerId < 1) return new BadRequestResult();

                var objectsRequested = agent.Select<hwm>().Where(h => h.marker_id == markerId);

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

        [HttpGet("/PeakSummaries/{peakSummaryId}/[controller]")]
        public async Task<IActionResult> GetPeakSummaryHWMs(int peakSummaryId)
        {
            try
            {
                if (peakSummaryId < 1) return new BadRequestResult();

                var objectsRequested = agent.Select<hwm>().Where(h => h.peak_summary_id == peakSummaryId);

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
        
        /*("/HWMs/FilteredHWMs?Event={eventIds}&EventType={eventTypeIDs}&EventStatus={eventStatusID}&States={states}&County={counties}&HWMType={hwmTypeIDs}
                    &HWMQuality={hwmQualIDs}&HWMEnvironment={hwmEnvironment}&SurveyComplete={surveyComplete}&StillWater={stillWater}") */        
        [HttpGet("FilteredHWMs")]
        public async Task<IActionResult> FilteredHWMs([FromQuery] string Event = "", [FromQuery] string EventType = "", [FromQuery] string EventStatus = "", 
                                        [FromQuery] string States = "", [FromQuery] string County = "", [FromQuery] string HWMType = "", [FromQuery] string HWMQuality = "", 
                                        [FromQuery] string HWMEnvironment = "", [FromQuery] string SurveyComplete = "", [FromQuery] string StillWater = "")
        {
            try
            {
                //sm(agent.Messages);
                return Ok(agent.GetFilterHWMs(Event, EventType, EventStatus, States, County, HWMType, HWMQuality, HWMEnvironment, SurveyComplete, StillWater));                
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
        public async Task<IActionResult> Post([FromBody]hwm entity)
        {
            try
            {
                if (!isValid(entity)) return new BadRequestResult();

                if (string.IsNullOrEmpty(entity.hwm_label))
                    entity.hwm_label = "no_label";

                var loggedInMember = LoggedInUser();
                if (loggedInMember == null) return new BadRequestObjectResult("Invalid input parameters");
                entity.last_updated = DateTime.Now;
                entity.last_updated_by = loggedInMember.member_id;

                //sm(agent.Messages);
                return Ok(await agent.Add<hwm>(entity));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }
                
        [HttpPost][Authorize(Policy = "CanModify")]
        [Route("Batch")]
        public async Task<IActionResult> Batch([FromBody]List<hwm> entities)
        {
            try
            {
                if (!isValid(entities)) return new BadRequestObjectResult("Object is invalid");

                var loggedInMember = LoggedInUser();
                if (loggedInMember == null) return new BadRequestObjectResult("Invalid input parameters");
                
                entities.ForEach(entity =>
                {
                    if (string.IsNullOrEmpty(entity.hwm_label))
                        entity.hwm_label = "no_label";

                    entity.last_updated = DateTime.Now;
                    entity.last_updated_by = loggedInMember.member_id;
                });

                //sm(agent.Messages);
                return Ok(await agent.Add<hwm>(entities));
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
        public async Task<IActionResult> Put(int id, [FromBody]hwm entity)
        {
            try
            {
                if (id < 0 || !isValid(entity)) return new BadRequestResult();

                var loggedInMember = LoggedInUser();
                if (loggedInMember == null) return new BadRequestObjectResult("Invalid input parameters");
                entity.last_updated = DateTime.Now;
                entity.last_updated_by = loggedInMember.member_id;

                return Ok(await agent.Update<hwm>(id, entity));
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
                var entity = await agent.Find<hwm>(id);
                if (entity == null) return new NotFoundResult();
                
                //Cascade delete
                var approvalID = entity.approval_id;

                //remove files
                //TODO___ entity.files.ToList().ForEach(f => agent.RemoveFileItem(f));
                if (entity.files != null)
                    entity.files.ToList().ForEach(f => agent.Delete<file>(f));

                //delete HWM now
                await agent.Delete<hwm>(entity);

                if (approvalID.HasValue)
                {
                    approval item = await agent.Find<approval>(approvalID.Value);
                    await agent.Delete<approval>(item);
                }
           
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
