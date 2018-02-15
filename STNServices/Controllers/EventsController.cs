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
    public class EventsController : STNControllerBase
    {
        public EventsController(ISTNServicesAgent sa) : base(sa)
        {}

        #region METHODS
        
        #region GET
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                //sm(agent.Messages);
                return Ok(agent.Select<events>());
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
                return Ok(await agent.Find<events>(id));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpGet("/HWMs/{hwmId}/Event")]
        public async Task<IActionResult> GetHWMEvent(int hwmId)
        {
            try
            {
                if (hwmId < 0) return new BadRequestResult();

                var objectRequested = agent.Select<hwm>().Include(m => m.@event).FirstOrDefault(x => x.hwm_id == hwmId).@event;
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

        [HttpGet("/Instruments/{instrumentId}/Event")]
        public async Task<IActionResult> GetInstrumentEvent(int instrumentId)
        {
            try
            {
                if (instrumentId < 0) return new BadRequestResult(); // This returns HTTP 404

                var objectRequested = agent.Select<instrument>().Include(m => m.@event).FirstOrDefault(x => x.instrument_id == instrumentId).@event;
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
                
        [HttpGet("bySite")] //(eventsResource + "?Site={siteId}") ==> (eventsResource + "bySite?Site={siteId}")
        public async Task<IActionResult> GetSiteEvents([FromQuery] int Site)
        {
            try
            {
                if (Site < 0) return new BadRequestResult();
                
                var objectsRequested = agent.Select<events>()
                    .Include(e => e.hwms).Include(e => e.instruments)
                    .Where(e => e.hwms.Any(h => h.site_id == Site) || e.instruments.Any(ins => ins.site_id == Site));
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

        [HttpGet("/EventTypes/{eventTypeId}/[controller]")]
        public async Task<IActionResult> GetEventTypeEvents(int eventTypeId)
        {
            try
            {
                if (eventTypeId < 0) return new BadRequestResult();

                var objectsRequested = agent.Select<event_type>()
                    .Include(e => e.events).FirstOrDefault(e => e.event_type_id == eventTypeId).events;

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

        [HttpGet("/EventStatus/{eventStatusId}/[controller]")]
        public async Task<IActionResult> GetEventStatusEvents(int eventStatusId)
        {
            try
            {
                if (eventStatusId < 0) return new BadRequestResult();

                var objectsRequested = agent.Select<event_status>()
                    .Include(e => e.events).FirstOrDefault(e => e.event_status_id == eventStatusId).events;

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
                
        [HttpGet("FilteredEvents")] // (eventsResource+"/FilteredEvents?Date={date}&Type={eventTypeId}&State={stateName}")
        public async Task<IActionResult> GetFilteredEvents([FromQuery] string Date = "", [FromQuery] string Type = "", [FromQuery] string State = "")
        {
            try
            {
                //sm(agent.Messages);
                return Ok(agent.GetFiltedEvents(Date, Type, State));
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
        public async Task<IActionResult> Post([FromBody]events entity)
        {
            try
            {
                if (!isValid(entity)) return new BadRequestResult();

                var loggedInMember = LoggedInUser();
                if (loggedInMember == null) return new BadRequestObjectResult("Invalid input parameters");
                entity.last_updated = DateTime.Now;
                entity.last_updated_by = loggedInMember.member_id;

                //sm(agent.Messages);
                return Ok(await agent.Add<events>(entity));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpPost][Authorize(Policy = "AdminOnly")]
        [Route("Batch")]
        public async Task<IActionResult> Batch([FromBody]List<events> entities)
        {
            try
            {
                if (!isValid(entities)) return new BadRequestObjectResult("Object is invalid");
                entities.ForEach(e =>
                {
                    e.last_updated = DateTime.Now;
                    e.last_updated_by = LoggedInUser().member_id;
                });

                //sm(agent.Messages);
                return Ok(await agent.Add<events>(entities));
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
        public async Task<IActionResult> Put(int id, [FromBody]events entity)
        {
            try
            {
                if (id < 0 || !isValid(entity)) return new BadRequestResult(); // This returns HTTP 404
                //sm(agent.Messages);

                var loggedInMember = LoggedInUser();
                if (loggedInMember == null) return new BadRequestObjectResult("Invalid input parameters");                
                entity.last_updated = DateTime.Now;
                entity.last_updated_by = loggedInMember.member_id;

                return Ok(await agent.Update<events>(id, entity));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
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
                var entity = await agent.Find<events>(id);
                if (entity == null) return new NotFoundResult();

                await agent.Delete<events>(entity);
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
