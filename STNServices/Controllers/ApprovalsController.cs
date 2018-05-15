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
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace STNServices.Controllers
{
    [Route("[controller]")]
    public class ApprovalsController : STNControllerBase
    {
        public ApprovalsController(ISTNServicesAgent sa) : base(sa)
        { }

        #region METHOD
    
        #region GET
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                //sm(agent.Messages);
                return Ok(agent.Select<approval>());
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
                if (id < 0) return new BadRequestObjectResult("Invalid input parameters");
                //sm(agent.Messages);
                return Ok(await agent.Find<approval>(id));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        
        [HttpGet("/DataFiles/{dataFileId}/Approval")]
        public async Task<IActionResult> GetDataFileApproval(int dataFileId)
        {
            try
            {
                if (dataFileId < 0) return new BadRequestObjectResult("Invalid input parameters"); // This returns HTTP 404
                //  find == by primary key  use firstOrDefault == all others
                var objectRequested = agent.Select<data_file>().Include(m => m.approval).FirstOrDefault(x => x.data_file_id == dataFileId).approval;
                //sm(agent.Messages);
                return Ok(objectRequested);
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpGet("/HWMs/{hwmId}/Approval")]
        public async Task<IActionResult> GetHWMApproval(int hwmId)
        {
            try
            {
                if (hwmId < 0) return new BadRequestObjectResult("Invalid input parameters");

                var objectRequested = agent.Select<hwm>().Include(m => m.approval).FirstOrDefault(x => x.hwm_id == hwmId).approval;
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
        [HttpPost("/DataFiles/{dataFileId}/Approve")]
        [Authorize(Policy = "Restricted")]
        public async Task<IActionResult> ApproveDataFile(int dataFileId)
        {
            try
            {
                data_file aDataFile = null;
                approval anEntity = null;
                Int32 loggedInUserId = 0;
                
                if (dataFileId <= 0) return new BadRequestObjectResult("Invalid input parameters");
                anEntity = new approval();

                //get logged in user
                var loggedInMember = LoggedInUser();
                if (loggedInMember == null) return new BadRequestObjectResult("Invalid input parameters");
                loggedInUserId = loggedInMember.member_id;

                aDataFile = await agent.Find<data_file>(dataFileId);
                if (aDataFile == null) return new BadRequestObjectResult("invalid data file id");
                                
                //set time and user of approval
                anEntity.approval_date = DateTime.UtcNow;
                anEntity.member_id = loggedInUserId;
                anEntity = await agent.Add<approval>(anEntity);

                //set datafile approvalID
                aDataFile.approval_id = anEntity.approval_id;
                // last_update parts
                aDataFile.last_updated = DateTime.Now;
                aDataFile.last_updated_by = loggedInUserId;
                await agent.Update<data_file>(aDataFile.data_file_id, aDataFile);

                //sm(agent.Messages);
                return Ok(anEntity);
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpPost("/DataFiles/{dataFileId}/NWISApprove")]
        [Authorize(Policy = "Restricted")]
        public async Task<IActionResult> ApproveNWISDataFile(int dataFileId)
        {
            try
            {                
                data_file aDataFile = null;
                approval anEntity = null;
                Int32 loggedInUserId = 0;
                
                if (dataFileId <= 0) return new BadRequestObjectResult("Invalid input parameters");
                anEntity = new approval();

                //get logged in user
                var loggedInMember = LoggedInUser();
                if (loggedInMember == null) return new BadRequestObjectResult("Invalid input parameters");
                loggedInUserId = loggedInMember.member_id;

                aDataFile = agent.Select<data_file>().Include(d=>d.instrument).FirstOrDefault(d=>d.data_file_id == dataFileId);
                if (aDataFile == null) return new BadRequestObjectResult("invalid data file id");

                //get event coordinator to set as approver for NWIS datafile
                
                members eventCoor = agent.Select<members>().Include(m=>m.events).FirstOrDefault(m => m.events.Any(e => e.event_id == aDataFile.instrument.event_id));
                if (eventCoor == null) return new BadRequestObjectResult("invalid input parameters");

                //set time and user of approval
                anEntity.approval_date = DateTime.UtcNow;
                anEntity.member_id = eventCoor.member_id;
                anEntity = await agent.Add<approval>(anEntity);

                //set datafile approvalID
                aDataFile.approval_id = anEntity.approval_id;
                aDataFile.last_updated = DateTime.Now;
                aDataFile.last_updated_by = loggedInUserId;

                await agent.Update<data_file>(aDataFile.data_file_id, aDataFile);
               
                //sm(agent.Messages);
                return Ok(anEntity);
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpPost("/HWMs/{hwmId}/Approve")]
        [Authorize(Policy = "Restricted")]
        public async Task<IActionResult> ApproveHWM(int hwmId)
        {
            try
            {
                hwm aHWM = null;
                approval anEntity = null;
                Int32 loggedInUserId = 0;

                if (hwmId <= 0) return new BadRequestObjectResult("Invalid input parameters");

                anEntity = new approval();
                aHWM = await agent.Find<hwm>(hwmId);
                if (aHWM == null) return new BadRequestObjectResult("Invalid hwm id");

                //get logged in user
                var loggedInMember = LoggedInUser();
                if (loggedInMember == null) return new BadRequestObjectResult("Invalid input parameters");
                loggedInUserId = loggedInMember.member_id;

                //set time and user of approval
                anEntity.approval_date = DateTime.UtcNow;
                anEntity.member_id = loggedInMember.member_id;
                
                anEntity = await agent.Add<approval>(anEntity);

                //set HWM approvalID
                aHWM.approval_id = anEntity.approval_id;

                // last updated parts                
                aHWM.last_updated = DateTime.Now;
                aHWM.last_updated_by = loggedInUserId;
                await agent.Update<hwm>(aHWM.hwm_id, aHWM);
               
                //sm(agent.Messages);                
                return Ok(anEntity);
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        #endregion

        #region DELETE        
        [HttpDelete("{id}")]
        [Authorize(Policy = "Restricted")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                approval anEntity = null;
                Int32 loggedInUserId = 0;
            
                if (id <= 0) return new BadRequestObjectResult("Invalid input parameters");
            
                var loggedInMember = LoggedInUser();
                loggedInUserId = loggedInMember.member_id;

                //fetch the object to be updated (assuming that it exists)
                anEntity = await agent.Find<approval>(id);
                if (anEntity == null) return new NotFoundResult();

                //remove id from HWM or DF
                if (anEntity.hwms.Count > 0) anEntity.hwms.ToList().ForEach(x => 
                    {
                        x.approval_id = null;
                        // last updated parts                               
                        x.last_updated = DateTime.Now;
                        x.last_updated_by = loggedInUserId;
                        agent.Update<hwm>(x.hwm_id, x);
                    });
                if (anEntity.data_file.Count > 0) anEntity.data_file.ToList().ForEach(x => 
                    {
                        x.approval_id = null;
                        x.last_updated = DateTime.Now;
                        x.last_updated_by = loggedInUserId;
                        agent.Update<data_file>(x.data_file_id, x);
                    });
                //delete it
                await agent.Delete<approval>(anEntity);
                //sm(agent.Messages);
                  
                return Ok();
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpDelete("/HWMs/{hwmId}/Unapprove")]
        [Authorize(Policy = "Restricted")]
        public async Task<IActionResult> UnApproveHWM(int hwmId)
        {
            try
            {
                hwm aHWM = null;
                //Return BadRequest if missing required fields
                if (hwmId <= 0) return new BadRequestObjectResult("Invalid input parameters");
                
                //fetch the object to be updated (assuming that it exists)
                aHWM = await agent.Find<hwm>(hwmId);
                if (aHWM == null) return new BadRequestObjectResult("invalid hwm id");

                Int32 apprId = aHWM.approval_id.HasValue ? aHWM.approval_id.Value : 0;
                        
                //remove id from hwm
                aHWM.approval_id = null;
                        
                // last updated parts
                var loggedInMember = LoggedInUser();
                Int32 loggedInUserId = loggedInMember.member_id;
                aHWM.last_updated = DateTime.Now;
                aHWM.last_updated_by = loggedInUserId;
                await agent.Update<hwm>(aHWM.hwm_id, aHWM);

                approval anApproval = await agent.Find<approval>(apprId);
                //remove approval     
                await agent.Delete<approval>(anApproval);
                //sm(agent.Messages);
       
                return Ok();
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpDelete("/DataFiles/{dataFileId}/Unapprove")]
        [Authorize(Policy = "Restricted")]
        public async Task<IActionResult> UnApproveDataFile(int dataFileId)
        {
            try
            {
                data_file aDataFile = null;
                Int32 loggedInUserId = 0;
            
                //Return BadRequest if missing required fields
                if (dataFileId <= 0) return new BadRequestObjectResult("Invalid input parameters");

                var loggedInMember = LoggedInUser();
                loggedInUserId = loggedInMember.member_id;

                //fetch the object to be updated (assuming that it exists)
                aDataFile = await agent.Find<data_file>(dataFileId);
                if (aDataFile == null) return new BadRequestObjectResult("invalid data file id");

                Int32 apprId = aDataFile.approval_id.HasValue ? aDataFile.approval_id.Value : 0;
                //remove id from hwm
                aDataFile.approval_id = null;
                aDataFile.last_updated = DateTime.Now;
                aDataFile.last_updated_by = loggedInUserId;
                await agent.Update<data_file>(aDataFile.data_file_id, aDataFile);
                        
                approval anApproval = await agent.Find<approval>(apprId);
                //remove approval     
                await agent.Delete<approval>(anApproval);
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
