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
using WiM.Resources;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.IO;

namespace STNServices.Controllers
{
    [Route("[controller]")]
    public class FilesController : STNControllerBase
    {
        public FilesController(ISTNServicesAgent sa) : base(sa)
        { }

        #region METHODS

        #region GET
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                //sm(agent.Messages);
                return Ok(agent.Select<file>());
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
                return Ok(await agent.Find<file>(id));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        //(fileResource+"/{id}/Item")
         [HttpGet("{id}/Item")]
         public async Task<IActionResult> GetFileItem(int id)
         {
             try
             {
                 if (id < 0) return new BadRequestResult();             
                 var fileItem = await agent.GetFileItem(id);
                 //sm(agent.Messages);

                 return Ok(fileItem);
             }
             catch (Exception ex)
             {
                 //sm(agent.Messages);
                 return await HandleExceptionAsync(ex);
             }
         }
/*
         //(hwmResource + "/historicHWMspreadsheet")
         [HttpGet("HWMs/historicHWMspreadsheet")]
         public async Task<IActionResult> GetHistoricHWMspreadsheet()
         {
             try
             {
                 var fileItem = agent.GetHWMSpreadsheetItem();
                 //sm(agent.Messages);
                 return Ok(fileItem);
             }
             catch (Exception ex)
             {
                 //sm(agent.Messages);
                 return await HandleExceptionAsync(ex);
             }
         }
         //(fileResource + "/testDataFile")
         [HttpOperation(HttpMethod.GET, ForUriName = "GetTESTdataFile")]
         public OperationResult GetTESTdataFile()
         {
             InMemoryFile fileItem;
             dynamic files;
             try
             {
                 using (STNAgent sa = new STNAgent())
                 {
                     fileItem = sa.GetTESTdataItem();
                     using (var fileStream = fileItem.OpenStream())

                     using (StreamReader reader = new StreamReader(fileStream))
                     {
                         string json = reader.ReadToEnd();
                         files = JsonConvert.DeserializeObject(json);
                     }

                     sm(sa.Messages);
                 }//end using
                 return new OperationResult.OK { ResponseResource = files, Description = this.MessageString };
             }
             catch (Exception ex)
             { return HandleException(ex); }
         }
              */

        //(hwmResource+"/{hwmId}/" + fileResource)
        [HttpGet("/HWMs/{hwmId}/[controller]")]
        public async Task<IActionResult> GetHWMFiles(int hwmId)
        {
            try
            {
                if (hwmId < 0) return new BadRequestResult();

                var objectsRequested = agent.Select<file>().Where(f => f.hwm_id == hwmId);
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

        //(objectivePointResource+"/{objectivePointId}/" + fileResource)
        [HttpGet("/ObjectivePoints/{objectivePointId}/[controller]")]
        public async Task<IActionResult> GetObjectivePointFiles(int objectivePointId)
        {
            try
            {
                if (objectivePointId < 0) return new BadRequestResult();

                var objectsRequested = agent.Select<file>().Where(f => f.objective_point_id == objectivePointId);
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

        //(filetypeResource+"/{fileTypeId}/" + fileResource)
        [HttpGet("/FileTypes/{fileTypeId}/[controller]")]
        public async Task<IActionResult> GetFileTypeFiles(int fileTypeId)
        {
            try
            {
                if (fileTypeId < 0) return new BadRequestResult();

                var objectsRequested = agent.Select<file>().Where(f => f.filetype_id == fileTypeId);
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

        //(siteResource+"/{siteId}/" + fileResource)
        [HttpGet("/Sites/{siteId}/[controller]")]
        public async Task<IActionResult> GetSiteFile(int siteId)
        {
            try
            {
                if (siteId < 0) return new BadRequestResult();

                var objectsRequested = agent.Select<file>().Where(f => f.site_id == siteId);
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

        //(sourceResource+"/{sourceId}/" + fileResource)
        [HttpGet("/Sources/{sourceId}/[controller]")]
        public async Task<IActionResult> GetSourceFiles(int sourceId)
        {
            try
            {
                if (sourceId < 0) return new BadRequestResult();

                var objectsRequested = agent.Select<file>().Where(f => f.source_id == sourceId);
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

        //(datafileResource+"/{dataFileId}/" + fileResource)
        [HttpGet("/DataFiles/{dataFileId}/[controller]")]
        public async Task<IActionResult> GetDataFileFiles(int dataFileId)
        {
            try
            {
                if (dataFileId < 0) return new BadRequestResult();

                var objectsRequested = agent.Select<file>().Where(f => f.data_file_id == dataFileId);
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

        //(instrumentsResource+"/{instrumentId}/" + fileResource)
        [HttpGet("/Instruments/{instrumentId}/[controller]")]
        public async Task<IActionResult> GetInstrumentFiles(int instrumentId)
        {
            try
            {
                if (instrumentId < 0) return new BadRequestResult();

                var objectsRequested = agent.Select<file>().Where(f => f.instrument_id == instrumentId);
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

        //(eventsResource+"/{eventId}/" + fileResource)
        [HttpGet("/Events/{eventId}/[controller]")]
        public async Task<IActionResult> GetEventFiles(int eventId)
        {
            try
            {
                if (eventId < 0) return new BadRequestResult();

                var objectsRequested = agent.Select<file>().Include(f => f.hwm).Include(f => f.instrument).ThenInclude(i => i.data_files)
                    .Where(f => f.hwm.event_id == eventId || f.instrument.event_id == eventId || f.data_file.instrument.event_id == eventId);

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

        //(fileResource+"?FromDate={fromDate}&ToDate={toDate}")
        [HttpGet]
        public async Task<IActionResult> GetFilesByDateRange([FromQuery] string fromDate, [FromQuery] string toDate)
        {
            try
            {
                DateTime? FromDate = agent.ValidDate(fromDate);
                DateTime? ToDate = agent.ValidDate(toDate);
                if (!FromDate.HasValue) return new BadRequestResult();
                if (!ToDate.HasValue) ToDate = DateTime.Now;
                
                var objectsRequested = agent.Select<file>().Where(f => f.file_date >= FromDate && f.file_date <= ToDate).OrderBy(f => f.file_date).ToList();

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

        //(fileResource+"?State={stateName}")
        [HttpGet]
        public async Task<IActionResult> GetFilesByStateName([FromQuery] string stateName)
        {
            try
            {
                if (string.IsNullOrEmpty(stateName)) return new BadRequestResult();

                var objectsRequested = agent.Select<file>()
                    .Include(f => f.hwm).ThenInclude(h => h.site)
                    .Include(f => f.instrument).ThenInclude(i => i.site)
                    .Include(f => f.data_file).ThenInclude(df => df.instrument).ThenInclude(i => i.site)
                    .Where(f => f.hwm.site.state.ToUpper() == stateName.ToUpper() ||
                           f.instrument.site.state.ToUpper() == stateName.ToUpper() ||
                           f.data_file.instrument.site.state.ToUpper() == stateName.ToUpper());

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

        //(fileResource+"?Site={siteId}&Event={eventId}")
        [HttpGet]
        public async Task<IActionResult> GetSiteEventFiles([FromQuery] int siteId, [FromQuery] int eventId)
        {
            try
            {
                if (siteId < 1 || eventId < 1) return new BadRequestResult();

                var objectsRequested = agent.Select<file>()
                    .Include(f => f.hwm)
                    .Include(f => f.instrument)
                    .Include(f => f.data_file).ThenInclude(df=> df.instrument)
                    .Where(f => f.site_id == siteId && 
                        (f.hwm.event_id == eventId || f.instrument.event_id == eventId || f.data_file.instrument.event_id == eventId));

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


        //(eventsResource + "/{eventId}/EventFileItems?FromDate={fromDate}&ToDate={toDate}&State={stateName}&County={county}&FilesFor={filesFor}&HWMFileType={hwmFileTypes}&SensorFileTypes={sensorFileTypes}")
         //[STNRequiresRole(new string[] { AdminRole, ManagerRole, FieldRole })] WHAT IS THIS?
         //[HttpGet("/Events/{eventId}/EventFileItems")]
         //[Authorize(Policy = "CanModify")]
         //public async Task<IActionResult> GetEventFileItems(Int32 eventId, [FromQuery] string fromDate, [FromQuery] string toDate, [FromQuery] string stateName, [FromQuery] string county, 
         //   [FromQuery] string filesFor, [FromQuery] string hwmFileTypes) //, [FromQuery] string sensorFileTypes)
         //{
         //    List<file> entities = null;
         //    MemoryStream fileItem = null;
         //    char[] delimiterChars = { ';', ',', ' ' };
         //    List<decimal> hwmFTList = !string.IsNullOrEmpty(hwmFileTypes) ? hwmFileTypes.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries).Select(decimal.Parse).ToList() : null;
         //    List<decimal> sensFTList = !string.IsNullOrEmpty(sensorFileTypes) ? sensorFileTypes.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries).Select(decimal.Parse).ToList() : null;
         //    DateTime? FromDate = ValidDate(fromDate);
         //    DateTime? ToDate = ValidDate(toDate);

         //    try
         //    {
         //        if (eventId <= 0 || (string.IsNullOrEmpty(filesFor) && (string.IsNullOrEmpty(sensorFiles)))) throw new BadRequestException("Invalid input parameters");
         //        using (EasySecureString securedPassword = GetSecuredPassword())
         //        {
         //            using (STNAgent sa = new STNAgent(username, securedPassword))
         //            {
         //                //all files for this event
         //                IQueryable<file> allEventFilesQuery = sa.Select<file>().Include(f => f.hwm).Include(f => f.instrument).Include("data_file.instrument")
         //                .Where(f => f.hwm.event_id == eventId || f.instrument.event_id == eventId || f.data_file.instrument.event_id == eventId);

         //                IQueryable<file> hwmFilesQuery = null;
         //                IQueryable<file> sensorFilesQuery = null;

         //                //date range
         //                if (FromDate.HasValue)
         //                    allEventFilesQuery = allEventFilesQuery.Where(f => f.file_date >= FromDate);

         //                if (ToDate.HasValue)
         //                    allEventFilesQuery = allEventFilesQuery.Where(f => f.file_date <= ToDate);

         //                //state
         //                if (!string.IsNullOrEmpty(stateName))
         //                    allEventFilesQuery = allEventFilesQuery.Where(f => f.hwm.site.state.ToUpper() == stateName.ToUpper() ||
         //                               f.instrument.site.state.ToUpper() == stateName.ToUpper() ||
         //                               f.data_file.instrument.site.state.ToUpper() == stateName.ToUpper());

         //                //county
         //                if (!string.IsNullOrEmpty(county))
         //                    allEventFilesQuery = allEventFilesQuery.Where(f => f.hwm.site.county.ToUpper() == county.ToUpper() ||
         //                                f.instrument.site.county.ToUpper() == county.ToUpper() ||
         //                                f.data_file.instrument.site.county.ToUpper() == county.ToUpper());
         //                //only HWM files only
         //                if (filesFor == "HWMs")
         //                {
         //                    //only site files
         //                    hwmFilesQuery = allEventFilesQuery.Where(f => (f.hwm_id.HasValue && f.hwm_id > 0) && (!f.instrument_id.HasValue || f.instrument_id == 0));

         //                    //now see which types they want
         //                    if (hwmFTList != null && hwmFTList.Count > 0)
         //                        hwmFilesQuery = hwmFilesQuery.Where(f => hwmFTList.Contains(f.filetype_id.Value));
         //                }
         //                //only Sensor files only
         //                if (filesFor == "Sensors")
         //                {
         //                    //only site files
         //                    sensorFilesQuery = allEventFilesQuery.Where(f => ((!f.hwm_id.HasValue || f.hwm_id == 0) && (!f.objective_point_id.HasValue || f.objective_point_id == 0)) && f.instrument_id.HasValue);

         //                    //now see which types they want
         //                    if (sensFTList != null && sensFTList.Count > 0)
         //                        sensorFilesQuery = sensorFilesQuery.Where(f => sensFTList.Contains(f.filetype_id.Value));
         //                }

         //                entities = new List<file>();
         //                if (hwmFilesQuery != null) hwmFilesQuery.ToList().ForEach(h => entities.Add(h));
         //                if (sensorFilesQuery != null) sensorFilesQuery.ToList().ForEach(s => entities.Add(s));

         //                sm(MessageType.info, "FileCount:" + entities.Count);
         //                fileItem = sa.GetFileItemZip(entities);

         //                sm(MessageType.info, "Count: " + entities.Count());
         //                sm(sa.Messages);
         //            }//end using
         //        }//end using

         //        return new OperationResult.OK { ResponseResource = fileItem, Description = this.MessageString };
         //    }
         //    catch (Exception ex)
         //    { return HandleException(ex); }
         //}//end HttpMethod.GET
         
        #endregion

        #region POST
        [HttpPost][Authorize(Policy = "CanModify")]
        public async Task<IActionResult> Post([FromBody]file entity)
        {
            try
            {
                if (!isValid(entity)) return new BadRequestResult();
                var loggedInMember = LoggedInUser();
                if (loggedInMember == null) return new BadRequestObjectResult("Invalid input parameters");
                entity.last_updated = DateTime.Now;
                entity.last_updated_by = loggedInMember.member_id;
                
                //sm(agent.Messages);
                return Ok(await agent.Add<file>(entity));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }
                
        [HttpPost][Authorize(Policy = "CanModify")]
        [Route("Batch")]
        public async Task<IActionResult> Batch([FromBody]List<file> entities)
        {
            try
            {
                if (!isValid(entities)) return new BadRequestObjectResult("Object is invalid");
                var loggedInMember = LoggedInUser();
                if (loggedInMember == null) return new BadRequestObjectResult("Invalid input parameters");
                
                entities.ForEach(f =>
                {
                    f.last_updated = DateTime.Now;
                    f.last_updated_by = loggedInMember.member_id;                    
                });

                //sm(agent.Messages);
                return Ok(await agent.Add<file>(entities));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }
        /*[STNRequiresRole(new string[] { AdminRole, ManagerRole, FieldRole })]
        [HttpOperation(HttpMethod.POST, ForUriName = "UploadFile")]
        public OperationResult UploadFile(IEnumerable<IMultipartHttpEntity> entities)
        {
            try
            {
                //TODO: The stream decoding should really be in a custom Codec
                using (var memoryStream = new MemoryStream())
                {
                    String filename = "";
                    XmlSerializer serializer;

                    file uploadFile = null;

                    foreach (var entity in entities)
                    {
                        //Process Stream
                        if (!entity.Headers.ContentDisposition.Disposition.ToLower().Equals("form-data"))
                            return new OperationResult.BadRequest { ResponseResource = "Sent a field that is not declared as form-data, cannot process" };

                        if (entity.Stream != null && entity.ContentType != null)
                        {
                            //Process Stream
                            if (entity.Headers.ContentDisposition.Name.Equals("File"))
                            {
                                entity.Stream.CopyTo(memoryStream);
                                filename = entity.Headers.ContentDisposition.FileName;
                            }
                        }
                        else
                        {
                            //Process Variables
                            if (entity.Headers.ContentDisposition.Name.Equals("FileEntity"))
                            {
                                var mem = new MemoryStream();
                                entity.Stream.CopyTo(mem);
                                mem.Position = 0;
                                try
                                {
                                    serializer = new XmlSerializer(typeof(file));
                                    uploadFile = (file)serializer.Deserialize(mem);
                                }
                                catch
                                {
                                    mem.Position = 0;
                                    JsonSerializer jsonSerializer = new JsonSerializer();
                                    using (StreamReader streamReader = new StreamReader(mem, new UTF8Encoding(false, true)))
                                    {
                                        using (JsonTextReader jsonTextReader = new JsonTextReader(streamReader))
                                        {
                                            uploadFile = (file)jsonSerializer.Deserialize(jsonTextReader, typeof(file));
                                        }
                                    }//end using
                                }
                            }
                        }
                    }//next
                    //Return BadRequest if missing required fields

                    if (uploadFile.filetype_id <= 0 || !uploadFile.file_date.HasValue ||
                        uploadFile.site_id <= 0)
                        throw new BadRequestException("Invalid input parameters");

                    //Get basic authentication password
                    using (EasySecureString securedPassword = GetSecuredPassword())
                    {
                        using (STNAgent sa = new STNAgent(username, securedPassword, true))
                        {                            
                            //now remove existing fileItem for this file
                            if (uploadFile.file_id > 0)
                                sa.RemoveFileItem(uploadFile);
                         
                            //last updated parts
                            List<member> MemberList = sa.Select<member>().Where(m => m.username.ToUpper() == username.ToUpper()).ToList();
                             var loggedInMember = LoggedInUser();
                            if (loggedInMember == null) return new BadRequestObjectResult("Invalid input parameters");
                            uploadFile.last_updated = DateTime.Now;
                            uploadFile.last_updated_by = loggedInMember.member_id;
                            Int32 loggedInUserId = MemberList.First<member>().member_id;

                            uploadFile.name = filename;
                            
                            //are they 'reuploading lost fileItem to existing file or posting new fileItem with new file
                            if (uploadFile.file_id > 0)
                                sa.PutFileItem(uploadFile, memoryStream);
                            else
                                sa.AddFile(uploadFile, memoryStream);
                            sm(sa.Messages);
                        }//end using
                    }//end using

                    return new OperationResult.Created { ResponseResource = uploadFile, Description = this.MessageString };
                }//end using
            }//end try    
            catch (Exception ex)
            { return HandleException(ex); }//end catch
        }//end HttpMethod.POST
        
        
        [HttpOperation(HttpMethod.POST, ForUriName = "RunChopperScript")]
        public OperationResult RunChopperScript(IEnumerable<IMultipartHttpEntity> entities)
        {
            dynamic responseJson = null;           
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    String filename = "";
                    XmlSerializer serializer;
                    file uploadFile = null;

                    foreach (var entity in entities)
                    {
                        //Process Stream
                        if (!entity.Headers.ContentDisposition.Disposition.ToLower().Equals("form-data"))
                            return new OperationResult.BadRequest { ResponseResource = "Sent a field that is not declared as form-data, cannot process" };

                        if (entity.Stream != null && entity.ContentType != null)
                        {
                            //Process Stream
                            if (entity.Headers.ContentDisposition.Name.Equals("File"))
                            {
                                entity.Stream.CopyTo(memoryStream);
                                filename = entity.Headers.ContentDisposition.FileName;
                            }
                        }
                        else
                        {
                            //Process Variables
                            if (entity.Headers.ContentDisposition.Name.Equals("FileEntity"))
                            {
                                var mem = new MemoryStream();
                                entity.Stream.CopyTo(mem);
                                mem.Position = 0;
                                try
                                {
                                    serializer = new XmlSerializer(typeof(file));
                                    uploadFile = (file)serializer.Deserialize(mem);
                                }
                                catch
                                {
                                    mem.Position = 0;
                                    JsonSerializer jsonSerializer = new JsonSerializer();
                                    using (StreamReader streamReader = new StreamReader(mem, new UTF8Encoding(false, true)))
                                    {
                                        using (JsonTextReader jsonTextReader = new JsonTextReader(streamReader))
                                        {
                                            uploadFile = (file)jsonSerializer.Deserialize(jsonTextReader, typeof(file));
                                        }
                                    }//end using
                                } //end catch
                            } //end if FileEntity
                        }//end else
                    }//next

                    //Return BadRequest if missing required fields
                    if (uploadFile.instrument_id <= 0 || uploadFile.site_id <= 0)
                        throw new BadRequestException("Invalid input parameters");

                    STNServiceAgent stnsa = new STNServiceAgent(uploadFile.instrument_id.Value, memoryStream, filename);
                    if (stnsa.chopperInitialized)
                        responseJson = stnsa.RunChopperScript();
                    else
                        throw new BadRequestException("Error initializing python script.");
                } //end using memoryStream
                return new OperationResult.OK { ResponseResource = responseJson };
            }//end try
            catch (Exception ex)
            { return HandleException(ex); }
        }//end HttpMethod.GET
    */
        #endregion

        #region PUT
        [HttpPut("{id}")][Authorize(Policy = "CanModify")]
        public async Task<IActionResult> Put(int id, [FromBody]file entity)
        {
            try
            {
                if (id < 0 || !isValid(entity)) return new BadRequestResult();
                var loggedInMember = LoggedInUser();
                if (loggedInMember == null) return new BadRequestObjectResult("Invalid input parameters");
                entity.last_updated = DateTime.Now;
                entity.last_updated_by = loggedInMember.member_id;
                
                return Ok(await agent.Update<file>(id, entity));
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
                var entity = await agent.Find<file>(id);
                if (entity == null) return new NotFoundResult();

                /*file ObjectToBeDeleted = sa.Select<file>().SingleOrDefault(c => c.file_id == entityId);
                        if (ObjectToBeDeleted == null) throw new WiM.Exceptions.NotFoundRequestException();
                        Int32 datafileID = ObjectToBeDeleted.data_file_id.HasValue ? ObjectToBeDeleted.data_file_id.Value : 0;
                        sa.RemoveFileItem(ObjectToBeDeleted);
                        sa.Delete<file>(ObjectToBeDeleted);

                        if (datafileID > 0)
                        {
                            data_file df = sa.Select<data_file>().FirstOrDefault(d => d.data_file_id == datafileID);
                            sa.Delete<data_file>(df);
                        }*/
                await agent.Delete<file>(entity);
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
