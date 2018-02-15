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
    public class ContactsController : STNControllerBase
    {
        public ContactsController(ISTNServicesAgent sa) : base(sa)
        { }

        #region METHODS

        #region GET
        [HttpGet]
        [Authorize(Policy = "CanModify")] // (admin,manager,field)
        public async Task<IActionResult> Get()
        {
            try
            {
                //sm(agent.Messages);
                return Ok(agent.Select<contact>().OrderBy(c => c.lname));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "CanModify")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id < 0) return new BadRequestObjectResult("Invalid input parameters");
                //sm(agent.Messages);
                return Ok(await agent.Find<contact>(id));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpGet("byReportContactType")]//"Contacts/byReportContactType?ReportMetric={reportMetricsId}&ContactType={contactTypeId}" == was==> Contacts?ReportMetric={reportMetricsId}&ContactType={contactTypeId}")]
        [Authorize(Policy = "CanModify")]
        public async Task<IActionResult> GetReportMetricContactsByType([FromQuery] int ReportMetric, [FromQuery] int ContactType)
        {
            try
            {
                if (ReportMetric < 0 || ContactType < 0) return new BadRequestResult(); // This returns HTTP 404

                var objectRequested = agent.Select<reportmetric_contact>()
                    .Include(rc => rc.contact)
                    .FirstOrDefault(x => x.reporting_metrics_id == ReportMetric && x.contact_type_id == ContactType).contact;
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

        [HttpGet("byReport")]//"Contacts /byReport?ReportMetric={reportMetricsId} == was ==>/Contacts?ReportMetric={reportMetricsId}")]
        [Authorize(Policy = "CanModify")]
        public async Task<IActionResult> GetReportMetricContacts([FromQuery] int ReportMetric)
        {
            try
            {
                if (ReportMetric < 0) return new BadRequestResult(); 

                var objectsRequested = agent.Select<reportmetric_contact>().Include(m => m.contact).Include(m => m.contact_type).Where(x => x.reporting_metrics_id == ReportMetric);
                if (objectsRequested == null) return new BadRequestObjectResult(new Error(errorEnum.e_notFound)); // This returns HTTP 400
                
                //sm(agent.Messages);
                return Ok(objectsRequested.Select(c => new Contact()
                        {
                            contact_id = c.contact_id,
                            fname = c.contact.fname,
                            lname = c.contact.lname,
                            phone = c.contact.phone,
                            alt_phone = c.contact.alt_phone,
                            email = c.contact.email,
                            contactType = c.contact_type.type
                        }));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }
        #endregion

        #region POST

        [HttpPost("/ReportingMetrics/{reportId}/AddContactType/{contactTypeId}")]
        [Authorize(Policy = "CanModify")]
        public async Task<IActionResult> AddReportContact(int reportId, int contactTypeId, [FromBody]contact entity)
        {
            try
            { 
                reportmetric_contact newRepContact = null;
                contact thisContact = null;
                Int32 loggedInUserId = 0;

                if (!isValid(entity)) return new BadRequestResult();

                var loggedInMember = LoggedInUser();
                loggedInUserId = loggedInMember.member_id;
                        
                //check if valid Contact Type

                if (await agent.Find<contact_type>(contactTypeId) == null) return new BadRequestResult();
                if (await agent.Find<reporting_metrics>(reportId) == null) return new BadRequestResult();
                
                //save the contact
                thisContact = await agent.Add<contact>(entity);

                // last_updated parts (need to do this after add in case contact already exists, don't want to make a new one)
                thisContact.last_updated = DateTime.Now;
                thisContact.last_updated_by = loggedInUserId;
                await agent.Update(thisContact.contact_id, thisContact);
                
                //add ReportMetrics_Contact if not already there.
                if (agent.Select<reportmetric_contact>().FirstOrDefault(rt =>
                    rt.contact_id == thisContact.contact_id && rt.reporting_metrics_id == reportId && rt.contact_type_id == contactTypeId) == null)
                {
                    newRepContact = new reportmetric_contact();
                    newRepContact.reporting_metrics_id = reportId;
                    newRepContact.contact_type_id = contactTypeId;
                    newRepContact.contact_id = thisContact.contact_id;
                    // last_updated parts
                    newRepContact.last_updated = DateTime.Now;
                    newRepContact.last_updated_by = loggedInUserId;
                    newRepContact = await agent.Add<reportmetric_contact>(newRepContact);
                }//end if

                //sm(agent.Messages);
                return Ok(thisContact);
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpPost][Authorize(Policy = "CanModify")]
        [Route("Batch")]
        public async Task<IActionResult> Batch([FromBody]List<contact> entities)
        {
            try
            {
                if (!isValid(entities)) return new BadRequestObjectResult("Object is invalid");
                //sm(agent.Messages);
                return Ok(await agent.Add<contact>(entities));
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
        public async Task<IActionResult> Put(int id, [FromBody]contact entity)
        {
            Int32 loggedInUserId = 0;
            try
            {
                if (id < 0 || !isValid(entity)) return new BadRequestResult();

                // last_updated parts
                var loggedInMember = LoggedInUser();
                if (loggedInMember == null) return new BadRequestObjectResult("Invalid input parameters");
                loggedInUserId = loggedInMember.member_id;
                entity.last_updated = DateTime.Now;
                entity.last_updated_by = loggedInUserId;

                //sm(agent.Messages);
                return Ok(await agent.Update<contact>(id, entity));                
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
                var entity = await agent.Find<contact>(id);
                if (entity == null) return new NotFoundResult();

                await agent.Delete<contact>(entity);
                //sm(agent.Messages);
                return Ok();

            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }
                
        [HttpDelete("{contactId}/removeReportContact")] //(contactResource + "/{contactId}/removeReportContact?ReportId={reportMetricsId}")
        [Authorize(Policy = "CanModify")]
        public async Task<IActionResult> RemoveReportContact(int contactId, [FromQuery] int ReportId)
        {
            try
            {
                if (contactId < 1 || ReportId < 1) return new BadRequestResult();
                
                //check if valid site
                if (await agent.Find<reporting_metrics>(ReportId) == null) return new NotFoundResult();

                //remove from Reporting_Contact
                reportmetric_contact entity = agent.Select<reportmetric_contact>().FirstOrDefault(rmc => rmc.contact_id == contactId && rmc.reporting_metrics_id == ReportId);
                if (entity == null) return new NotFoundResult();
                await agent.Delete<reportmetric_contact>(entity);
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
