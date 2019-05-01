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
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

namespace STNServices.Controllers
{
    [Route("[controller]")]
    public class SitesController : STNControllerBase
    {
        IConfiguration _config;
        public SitesController(ISTNServicesAgent sa, IConfiguration config) : base(sa)
        {
            _config = config;
        }

        #region METHODS
        
        #region GET
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                //sm(agent.Messages);
                return Ok(agent.Select<sites>());
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
                return Ok(await agent.Find<sites>(id));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpGet("/Events/{eventId}/[controller]")]
        public async Task<IActionResult> GetEventSites(int eventId)
        {
            try
            {
                if (eventId < 0) return new BadRequestResult();

                var objectRequested = agent.Select<sites>().Include(s => s.instruments).Include(s => s.hwms)
                    .Where(s => s.instruments.Any(i => i.event_id == eventId) || s.hwms.Any(h => h.event_id == eventId));

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

        [HttpGet("/NetworkTypes/{networkTypeId}/[controller]")]
        public async Task<IActionResult> getNetworkTypeSites(int networkTypeId)
        {
            try
            {
                if (networkTypeId < 0) return new BadRequestResult();

                var objectRequested = agent.Select<sites>().Include(s => s.network_type_site).Where(s => s.network_type_site.Any(nt => nt.network_type_id == networkTypeId));

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

        [HttpGet("/NetworkNames/{networkNameId}/[controller]")]
        public async Task<IActionResult> getNetworkNameSites(int networkNameId)
        {
            try
            {
                if (networkNameId < 0) return new BadRequestResult();

                var objectRequested = agent.Select<sites>().Include(s => s.network_name_site).Where(s => s.network_name_site.Any(nt => nt.network_name_id == networkNameId));

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

        [HttpGet("/States/{stateAbbrev}/[controller]")]
        public async Task<IActionResult> GetSitesByStateName(string stateAbbrev)
        {
            try
            {
                if (string.IsNullOrEmpty(stateAbbrev)) return new BadRequestResult();

                var objectRequested = agent.Select<sites>().Where(s => s.state.ToUpper() == stateAbbrev.ToUpper());

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

        [HttpGet("/ByDistance")] //?Latitude={latitude}&Longitude={longitude}&Buffer={buffer}
        public async Task<IActionResult> GetSitesByLatLong([FromQuery] double Latitude, [FromQuery] double Longitude, [FromQuery] double Buffer)
        {
            try
            {
                if (Latitude <= 0 || Longitude >= 0) return new BadRequestResult();

                var objectRequested = agent.Select<sites>().Where(s => (s.latitude_dd >= Latitude - Buffer && s.latitude_dd <= Latitude + Buffer) &&
                                                            (s.longitude_dd >= Longitude - Buffer && s.longitude_dd <= Longitude + Buffer)).ToList();

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

        [HttpGet("/Events/{eventId}/PeaklessSites")] 
        public async Task<IActionResult> GetSitesWithoutPeaks(int eventId)
        {
            try
            {
                if (eventId < 0) return new BadRequestResult();

                // all sites with hwms but none of them have a peak for this eventId
                List<sites> hwmSList = agent.Select<sites>().Include(s => s.hwms)
                    .Where(s => s.hwms.Any(h => (h.peak_summary_id <= 0 || !h.peak_summary_id.HasValue) && h.event_id == eventId)).ToList();

                // all sites with sensors that have data files but none of them have a peak for this event IQueryable
                List<sites> dfSList = agent.Select<sites>().Include(s => s.instruments).ThenInclude(i => i.data_files)
                    .Where(s => s.instruments.Any(i => i.data_files.Any(d => (d.peak_summary_id <= 0 || !d.peak_summary_id.HasValue) && i.event_id == eventId))).ToList();

                // join them and remove duplicates
                List<sites> objectsRequested = hwmSList.Union(dfSList).ToList<sites>();

               // if (objectsRequested == null) return new BadRequestObjectResult(new Error(errorEnum.e_notFound));
                //sm(agent.Messages);
                return Ok(objectsRequested);
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpGet("/HorizontalDatums/{hdatumId}/[controller]")]
        public async Task<IActionResult> GetHDatumSites(int hdatumId)
        {
            try
            {
                if (hdatumId < 0) return new BadRequestResult();

                var objectRequested = agent.Select<sites>().Where(s => s.hdatum_id == hdatumId);

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

        [HttpGet("/LandOwners/{landOwnerId}/[controller]")]
        public async Task<IActionResult> GetLandOwnserSites(int landOwnerId)
        {
            try
            {
                if (landOwnerId < 0) return new BadRequestResult();

                var objectRequested = agent.Select<sites>().Where(s => s.landownercontact_id == landOwnerId);

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

        [HttpGet("Search")] //?bySiteNo={siteNo}&bySiteName={siteName}&bySiteId={siteId}
        public async Task<IActionResult> GetSiteBySearch([FromQuery] string bySiteNo = "", [FromQuery] string bySiteName = "", [FromQuery] string bySiteId = "")
        {
            try
            {
                if (string.IsNullOrEmpty(bySiteNo) && string.IsNullOrEmpty(bySiteName) && string.IsNullOrEmpty(bySiteId)) return new BadRequestResult();
                sites objectRequested = null;
                //only one will be provided
                if (!string.IsNullOrEmpty(bySiteNo))
                {
                    objectRequested = agent.Select<sites>().SingleOrDefault(s => s.site_no.ToUpper() == bySiteNo.ToUpper());
                    if (objectRequested == null) return new BadRequestObjectResult(new Error(errorEnum.e_notFound));
                    //sm(sa.Messages);
                }
                if (!string.IsNullOrEmpty(bySiteName))
                {
                    objectRequested = agent.Select<sites>().SingleOrDefault(s => s.site_name.ToUpper() == bySiteName.ToUpper());
                    if (objectRequested == null) return new BadRequestObjectResult(new Error(errorEnum.e_notFound));
                    //sm(sa.Messages);
                }
                if (!string.IsNullOrEmpty(bySiteId))
                {
                    Int32 sid = Convert.ToInt32(bySiteId);
                    objectRequested = agent.Select<sites>().SingleOrDefault(s => s.site_id == sid);
                    if (objectRequested == null) return new BadRequestObjectResult(new Error(errorEnum.e_notFound));
                    //sm(sa.Messages);
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

        [HttpGet("/Files/{fileId}/Site")] 
        public async Task<IActionResult> GetFileSite(int fileId)
        {
            try
            {
                if (fileId < 0) return new BadRequestResult();
                var objectRequested = agent.Select<file>().Include(f=> f.site).FirstOrDefault(f => f.file_id == fileId).site;

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

        [HttpGet("/ObjectivePoints/{objectivePointId}/Site")]
        public async Task<IActionResult> GetOPSite(int objectivePointId)
        {
            try
            {
                if (objectivePointId < 0) return new BadRequestResult();
                var objectRequested = agent.Select<objective_point>().Include(f => f.site).FirstOrDefault(o => o.objective_point_id == objectivePointId).site;

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

        [HttpGet("/HWMs/{hwmId}/Site")]
        public async Task<IActionResult> getHWMSite(int hwmId)
        {
            try
            {
                if (hwmId < 0) return new BadRequestResult();
                var objectRequested = agent.Select<hwm>().Include(f => f.site).FirstOrDefault(h => h.hwm_id == hwmId).site;

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

        [HttpGet("/Instruments/{instrumentId}/Site")]
        public async Task<IActionResult> GetInstrumentSite(int instrumentId)
        {
            try
            {
                if (instrumentId < 0) return new BadRequestResult();
                var objectRequested = agent.Select<instrument>().Include(f => f.site).FirstOrDefault(i => i.instrument_id == instrumentId).site;

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

        [HttpGet("FilteredSites")] //?Event={eventId}&State={stateNames}&SensorType={sensorTypeId}&NetworkName={networkNameId}&OPDefined={opDefined}&HWMOnly={hwmOnlySites}&HWMSurveyed={surveyedHWMs}&SensorOnly={sensorOnlySites}&RDGOnly={rdgOnlySites}
        public async Task<IActionResult> FilteredSites([FromQuery] string Event = "", [FromQuery] string State = "", [FromQuery] string SensorType = "", [FromQuery] string NetworkName = "", [FromQuery] string OPDefined = "", 
            [FromQuery] string HWMOnly = "", [FromQuery] string HWMSurveyed = "", [FromQuery] string SensorOnly = "", [FromQuery] string RDGOnly = "")
        {
            try
            {
                //sm(agent.Messages);
                return Ok(agent.GetFilterSites(Event, State, SensorType, NetworkName, OPDefined, HWMOnly, HWMSurveyed, SensorOnly, RDGOnly));
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }
        
        [HttpGet("{entityId}/GetDataFileScript")]
        public async Task<IActionResult> GetDataFileScript(int entityId)
        {
            try
            {
                string isRunning = "false";
                if (entityId < 0) return new BadRequestResult();
                string path1 = _config["STNRepository"].ToString();
                string path2 = entityId.ToString();
                string combinedPath = System.IO.Path.Combine(path1);

                string[] subdirs = Directory.GetDirectories(combinedPath, path2 + "*");

                if (subdirs.Length > 0)
                    isRunning = "true";
                else
                    isRunning = "false";

                //sm(agent.Messages);
                return Ok(isRunning);
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
        public async Task<IActionResult> Post([FromBody]sites entity)
        {
            try
            {
                if (string.IsNullOrEmpty(entity.site_description) || entity.latitude_dd <= 0 || entity.longitude_dd >= 0 || entity.hdatum_id <= 0 ||
                    entity.hcollect_method_id <= 0 || string.IsNullOrEmpty(entity.state) || string.IsNullOrEmpty(entity.county) ||
                    string.IsNullOrEmpty(entity.waterbody) || (entity.member_id <= 0)) return new BadRequestResult();
                //no duplicate lat/longs allowed
                List<sites> query = agent.Select<sites>().Where(s => s.latitude_dd == entity.latitude_dd && s.longitude_dd == entity.longitude_dd).ToList();
                if (query.Count > 0)
                    return new BadRequestObjectResult("Lat/Long already exists");
                var loggedInMember = LoggedInUser();
                if (loggedInMember == null) return new BadRequestObjectResult("Invalid input parameters");
                entity.last_updated = DateTime.Now;
                entity.last_updated_by = loggedInMember.member_id;
                //var postedSite = new sites();

                sites postedSite = await agent.Add<sites>(entity);

                //postedSite = entity;
                //await agent.Add<sites>(postedSite);

                postedSite.site_no = buildSiteNO(agent, postedSite.state, postedSite.county, Convert.ToInt32(postedSite.site_id), postedSite.site_name);
                //if siteName contains historic name (with dashes) leave it as is..coming from uploader.. else assign the no to the name too
                postedSite.site_name = string.IsNullOrEmpty(postedSite.site_name) ? postedSite.site_no : postedSite.site_name;

                //now hit the update to save this
                return Ok(await agent.Update<sites>(postedSite.site_id, postedSite));
                //sm(agent.Messages);
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }

        
        /*[HttpPost][Authorize(Policy = "AdminOnly")]
        [Route("Batch")]
        public async Task<IActionResult> Batch([FromBody]List<sites> entities)
        {
            try
            {
                if (!isValid(entities)) return new BadRequestObjectResult("Object is invalid");

                //sm(agent.Messages);
                //no duplicate lat/longs allowed
                var loggedInMember = LoggedInUser();
                if (loggedInMember == null) return new BadRequestObjectResult("Invalid input parameters");

                entities.ForEach(site =>
                {
                    List<sites> query = agent.Select<sites>().Where(s => s.latitude_dd == site.latitude_dd && s.longitude_dd == site.longitude_dd).ToList();
                    /*if (query.Count > 0)
                        return new BadRequestObjectResult("Object is invalid");*

                    site.last_updated = DateTime.Now;
                    site.last_updated_by = loggedInMember.member_id;                
                });

                var postedEntities = await agent.Add<sites>(entities);
                List<sites> putEntities = new List<sites>();
                postedEntities.ToList().ForEach(site =>
                {

                    site.site_no = buildSiteNO(agent, site.state, site.county, Convert.ToInt32(site.site_id), site.site_name);
                    //if siteName contains historic name (with dashes) leave it as is..coming from uploader.. else assign the no to the name too
                    site.site_name = string.IsNullOrEmpty(site.site_name) ? site.site_no : site.site_name;

                    //now hit the update to save this
                    sites e = agent.Update<sites>(site.site_id, site);
                    putEntities.Add(e);
                    //sm(agent.Messages);
                    return Ok(putEntities);
                });
            }
            catch (Exception ex)
            {
                //sm(agent.Messages);
                return await HandleExceptionAsync(ex);
            }
        }*/
        #endregion
       
        #region PUT
        [HttpPut("{id}")][Authorize(Policy = "CanModify")]
        public async Task<IActionResult> Put(int id, [FromBody]sites entity)
        {
            try
            {
                if (id < 0 || !isValid(entity)) return new BadRequestResult();
                // no duplicate lat/ longs allowed(just this one)
                List<sites> query = agent.Select<sites>().Where(s => s.latitude_dd == entity.latitude_dd && s.longitude_dd == entity.longitude_dd).ToList();
                if (query.Count > 1)
                    return new BadRequestObjectResult("Lat/Long already exists");
                var updatedSite = agent.Select<sites>().FirstOrDefault(s => s.site_id == entity.site_id);
                if ((!string.Equals(entity.state.ToUpper(), updatedSite.state.ToUpper())) || (!string.Equals(entity.county.ToUpper(), updatedSite.county.ToUpper())))
                    entity.site_no = buildSiteNO(agent, entity.state, entity.county, Convert.ToInt32(entity.site_id), entity.site_name);

                updatedSite.site_no = entity.site_no;
                updatedSite.site_name = entity.site_name;
                updatedSite.site_description = entity.site_description;
                updatedSite.address = entity.address;
                updatedSite.city = entity.city;
                updatedSite.state = entity.state;
                updatedSite.zip = entity.zip;
                updatedSite.other_sid = entity.other_sid;
                updatedSite.county = entity.county;
                updatedSite.waterbody = entity.waterbody;
                updatedSite.latitude_dd = entity.latitude_dd;
                updatedSite.longitude_dd = entity.longitude_dd;
                updatedSite.hdatum_id = entity.hdatum_id;
                updatedSite.drainage_area_sqmi = entity.drainage_area_sqmi;
                updatedSite.landownercontact_id = entity.landownercontact_id;
                updatedSite.priority_id = entity.priority_id;
                updatedSite.zone = entity.zone;
                updatedSite.is_permanent_housing_installed = entity.is_permanent_housing_installed;
                updatedSite.usgs_sid = entity.usgs_sid;
                updatedSite.noaa_sid = entity.noaa_sid;
                updatedSite.hcollect_method_id = entity.hcollect_method_id;
                updatedSite.site_notes = entity.site_notes;
                updatedSite.safety_notes = entity.safety_notes;
                updatedSite.access_granted = entity.access_granted;
                updatedSite.member_id = entity.member_id;
                updatedSite.sensor_not_appropriate = entity.sensor_not_appropriate;

                var loggedInMember = LoggedInUser();
                if (loggedInMember == null) return new BadRequestObjectResult("Invalid input parameters");
                updatedSite.last_updated = DateTime.Now;
                updatedSite.last_updated_by = loggedInMember.member_id;

                return Ok(await agent.Update<sites>(id, entity));
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
                var entity = agent.Select<sites>().Include(s => s.files).Include(s => s.instruments).Include(s => s.hwms).Include(s => s.objective_points)
                            .Include(s => s.network_name_site).Include(s => s.network_type_site).Include(s => s.site_housing)
                            .FirstOrDefault(s => s.site_id == id);
                if (entity == null) return new NotFoundResult();

                if ((entity.objective_points != null && entity.objective_points.Count > 0) || 
                    (entity.files != null && entity.files.Count > 0) || (entity.instruments != null && entity.instruments.Count > 0) ||
                    (entity.hwms != null && entity.hwms.Count > 0))
                {
                    // sm(MessageType.error, "Related Entities");
                    return new BadRequestObjectResult("Please remove attached entities first.");
                }
                else
                {
                    if (entity.site_housing != null) entity.site_housing.Clear();
                    if (entity.network_type_site != null) entity.network_type_site.Clear();
                    if (entity.network_type_site != null) entity.network_type_site.Clear();
                    await agent.Delete<sites>(entity);
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
        private string buildSiteNO(ISTNServicesAgent agent, string state, string county, int siteId, string siteName)
        {
            String siteNo;
            //[a-zA-Z]{2}
            if (!string.IsNullOrEmpty(siteName) && Regex.IsMatch(siteName, @"^[a-zA-Z]{3}[-][a-zA-Z]{2}[-][a-zA-Z]{3}[-][0-9]{3}"))
            {
                string[] substring = siteName.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                siteNo = substring[1] + substring[2] + "00" + Regex.Replace(substring[3], "[^0-9.]", "");

                int incr = 0;
                while (agent.Select<sites>().FirstOrDefault(s => s.site_id == siteId) != null)
                {
                    siteNo += incr;
                    incr++;
                }//end while
            }
            else
            {
                //remove . and space from counties like 'st. lucie'  this only removes from start and end of name
                county = county.Trim(new Char[] { ' ', '.' });
                //if there's a dot, remove it and the space following
                if (county.IndexOf(".") > -1)
                {
                    county = county.Remove(county.IndexOf("."), 2);
                }
                siteNo = state + county.Substring(0, 3).ToUpper() + siteId.ToString("D5");
            }

            return siteNo;
        }

        #endregion
    }
}
