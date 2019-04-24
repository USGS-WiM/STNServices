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
using WiM.Security;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace STNServices.Controllers
{
    [Route("[controller]")]
    public class MembersController : STNControllerBase
    {
        public MembersController(ISTNServicesAgent sa) : base(sa)
        { }
        #region METHODS

        #region GET
        [HttpGet]
        [Authorize(Policy = "CanModify")]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(agent.Select<members>().Select(m => new members()
                {
                    member_id = m.member_id,
                    email = m.email,
                    fname = m.fname,
                    lname = m.lname,
                    agency_id = m.agency_id,
                    emergency_contact_name = m.emergency_contact_name,
                    emergency_contact_phone = m.emergency_contact_phone,
                    phone = m.phone,
                    resetFlag = m.resetFlag,
                    username = m.username,
                    role_id = m.role_id
                }));
            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "CanModify")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id < 0) return new BadRequestResult();

                var x = await agent.Find<members>(id);
                //remove info not relevant
                x.salt = string.Empty;
                x.password = string.Empty;

                return Ok(x);
            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpGet("/Login")]
        [Authorize(Policy = "CanModify")]
        public async Task<IActionResult> GetLoggedUser()
        {
            try
            {
                return Ok(LoggedInUser());
            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex);
            }

        }

        //for unauthorized access to just the member's name and no other info
        [HttpGet("GetMemberName/{entityId}")]
        public async Task<IActionResult> GetMemberName(Int32 entityId)
        {
            try
            {
                if (entityId < 0) return new BadRequestResult();

                var objectRequested = agent.Select<members>().FirstOrDefault(x => x.member_id == entityId);
                if (objectRequested == null) return new BadRequestObjectResult(new Error(errorEnum.e_notFound));


                //sm(agent.Messages);
                return Ok(objectRequested.fname + " " + objectRequested.lname);
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpGet("/Agencies/{agencyId}/[controller]")]
        [Authorize(Policy = "CanModify")]
        public async Task<IActionResult> GetAgencyMembers(Int32 agencyId)
        {
            try
            {
                if (agencyId < 0) return new BadRequestResult();

                var objectRequested = agent.Select<agency>().Include(a => a.members).FirstOrDefault(x => x.agency_id == agencyId).members;
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

        [HttpGet("/Roles/{roleId}/[controller]")]
        [Authorize(Policy = "CanModify")]
        public async Task<IActionResult> GetRoleMembers(Int32 roleId)
        {
            try
            {
                if (roleId < 0) return new BadRequestResult();

                var objectRequested = agent.Select<roles>().Include(a => a.members).FirstOrDefault(x => x.role_id == roleId).members;
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

        [HttpGet("/Events/{eventId}/EventCoordinator")]
        [Authorize(Policy = "CanModify")]
        public async Task<IActionResult> GetEventCoordinator(Int32 eventId)
        {
            try
            {
                if (eventId < 0) return new BadRequestResult();

                var objectRequested = agent.Select<events>().Include(a => a.members).FirstOrDefault(x => x.event_id == eventId).members;
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

        [HttpGet("/Approvals/{approvalId}/ApprovingOfficial")]
        [Authorize(Policy = "CanModify")]
        public async Task<IActionResult> GetApprovingOfficial(Int32 approvalId)
        {
            try
            {
                if (approvalId < 0) return new BadRequestResult();

                var objectRequested = agent.Select<approval>().Include(a => a.members).FirstOrDefault(x => x.approval_id == approvalId).members;
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

        [HttpGet("/DataFiles/{dataFileId}/Processor")]
        [Authorize(Policy = "CanModify")]
        public async Task<IActionResult> GetDataFileProcessor(Int32 dataFileId)
        {
            try
            {
                if (dataFileId < 0) return new BadRequestResult();

                var objectRequested = agent.Select<data_file>().Include(a => a.members).FirstOrDefault(x => x.data_file_id == dataFileId).members;
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

        [HttpGet("/PeakSummaries/{peakSummaryId}/Processor")]
        [Authorize(Policy = "CanModify")]
        public async Task<IActionResult> GetPeakSummaryProcessor(Int32 peakSummaryId)
        {
            try
            {
                if (peakSummaryId < 0) return new BadRequestResult();

                var objectRequested = agent.Select<peak_summary>().Include(a => a.members).FirstOrDefault(x => x.peak_summary_id == peakSummaryId).members;
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
        [Authorize(Policy = "CanModify")]
        public async Task<IActionResult> GetEventMembers(Int32 eventId)
        {
            try
            {
                if (eventId < 0) return new BadRequestResult();

                var objectRequested = agent.Select<members>()
                    .Include(m => m.flag_memberHWMs).Include(m => m.survey_memberHWMs)
                    .Include(m => m.instrument_status).ThenInclude(inst => inst.instrument)
                    .Where(m => m.flag_memberHWMs.Any(h => h.event_id == eventId) || m.survey_memberHWMs.Any(h => h.event_id == eventId) || m.instrument_status.Any(inst => inst.instrument.event_id == eventId));

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
        [Authorize(Policy = "Restricted")] //("Admin", "Manager")
        public async Task<IActionResult> Post([FromBody] members entity)
        {
            try
            {
                if (string.IsNullOrEmpty(entity.fname) || string.IsNullOrEmpty(entity.lname) || string.IsNullOrEmpty(entity.username) ||
                    string.IsNullOrEmpty(entity.phone) || string.IsNullOrEmpty(entity.email) || entity.agency_id < 0)
                    return new BadRequestObjectResult(new Error(errorEnum.e_badRequest, "You are missing one or more required parameter."));

                if (string.IsNullOrEmpty(entity.password))
                    entity.password = generateDefaultPassword(entity);
                else
                    entity.password = Encoding.UTF8.GetString(Convert.FromBase64String(entity.password));

                if (entity.role_id <= 0) entity.role_id = agent.Select<roles>().SingleOrDefault(r => r.role_name == FieldRole).role_id;

                entity.salt = Cryptography.CreateSalt();
                entity.password = Cryptography.GenerateSHA256Hash(entity.password, entity.salt);

                var loggedInMember = LoggedInUser();
                if (loggedInMember == null) return new BadRequestObjectResult("Invalid input parameters");

                entity.last_updated = DateTime.Now;
                entity.last_updated_by = loggedInMember.member_id;

                if (!isValid(entity)) return new BadRequestResult(); // This returns HTTP 404
                var x = await agent.Add<members>(entity);
                //remove info not relevant
                x.salt = string.Empty;
                x.password = string.Empty;

                return Ok(x);
            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex);
            }
        }
        #endregion  

        #region PUT
        [HttpPut("{id}")]
        [Authorize(Policy = "CanModify")]
        public async Task<IActionResult> Put(int id, [FromBody]members entity)
        {
            members ObjectToBeUpdated = null;
            try
            {
                if (string.IsNullOrEmpty(entity.fname) || string.IsNullOrEmpty(entity.lname) || string.IsNullOrEmpty(entity.username) ||
                    string.IsNullOrEmpty(entity.phone) || string.IsNullOrEmpty(entity.email) || entity.agency_id < 1 || entity.role_id < 1)
                    return new BadRequestObjectResult(new Error(errorEnum.e_badRequest)); // This returns HTTP 404

                //fetch object, assuming it exists
                ObjectToBeUpdated = await agent.Find<members>(id);
                if (ObjectToBeUpdated == null) return new NotFoundObjectResult(entity);

                if ((!User.IsInRole("Admin") && !User.IsInRole("Manager")) && LoggedInUser().member_id != id)
                    return new UnauthorizedResult();

                ObjectToBeUpdated.username = entity.username;
                ObjectToBeUpdated.fname = entity.fname;
                ObjectToBeUpdated.lname = entity.lname;
                ObjectToBeUpdated.agency_id = entity.agency_id;
                ObjectToBeUpdated.phone = entity.phone;
                ObjectToBeUpdated.email = entity.email;
                ObjectToBeUpdated.emergency_contact_name = (string.IsNullOrEmpty(entity.emergency_contact_name) ?
                    ObjectToBeUpdated.emergency_contact_name : entity.emergency_contact_name);
                ObjectToBeUpdated.emergency_contact_phone = (string.IsNullOrEmpty(entity.emergency_contact_phone) ?
                    ObjectToBeUpdated.emergency_contact_phone : entity.emergency_contact_phone);

                //last updated parts
                var loggedInMember = LoggedInUser();
                if (loggedInMember == null) return new BadRequestObjectResult("Invalid input parameters");

                entity.last_updated = DateTime.Now;
                entity.last_updated_by = loggedInMember.member_id;

                //admin can only change role
                if (User.IsInRole("Admin"))
                    ObjectToBeUpdated.role_id = entity.role_id;

                //change password if needed
                if (!string.IsNullOrEmpty(entity.password) && !Cryptography
                            .VerifyPassword(Encoding.UTF8.GetString(Convert.FromBase64String(entity.password)),
                                                                    ObjectToBeUpdated.salt, ObjectToBeUpdated.password))
                {
                    ObjectToBeUpdated.salt = Cryptography.CreateSalt();
                    ObjectToBeUpdated.password = Cryptography.GenerateSHA256Hash(Encoding.UTF8
                                .GetString(Convert.FromBase64String(entity.password)), ObjectToBeUpdated.salt);
                    ObjectToBeUpdated.resetFlag = null;
                }//end if

                var x = await agent.Update<members>(id, ObjectToBeUpdated);

                //remove info not relevant
                x.salt = string.Empty;
                x.password = string.Empty;

                return Ok(x);
            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex);
            }
        }
        #endregion

        #region DELETE
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id < 1) return new BadRequestResult();
                var entity = await agent.Find<members>(id);
                if (entity == null) return new NotFoundResult();

                await agent.Delete<members>(entity);

                return Ok();

            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex);
            }
        }
        #endregion

        #endregion
        #region HELPER METHODS
        private string generateDefaultPassword(members entity)
        {
            //WUDefau1t+numbercharInlastname+first2letterFirstName
            string generatedPassword = "STNDefau1t" + entity.lname.Length + entity.fname.Substring(0, 2);

            return generatedPassword;
        }
        #endregion
    }
}
