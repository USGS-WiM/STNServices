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
    public class ReportingMetricsController : STNControllerBase
    {
        public ReportingMetricsController(ISTNServicesAgent sa) : base(sa)
        {}

        #region METHODS
        
        #region GET
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                //sm(agent.Messages);
                return Ok(agent.Select<reporting_metrics>());
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
                return Ok(await agent.Find<reporting_metrics>(id));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }
                
        [HttpGet("/ReportResource/{entityId}")] 
        public async Task<IActionResult> GetReportModel(int entityId)
        {
            try
            {
                if (entityId < 0) return new BadRequestResult();

                var objectRequested = agent.Select<reporting_metrics>().Include(r=>r.reportmetric_contact).ThenInclude(rc=> rc.contact_type)
                    .Where(r => r.reporting_metrics_id == entityId).Select(rm => new ReportResource
                {
                        reporting_metrics_id = rm.reporting_metrics_id,
                        event_id = rm.event_id,
                        sw_fieldpers_notacct = rm.sw_fieldpers_notacct,
                        wq_fieldpers_notacct = rm.wq_fieldpers_notacct,
                        yest_fieldpers = rm.yest_fieldpers,
                        tod_fieldpers = rm.tod_fieldpers,
                        tmw_fieldpers = rm.tmw_fieldpers,
                        yest_officepers = rm.yest_officepers,
                        tod_officepers = rm.tod_officepers,
                        tmw_officepers = rm.tmw_officepers,
                        gage_visit = rm.gage_visit,
                        gage_down = rm.gage_down,
                        tot_discharge_meas = rm.tot_discharge_meas,
                        plan_discharge_meas = rm.plan_discharge_meas,
                        plan_indirect_meas = rm.plan_indirect_meas,
                        rating_extens = rm.rating_extens,
                        gage_peak_record = rm.gage_peak_record,
                        plan_rapdepl_gage = rm.plan_rapdepl_gage,
                        dep_rapdepl_gage = rm.dep_rapdepl_gage,
                        rec_rapdepl_gage = rm.rec_rapdepl_gage,
                        lost_rapdepl_gage = rm.lost_rapdepl_gage,
                        plan_wtrlev_sensor = rm.plan_wtrlev_sensor,
                        dep_wtrlev_sensor = rm.dep_wtrlev_sensor,
                        rec_wtrlev_sensor = rm.rec_wtrlev_sensor,
                        lost_wtrlev_sensor = rm.lost_wtrlev_sensor,
                        plan_wv_sens = rm.plan_wv_sens,
                        dep_wv_sens = rm.dep_wv_sens,
                        rec_wv_sens = rm.rec_wv_sens,
                        lost_wv_sens = rm.lost_wv_sens,
                        plan_barometric = rm.plan_barometric,
                        dep_barometric = rm.dep_barometric,
                        rec_barometric = rm.rec_barometric,
                        lost_barometric = rm.lost_barometric,
                        plan_meteorological = rm.plan_meteorological,
                        dep_meteorological = rm.dep_meteorological,
                        rec_meteorological = rm.rec_meteorological,
                        lost_meteorological = rm.lost_meteorological,
                        hwm_flagged = rm.hwm_flagged,
                        hwm_collected = rm.hwm_collected,
                        qw_discr_samples = rm.qw_discr_samples,
                        coll_sedsamples = rm.coll_sedsamples,
                        member_id = rm.member_id,
                        complete = rm.complete,
                        notes = rm.notes,
                        report_date = rm.report_date,
                        state = rm.state,
                        ReportContacts = rm.reportmetric_contact.Select(rc => new ReportContactModel
                        {
                            fname = rc.contact.fname,
                            lname = rc.contact.lname,
                            email = rc.contact.email,
                            phone = rc.contact.phone,
                            alt_phone = rc.contact.alt_phone,
                            contact_id = rc.contact.contact_id,
                            type = rc.contact_type.type
                        }).ToList<ReportContactModel>()
                    }).FirstOrDefault();

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

        [HttpGet("/ReportResource/FilteredReportModel")] // ?Event={eventId}&States={stateNames}&Date={aDate}"
        public async Task<IActionResult> GetFilteredReportsModel([FromQuery] int Event, [FromQuery] string State = "", [FromQuery] string Date = "")
        {
            try
            {
                if (Date == string.Empty || Event <= 0)
                    return new BadRequestResult();

                //sm(agent.Messages);
                return Ok(agent.GetFilteredReportsModel(Event, State, Date));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpGet("FilteredReports")] // ?Event={eventId}&States={stateNames}&Date={aDate}
        public async Task<IActionResult> GetFilteredReports([FromQuery] string Event = "", [FromQuery] string Date = "", [FromQuery] string States = "")
        {
            try
            {
                if (Date == string.Empty)
                    return new BadRequestResult();

                //sm(agent.Messages);
                return Ok(agent.GetFiltedReports(Event, Date, States));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpGet("DailyReportTotals")] // ?Date={date}&Event={eventId}&State={stateName}
        public async Task<IActionResult> GetDailyReportTotals([FromQuery] string Date, [FromQuery] int Event, [FromQuery] string State)
        {
            try
            {
                if (Date == string.Empty || Event <= 0 || string.IsNullOrEmpty(State))
                    return new BadRequestResult();

                string stateAbbrev = State.ToUpper();
                DateTime aDate = Convert.ToDateTime(Date).Date;

                //statusType 1 : Deployed, statusType 2 : Retrieved, statusType 3 : lost

                //query instruments with specified eventid, state (input can handle state name or abbr name)                    
                IQueryable<instrument> sensorQuery = agent.Select<instrument>().Include(i => i.instrument_status).Include(i => i.site)
                    .Where(i => i.event_id == Event && i.site.state.Equals(stateAbbrev));

                //query instruments by the date being less than status date
                List<instrument> sensorList = sensorQuery.AsEnumerable().Where(i => i.instrument_status.OrderByDescending(instStat => instStat.instrument_status_id)
                                                                .FirstOrDefault().time_stamp.Value <= aDate).ToList();


                List<instrument> depINSTs = sensorList.Where(i => i.instrument_status.OrderByDescending(inst => inst.time_stamp).FirstOrDefault().status_type_id == 1).ToList();
                List<instrument> recINSTs = sensorList.Where(i => i.instrument_status.OrderByDescending(inst => inst.time_stamp).FirstOrDefault().status_type_id == 2).ToList();
                List<instrument> lostINSTs = sensorList.Where(i => i.instrument_status.OrderByDescending(inst => inst.time_stamp).FirstOrDefault().status_type_id == 3).ToList();

                //RDG totals ( S(5) )
                Int32 DEPrapidDeploymentGageCount = depINSTs.Where(s => s.sensor_type_id == 5).ToList().Count();
                Int32 RECrapidDeploymentGageCount = recINSTs.Where(s => s.sensor_type_id == 5).ToList().Count();
                Int32 LOSTrapidDeploymentGageCount = lostINSTs.Where(s => s.sensor_type_id == 5).ToList().Count();

                //water level ( S(1), D(1) )
                Int32 DEPwaterLevelCount = depINSTs.Where(s => s.sensor_type_id == 1 && s.deployment_type_id == 1).ToList().Count();
                Int32 RECwaterLevelCount = recINSTs.Where(s => s.sensor_type_id == 1 && s.deployment_type_id == 1).ToList().Count();
                Int32 LOSTwaterLevelCount = lostINSTs.Where(s => s.sensor_type_id == 1 && s.deployment_type_id == 1).ToList().Count();

                //wave height ( S(1), D(2) )
                Int32 DEPwaveHeightCount = depINSTs.Where(s => s.sensor_type_id == 1 && s.deployment_type_id == 2).ToList().Count();
                Int32 RECwaveHeightCount = recINSTs.Where(s => s.sensor_type_id == 1 && s.deployment_type_id == 2).ToList().Count();
                Int32 LOSTwaveHeightCount = lostINSTs.Where(s => s.sensor_type_id == 1 && s.deployment_type_id == 2).ToList().Count();

                //Barometric totals ( S(1), D(3) )
                Int32 DEPbarometricCount = depINSTs.Where(s => s.sensor_type_id == 1 && s.deployment_type_id == 3).ToList().Count();
                Int32 RECbarometricCount = recINSTs.Where(s => s.sensor_type_id == 1 && s.deployment_type_id == 3).ToList().Count();
                Int32 LOSTbarometricCount = lostINSTs.Where(s => s.sensor_type_id == 1 && s.deployment_type_id == 3).ToList().Count();

                //Met totals ( S(2) )
                Int32 DEPmetCount = depINSTs.Where(s => s.sensor_type_id == 2).ToList().Count();
                Int32 RECmetCount = recINSTs.Where(s => s.sensor_type_id == 2).ToList().Count();
                Int32 LOSTmetCount = lostINSTs.Where(s => s.sensor_type_id == 2).ToList().Count();

                //now go get the HWMs for Flagged and Collected
                IQueryable<hwm> hwmQuery = agent.Select<hwm>().Include(h => h.site).Include(h => h.@event).Where(i => i.event_id == Event && i.site.state.Equals(stateAbbrev));

                //query instruments by the date being less than status date
                hwmQuery = hwmQuery.Where(i => i.@event.event_start_date <= aDate.Date);

                Int32 hwmFlagged = hwmQuery.Where(h => h.flag_date != null).ToList().Count();
                Int32 hwmCollected = hwmQuery.Where(h => h.elev_ft != null).ToList().Count();

                //now populate the report to pass back
                reporting_metrics ReportForTotals = new reporting_metrics();
                ReportForTotals.dep_rapdepl_gage = DEPrapidDeploymentGageCount >= 1 ? DEPrapidDeploymentGageCount : 0;
                ReportForTotals.rec_rapdepl_gage = RECrapidDeploymentGageCount >= 1 ? RECrapidDeploymentGageCount : 0;
                ReportForTotals.lost_rapdepl_gage = LOSTrapidDeploymentGageCount >= 1 ? LOSTrapidDeploymentGageCount : 0;
                ReportForTotals.dep_wtrlev_sensor = DEPwaterLevelCount >= 1 ? DEPwaterLevelCount : 0;
                ReportForTotals.rec_wtrlev_sensor = RECwaterLevelCount >= 1 ? RECwaterLevelCount : 0;
                ReportForTotals.lost_wtrlev_sensor = LOSTwaterLevelCount >= 1 ? LOSTwaterLevelCount : 0;
                ReportForTotals.dep_wv_sens = DEPwaveHeightCount >= 1 ? DEPwaveHeightCount : 0;
                ReportForTotals.rec_wv_sens = RECwaveHeightCount >= 1 ? RECwaveHeightCount : 0;
                ReportForTotals.lost_wv_sens = LOSTwaveHeightCount >= 1 ? LOSTwaveHeightCount : 0;
                ReportForTotals.dep_barometric = DEPbarometricCount >= 1 ? DEPbarometricCount : 0;
                ReportForTotals.rec_barometric = RECbarometricCount >= 1 ? RECbarometricCount : 0;
                ReportForTotals.lost_barometric = LOSTbarometricCount >= 1 ? LOSTbarometricCount : 0;
                ReportForTotals.dep_meteorological = DEPmetCount >= 1 ? DEPmetCount : 0;
                ReportForTotals.rec_meteorological = RECmetCount >= 1 ? RECmetCount : 0;
                ReportForTotals.lost_meteorological = LOSTmetCount >= 1 ? LOSTmetCount : 0;
                ReportForTotals.hwm_flagged = hwmFlagged >= 1 ? hwmFlagged : 0;
                ReportForTotals.hwm_collected = hwmCollected >= 1 ? hwmCollected : 0;


                if (ReportForTotals == null) return new BadRequestObjectResult(new Error(errorEnum.e_notFound));
                //sm(agent.Messages);
                return Ok(ReportForTotals);
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpGet("/Members/{memberId}/Reports")]
        public async Task<IActionResult> GetMemberReports(int memberId)
        {
            try
            {
                if (memberId < 0) return new BadRequestResult();

                var objectsRequested = agent.Select<reporting_metrics>().Where(x => x.member_id == memberId);
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

        [HttpGet("ReportsByDate")] // ?Date={aDate}
        public async Task<IActionResult> GetReportsByDate([FromQuery] string Date)
        {
            try
            {
                if (string.IsNullOrEmpty(Date)) return new BadRequestResult();

                DateTime formattedDate = Convert.ToDateTime(Date).Date;

                var objectsRequested = agent.Select<reporting_metrics>().Where(x => x.report_date == formattedDate);

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

        [HttpGet("ByEventAndState")] // ?Event={eventId}&State={stateName}
        public async Task<IActionResult> GetReportsByEventAndState([FromQuery] int Event, [FromQuery] string State)
        {
            try
            {
                if (Event <= 0 || string.IsNullOrEmpty(State)) return new BadRequestResult();
                string stateAbbrev = State.ToUpper();

                var objectsRequested = agent.Select<reporting_metrics>().Where(x => x.event_id == Event && x.state == stateAbbrev);

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

        [HttpGet("/Events/{eventId}/Reports")]
        public async Task<IActionResult> GetEventReports(int eventId)
        {
            try
            {
                if (eventId < 0) return new BadRequestResult();

                var objectsRequested = agent.Select<reporting_metrics>().Where(x => x.event_id == eventId);
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
        public async Task<IActionResult> Post([FromBody]reporting_metrics entity)
        {
            try
            {
                if (!isValid(entity)) return new BadRequestResult();
                var loggedInMember = LoggedInUser();
                if (loggedInMember == null) return new BadRequestObjectResult("Invalid input parameters");
                entity.last_updated = DateTime.Now;
                entity.last_updated_by = loggedInMember.member_id;
                //sm(agent.Messages);
                return Ok(await agent.Add<reporting_metrics>(entity));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpPost][Authorize(Policy = "AdminOnly")]
        [Route("Batch")]
        public async Task<IActionResult> Batch([FromBody]List<reporting_metrics> entities)
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
                return Ok(await agent.Add<reporting_metrics>(entities));
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
        public async Task<IActionResult> Put(int id, [FromBody]reporting_metrics entity)
        {
            try
            {
                if (id < 0 || !isValid(entity)) return new BadRequestResult();

                 var loggedInMember = LoggedInUser();
                 if (loggedInMember == null) return new BadRequestObjectResult("Invalid input parameters");

                entity.last_updated_by =  loggedInMember.member_id;
                entity.last_updated = DateTime.Now;

                return Ok(await agent.Update<reporting_metrics>(id, entity));
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
                var entity = await agent.Find<reporting_metrics>(id);
                if (entity == null) return new NotFoundResult();

                await agent.Delete<reporting_metrics>(entity);
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
