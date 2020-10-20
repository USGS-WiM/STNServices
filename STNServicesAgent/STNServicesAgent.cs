//------------------------------------------------------------------------------
//----- ServiceAgent -------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2017 WiM - USGS

//    authors:  Jeremy K. Newson USGS Web Informatics and Mapping
//              Tonia Roddick  USGS Web Informatics and Mapping
//  
//   purpose:   The service agent is responsible for initiating the service call, 
//              capturing the data that's returned and forwarding the data back to 
//              the requestor.
//
//discussion:   delegated hunting and gathering responsibilities.   
//
// 

using System;
using System.Collections.Generic;
using System.Linq;
using STNDB;
using STNDB.Resources;
using WiM.Utilities;
using Microsoft.EntityFrameworkCore;
using STNAgent.Resources;
using System.Threading.Tasks;
using WiM.Security.Authentication.Basic;
using WiM.Resources;

namespace STNAgent
{
    public interface ISTNServicesAgent : IMessage
    {
        IQueryable<T> Select<T>() where T : class, new();
        Task<T> Find<T>(Int32 pk) where T : class, new();
        Task<T> Add<T>(T item) where T : class, new();        
        Task<IEnumerable<T>> Add<T>(List<T> items) where T : class, new();
        Task<T> Update<T>(Int32 pkId, T item) where T : class, new();
        Task Delete<T>(T item) where T : class, new();
        IQueryable<roles> GetRoles();
        IBasicUser GetUserByUsername(string username);
        IQueryable<data_file> GetFilterDataFiles(string approved, string eventId, string state, string counties);
        IQueryable<T> getTable<T>(object[] args) where T : class, new();
        IQueryable<events> GetFiltedEvents(string date, string eventTypeId, string stateName);
        DateTime? ValidDate(string date);
        List<hwm> GetFilterHWMs(string eventIds, string eventTypeIDs, string eventStatusID, string states, string counties, string hwmTypeIDs, string hwmQualIDs, string hwmEnvironment, string surveyComplete, string stillWater);
        List<instrument> GetFiltedInstruments(string Event, string EventType, string EventStatus, string States, string County, string CurrentStatus, string CollectionCondition, string SensorType, string DeploymentType);
        List<peak_summary> GetFilteredPeaks(string ev, string eventType, string eventStatus, string states, string county, string startDate, string endDate);
    //    FileStream GetFileItem(file anEntity);
        List<sites> GetFilterSites(string @event, string state, string sensorType, string networkName, string oPDefined, string hWMOnly, string hWMSurveyed, string sensorOnly, string rDGOnly);
        List<ReportResource> GetFiltedReportsModel(int ev, string state, string date);
        List<reporting_metrics> GetFiltedReports(string ev, string date, string states);
        List<STNDB.Resources.Isensor> GetSensorView(string ViewType, string Event, string EventType, string EventStatus, string States, string County, string CurrentStatus, string CollectionCondition, string SensorType, string DeploymentType);

        //    InMemoryFile GetFileItem(file anEntity);
        //    InMemoryFile GetHWMSpreadsheetItem();

    }

    public class STNServicesAgent : DBAgentBase, ISTNServicesAgent, IBasicUserAgent
    {
        //https://aws.amazon.com/blogs/developer/configuring-aws-sdk-with-net-core/
        Amazon.Runtime.IAmazonService aBucket { get; set; }
        //private Dictionary<string, string> Resources;

        //needed for peakDownloadable populating of related site
        private sites globalPeakSite = new sites();

        public STNServicesAgent(STNDBContext context):base(context){//, Dictionary<string, string> files) : base(context) {

            //optimize query for disconnected databases.
            this.context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            //this.Resources = files;
        }

        #region Filtered Returns
        public IQueryable<data_file> GetFilterDataFiles(string approved, string eventId, string state, string counties)
        {
            try
            {
                //set defaults
                char[] countydelimiterChars = { ';', ',' };
                bool isApprovedStatus = false;
                Boolean.TryParse(approved, out isApprovedStatus);
                string filterState = state;
                List<String> countyList = !string.IsNullOrEmpty(counties) ? counties.ToUpper().Split(countydelimiterChars, StringSplitOptions.RemoveEmptyEntries).ToList() : null;
                Int32 filterEvent = (!string.IsNullOrEmpty(eventId)) ? Convert.ToInt32(eventId) : -1;

                IQueryable<data_file> query;
                if (isApprovedStatus)
                    query = this.Select<data_file>().Include(d => d.instrument).ThenInclude(i => i.site).Where(d => d.approval_id > 0);
                else
                    query = this.Select<data_file>().Include(d => d.instrument).Where(d => d.approval_id <= 0 || !d.approval_id.HasValue);

                if (filterEvent > 0)
                    query = query.Where(d => d.instrument.event_id.Value == filterEvent);

                if (filterState != "")
                    query = query.Where(d => d.instrument.site.state == filterState);

                if (countyList != null && countyList.Count > 0)
                    query = query.Where(d => countyList.Contains(d.instrument.site.county.ToUpper()));

                sm(this.Messages);
                return query.Select(d => new data_file()
                {
                    approval_id = d.approval_id,
                    collect_date = d.collect_date,
                    data_file_id = d.data_file_id,
                    elevation_status = d.elevation_status,
                    end = d.end,
                    good_end = d.good_end,
                    good_start = d.good_start,
                    instrument_id = d.instrument_id,
                    peak_summary_id = d.peak_summary_id,
                    processor_id = d.processor_id,
                    start = d.start,
                    time_zone = d.time_zone
                });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IQueryable<events> GetFiltedEvents(string date, string eventTypeId, string stateName)
        {
            IQueryable<events> query;
            List<events> entities = new List<events>();
            DateTime? fromDate;
            try
            {
                fromDate = ValidDate(date);

                //get all events
                query = this.Select<events>().Include(d => d.instruments).ThenInclude(i => i.site).Include(d => d.hwms).ThenInclude(h => h.site);

                //events from this day forward
                if (fromDate.HasValue)
                    query = query.Where(s => s.event_start_date >= fromDate);

                //for this eventType only
                if (!string.IsNullOrEmpty(eventTypeId) && Convert.ToInt32(eventTypeId) > 0)
                {
                    Int32 eventTypdID = Convert.ToInt32(eventTypeId);
                    query = query.Where(s => s.event_type_id == eventTypdID);
                }

                //in this state only
                if (!string.IsNullOrEmpty(stateName))
                {
                    query = query.Where(e => e.instruments.Any(i => i.site.state == stateName.ToUpper()) || e.hwms.Any(h => h.site.state == stateName.ToUpper()));
                }
                return query.Distinct();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<hwm> GetFilterHWMs(string eventIds, string eventTypeIDs, string eventStatusID, string states, string counties, string hwmTypeIDs, string hwmQualIDs, string hwmEnvironment, string surveyComplete, string stillWater)
        {
            List<hwm> entities = null;
            try
            {
                char[] delimiterChars = { ';', ',', ' ' }; char[] countydelimiterChars = { ';', ',' };
                //parse the requests
                List<decimal> eventIdList = !string.IsNullOrEmpty(eventIds) ? eventIds.ToUpper().Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries).Select(decimal.Parse).ToList() : null;
                List<decimal> eventTypeList = !string.IsNullOrEmpty(eventTypeIDs) ? eventTypeIDs.ToUpper().Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries).Select(decimal.Parse).ToList() : null;
                List<string> stateList = !string.IsNullOrEmpty(states) ? states.ToUpper().Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries).ToList() : null;
                List<String> countyList = !string.IsNullOrEmpty(counties) ? counties.ToUpper().Split(countydelimiterChars, StringSplitOptions.RemoveEmptyEntries).ToList() : null;

                List<decimal> hwmTypeIdList = !string.IsNullOrEmpty(hwmTypeIDs) ? hwmTypeIDs.ToUpper().Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries).Select(decimal.Parse).ToList() : null;
                List<decimal> hwmQualIdList = !string.IsNullOrEmpty(hwmQualIDs) ? hwmQualIDs.ToUpper().Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries).Select(decimal.Parse).ToList() : null;


                IQueryable<hwm> query;
                query = this.Select<hwm>().Include(h => h.hwm_types).Include(h => h.@event).Include(h => h.hwm_qualities).Include(h => h.vertical_datums).Include(h => h.horizontal_datums).Include(h => h.survey_member)
                    .Include(h => h.flag_member).Include(h => h.vertical_collect_methods).Include(h => h.horizontal_collect_methods).Include(h => h.approval).ThenInclude(a=>a.members).Include(h => h.markers)
                    .Include(h => h.site).ThenInclude(s=>s.deployment_priority).Include("site.network_name_site.network_name").Where(s => s.hwm_id > 0);

                if (eventIdList != null && eventIdList.Count > 0)
                    query = query.Where(i => i.event_id.HasValue && eventIdList.Contains(i.event_id.Value));

                if (eventTypeList != null && eventTypeList.Count > 0)
                    query = query.Where(i => i.@event.event_type_id.HasValue && eventTypeList.Contains(i.@event.event_type_id.Value));

                if (!string.IsNullOrEmpty(eventStatusID))
                {
                    if (Convert.ToInt32(eventStatusID) > 0)
                    {
                        Int32 eveStatId = Convert.ToInt32(eventStatusID);
                        query = query.Where(i => i.@event.event_status_id.HasValue && i.@event.event_status_id.Value == eveStatId);
                    }
                }
                if (stateList != null && stateList.Count > 0)
                    query = query.Where(i => stateList.Contains(i.site.state));

                if (countyList != null && countyList.Count > 0)
                    query = query.Where(i => countyList.Contains(i.site.county.ToUpper()));

                if (hwmTypeIdList != null && hwmTypeIdList.Count > 0)
                    query = query.Where(i => hwmTypeIdList.Contains(i.hwm_type_id));

                if (hwmQualIdList != null && hwmQualIdList.Count > 0)
                    query = query.Where(i => hwmQualIdList.Contains(i.hwm_quality_id));

                if (!string.IsNullOrEmpty(hwmEnvironment))
                    query = query.Where(i => i.hwm_environment == hwmEnvironment);

                if (!string.IsNullOrEmpty(surveyComplete))
                {
                    if (surveyComplete == "True")
                        query = query.Where(i => i.survey_date.HasValue);
                    else
                        query = query.Where(i => !i.survey_date.HasValue);
                }

                //1 = yes, 0 = no
                if (!string.IsNullOrEmpty(stillWater))
                    if (stillWater == "True")
                        query = query.Where(i => i.stillwater.HasValue && i.stillwater.Value == 1);
                    else
                        query = query.Where(i => !i.stillwater.HasValue || i.stillwater.Value == 0);

                entities = query.AsEnumerable().Select(
                    hw => new HWMDownloadable
                    {
                        latitude = hw.latitude_dd.Value,
                        longitude = hw.longitude_dd.Value,
                        hwm_id = hw.hwm_id,
                        waterbody = hw.waterbody,
                        site_id = hw.site_id,
                        event_id = hw.event_id,
                        eventName = hw.@event != null ? hw.@event.event_name : "",
                        hwm_type_id = hw.hwm_type_id,
                        hwm_label = hw.hwm_label,
                        hwmTypeName = hw.hwm_type_id > 0 && hw.hwm_types != null ? hw.hwm_types.hwm_type : "",
                        hwm_quality_id = hw.hwm_quality_id,
                        hwmQualityName = hw.hwm_quality_id > 0 && hw.hwm_qualities != null ? hw.hwm_qualities.hwm_quality : "",
                        hwm_locationdescription = hw.hwm_locationdescription,
                        latitude_dd = hw.latitude_dd,
                        longitude_dd = hw.longitude_dd,
                        elev_ft = hw.elev_ft,
                        uncertainty = hw.uncertainty,
                        vdatum_id = hw.vdatum_id,
                        verticalDatumName = hw.vdatum_id > 0 && hw.vertical_datums != null ? hw.vertical_datums.datum_name : "",
                        hdatum_id = hw.hdatum_id,
                        horizontalDatumName = hw.hdatum_id > 0 && hw.horizontal_datums != null ? hw.horizontal_datums.datum_name : "",
                        vcollect_method_id = hw.vcollect_method_id,
                        verticalMethodName = hw.vcollect_method_id > 0 && hw.vertical_collect_methods != null ? hw.vertical_collect_methods.vcollect_method : "",
                        hcollect_method_id = hw.hcollect_method_id,
                        horizontalMethodName = hw.hcollect_method_id > 0 && hw.horizontal_collect_methods != null ? hw.horizontal_collect_methods.hcollect_method : "",
                        bank = hw.bank,
                        hwm_uncertainty = hw.hwm_uncertainty,
                        approval_id = hw.approval_id,
                        approvalMember = hw.approval_id > 0 ? getApprovalName(hw) : "",
                        marker_id = hw.marker_id,
                        markerName = hw.marker_id > 0 && hw.markers != null ? hw.markers.marker : "",
                        height_above_gnd = hw.height_above_gnd,
                        hwm_notes = hw.hwm_notes,
                        hwm_environment = hw.hwm_environment,
                        flag_date = hw.flag_date,
                        survey_date = hw.survey_date,
                        stillwater = hw.stillwater,
                        flag_member_id = hw.flag_member_id,
                        flagMemberName = hw.flag_member_id > 0 && hw.flag_member != null ? hw.flag_member.fname + " " + hw.flag_member.lname : "",
                        survey_member_id = hw.survey_member_id,
                        surveyMemberName = hw.survey_member_id > 0 && hw.survey_member != null ? hw.survey_member.fname + " " + hw.survey_member.lname : "",
                        peak_summary_id = hw.peak_summary_id,
                        site_no = hw.site != null ? hw.site.site_no : "",
                        site_latitude = hw.site != null ? hw.site.latitude_dd : 0,
                        site_longitude = hw.site != null ? hw.site.longitude_dd : 0,
                        siteDescription = hw.site != null ? hw.site.site_description : "",
                        networkNames = hw.site != null && hw.site.network_name_site.Count > 0 ? (hw.site.network_name_site.Where(ns => ns.site_id == hw.site.site_id).ToList()).Select(x => x.network_name.name).Distinct().Aggregate((x, j) => x + ", " + j) : "",
                        stateName = hw.site != null ? hw.site.state : "",
                        countyName = hw.site != null ? hw.site.county : "",
                        sitePriorityName = hw.site != null && hw.site.priority_id > 0 && hw.site.deployment_priority != null ? hw.site.deployment_priority.priority_name : "",
                        siteZone = hw.site != null ? hw.site.zone : "",
                        sitePermHousing = hw.site != null && hw.site.is_permanent_housing_installed == null || hw.site.is_permanent_housing_installed == "No" ? "No" : "Yes"
                    }).ToList<hwm>();

                return entities;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<sites> GetFilterSites(string eventId, string stateNames, string sensorTypeId, string networkNameId, string opDefined, string hwmOnlySites, string surveyedHWMs, string sensorOnlySites, string rdgOnlySites)
        {           
            try
            {
                List<sites> sites = null;

                List<string> states = new List<string>();
                char[] delimiter = { ',' };
                //stateNames will be a list that is comma separated. Need to parse out
                if (!string.IsNullOrEmpty(stateNames))
                {
                    states = stateNames.Split(delimiter, StringSplitOptions.RemoveEmptyEntries).ToList();
                }

                Int32 filterEvent = (!string.IsNullOrEmpty(eventId)) ? Convert.ToInt32(eventId) : -1;
                Int32 filterSensorType = (!string.IsNullOrEmpty(sensorTypeId)) ? Convert.ToInt32(sensorTypeId) : -1;
                Int32 filternetworkname = (!string.IsNullOrEmpty(networkNameId)) ? Convert.ToInt32(networkNameId) : -1;
                Boolean OPhasBeenDefined = (!string.IsNullOrEmpty(opDefined) && Convert.ToInt32(opDefined) > 0) ? true : false;
                Boolean hwmOnly = (!string.IsNullOrEmpty(hwmOnlySites) && Convert.ToInt32(hwmOnlySites) > 0) ? true : false;
                Boolean sensorOnly = (!string.IsNullOrEmpty(sensorOnlySites) && Convert.ToInt32(sensorOnlySites) > 0) ? true : false;
                Boolean rdgOnly = (!string.IsNullOrEmpty(rdgOnlySites) && Convert.ToInt32(rdgOnlySites) > 0) ? true : false;

                IQueryable<sites> query;
                query = this.Select<sites>()
                    .Include(s => s.instruments).ThenInclude(i => i.@event)
                    .Include(s=> s.hwms).ThenInclude(h=>h.@event)
                    .Include(s=> s.objective_points)
                    .Include(s => s.network_name_site).ThenInclude(nn=>nn.network_name)
                    .Include(s => s.site_housing);

                if (filterEvent > 0)
                    query = query.Where(s => s.instruments.Any(i => i.event_id == filterEvent) || s.hwms.Any(h => h.event_id == filterEvent));

                if (states.Count >= 2)
                {
                    //multiple STATES
                    query = from q in query where states.Any(s => q.state.Contains(s.Trim())) select q;
                }
                if (states.Count == 1)
                {
                    string thisState = states[0];                    
                    query = query.Where(r => r.state.Equals(thisState, StringComparison.OrdinalIgnoreCase));
                }
                if (OPhasBeenDefined)
                    query = query.Where(s => s.objective_points.Any());

                if (filterSensorType > 0)
                    query = query.Where(s => s.instruments.Any(i => i != null && i.sensor_type_id == filterSensorType));

                if (filternetworkname > 0)
                    query = query.Where(s => s.network_name_site.Any(i => i.network_name_id == filternetworkname));
                
                if (hwmOnly)
                {
                    //Site with no sensor deployments AND Site with no proposed sensors AND 
                    //(Site does not have permanent housing installed OR Site has had HWM collected at it in the past OR Site has “Sensor not appropriate” box checked)
                    query = query.Where(s => !s.instruments.Any() && ((s.is_permanent_housing_installed == "No" || s.is_permanent_housing_installed == null) || s.hwms.Any() || s.sensor_not_appropriate == 1));
                }
                if (!string.IsNullOrEmpty(surveyedHWMs))
                {
                    if (surveyedHWMs == "true")
                        query = query.Where(i => i.hwms.Any() && i.hwms.Any(h => h.survey_date.HasValue));
                    else
                        query = query.Where(i => i.hwms.Any() && i.hwms.Any(h => !h.survey_date.HasValue));
                }

                if (sensorOnly)
                {
                    //Site with previous sensor deployment OR Sites with housing types 1-4 indicated //OR Sites with permanent housing installed 
                    //OR Site with proposed sensor indicated of any type
                    query = query.Where(s => !s.hwms.Any() && (s.is_permanent_housing_installed == "Yes" || s.site_housing.Any(h => h.housing_type_id > 0 && h.housing_type_id < 5) || s.instruments.Any()));
                }

                if (rdgOnly)
                {
                    //Site with previous RDG sensor type deployed OR Site with RDG housing type listed (type 5) OR Site with RDG checked as a proposed sensor
                    query = query.Where(s => s.instruments.Any(inst => inst.sensor_type_id == 5) || s.site_housing.Any(h => h.housing_type_id == 5));
                }

                sites = query.AsEnumerable().Select(
                    s => new SiteLocationQuery
                    {
                        site_id = s.site_id,
                        site_no = s.site_no,
                        site_name = s.site_name,
                        site_description = s.site_description,
                        address = s.address,
                        city = s.city,
                        state = s.state,
                        zip = s.zip,
                        county = s.county,
                        waterbody = s.waterbody,
                        latitude_dd = s.latitude_dd,
                        longitude_dd = s.longitude_dd,
                        hdatum_id = s.hdatum_id,
                        drainage_area_sqmi = s.drainage_area_sqmi,
                        landownercontact_id = s.landownercontact_id,
                        priority_id = s.priority_id,
                        zone = s.zone,
                        is_permanent_housing_installed = s.is_permanent_housing_installed,
                        usgs_sid = s.usgs_sid,
                        other_sid = s.other_sid,
                        noaa_sid = s.noaa_sid,
                        hcollect_method_id = s.hcollect_method_id,
                        // site_notes = s.site_notes,  should this be removed?? (Internal)
                        safety_notes = s.safety_notes,
                        access_granted = s.access_granted,
                        member_id = s.member_id,
                        networkNames = s.network_name_site.Count > 0 ? s.network_name_site.Where(ns => ns.site_id == s.site_id).Select(x => x.network_name.name).Distinct().ToList() : new List<string>(),
                        RecentOP = getRecentOP(s),
                        Events = getSiteEvents(s)
                    }).ToList<sites>();

                return sites;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        
        public List<instrument> GetFiltedInstruments(string eventIds, string eventTypeIDs, string eventStatusID, string states, string counties, string statusIDs, string collectionConditionIDs, string sensorTypeIDs, string deploymentTypeIDs)
        {
            List<instrument> entities = null;
            try
            {
                char[] delimiterChars = { ';', ',', ' ' }; char[] countydelimiterChars = { ';', ',' };
                //parse the requests
                List<decimal> eventIdList = !string.IsNullOrEmpty(eventIds) ? eventIds.ToUpper().Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries).Select(decimal.Parse).ToList() : null;
                List<decimal> eventTypeList = !string.IsNullOrEmpty(eventTypeIDs) ? eventTypeIDs.ToUpper().Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries).Select(decimal.Parse).ToList() : null;
                List<string> stateList = !string.IsNullOrEmpty(states) ? states.ToUpper().Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries).ToList() : null;
                List<String> countyList = !string.IsNullOrEmpty(counties) ? counties.ToUpper().Split(countydelimiterChars, StringSplitOptions.RemoveEmptyEntries).ToList() : null;
                List<decimal> statusIdList = !string.IsNullOrEmpty(statusIDs) ? statusIDs.ToUpper().Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries).Select(decimal.Parse).ToList() : null;
                List<decimal> sensorTypeIdList = !string.IsNullOrEmpty(sensorTypeIDs) ? sensorTypeIDs.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries).Select(decimal.Parse).ToList() : null;
                List<decimal> collectionConditionIdList = !string.IsNullOrEmpty(collectionConditionIDs) ? collectionConditionIDs.ToUpper().Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries).Select(decimal.Parse).ToList() : null;
                List<decimal> deploymentTypeIdList = !string.IsNullOrEmpty(deploymentTypeIDs) ? deploymentTypeIDs.ToUpper().Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries).Select(decimal.Parse).ToList() : null;
                
                IQueryable<instrument> query;
                query = this.Select<instrument>().Include(i => i.instrument_status)
                    .Include(i => i.@event).Include(i => i.sensor_brand).Include(i => i.sensor_type).Include(i => i.instr_collection_conditions).Include(i => i.housing_type).Include(i => i.deployment_type)
                    .Include(i => i.site).ThenInclude(s=> s.deployment_priority)
                    .Include(i => i.site).ThenInclude(s => s.horizontal_datums)
                    .Include(i => i.site).ThenInclude(s => s.horizontal_collect_methods)
                    .Include(i => i.site).ThenInclude(s => s.network_name_site).ThenInclude(nns=> nns.network_name)                    
                    .Where(s => s.instrument_id > 0);

                if (eventIdList != null && eventIdList.Count > 0)
                    query = query.Where(i => i.event_id.HasValue && eventIdList.Contains(i.event_id.Value));

                if (eventTypeList != null && eventTypeList.Count > 0)
                    query = query.Where(i => i.@event.event_type_id.HasValue && eventTypeList.Contains(i.@event.event_type_id.Value));

                if (!string.IsNullOrEmpty(eventStatusID))
                {
                    if (Convert.ToInt32(eventStatusID) > 0)
                    {
                        Int32 evStatID = Convert.ToInt32(eventStatusID);
                        query = query.Where(i => i.@event.event_status_id.HasValue && i.@event.event_status_id.Value == evStatID);
                    }
                }
                if (stateList != null && stateList.Count > 0)
                    query = query.Where(i => stateList.Contains(i.site.state));

                if (countyList != null && countyList.Count > 0)
                    query = query.Where(i => countyList.Contains(i.site.county));

                if (collectionConditionIdList != null && collectionConditionIdList.Count > 0)
                    query = query.Where(i => i.inst_collection_id.HasValue && collectionConditionIdList.Contains(i.inst_collection_id.Value));

                if (deploymentTypeIdList != null && deploymentTypeIdList.Count > 0)
                    query = query.Where(i => i.deployment_type_id.HasValue && deploymentTypeIdList.Contains(i.deployment_type_id.Value));

                if (sensorTypeIdList != null && sensorTypeIdList.Count > 0)
                    query = query.Where(i => i.sensor_type_id.HasValue && sensorTypeIdList.Contains(i.sensor_type_id.Value));

                if (statusIdList != null && statusIdList.Count > 0)
                    query = query.AsEnumerable().Where(i => (i.instrument_status == null || i.instrument_status.Count <= 0) ? false :
                                                                statusIdList.Contains(i.instrument_status.OrderByDescending(insStat => insStat.time_stamp)
                                                                .Where(stat => stat != null).FirstOrDefault().status_type_id.Value)).AsQueryable();
                entities = query.AsEnumerable().Select(
                        inst => new InstrumentDownloadable
                        {
                            instrument_id = inst.instrument_id,
                            sensorType = inst.sensor_type.sensor,
                            sensor_type_id = inst.sensor_type_id,
                            deploymentType = inst.deployment_type != null ? inst.deployment_type.method : "",
                            deployment_type_id = inst.deployment_type_id,
                            serial_number = inst.serial_number,
                            housing_serial_number = inst.housing_serial_number,
                            interval = inst.interval,
                            site_id = inst.site_id,
                            eventName = inst.@event != null ? inst.@event.event_name : "",
                            location_description = inst.location_description,
                            collectionCondition = inst.instr_collection_conditions != null ? inst.instr_collection_conditions.condition : "",
                            housingType = inst.housing_type != null ? inst.housing_type.type_name : "",
                            vented = inst.vented,
                            sensorBrand = inst.sensor_brand != null ? inst.sensor_brand.brand_name : "",
                            statusId = inst.instrument_status != null ? inst.instrument_status.OrderByDescending(y => y.time_stamp).FirstOrDefault().status_type_id : null,
                            timeStamp = inst.instrument_status != null ? inst.instrument_status.OrderByDescending(y => y.time_stamp).FirstOrDefault().time_stamp : null,
                            site_no = inst.site.site_no,
                            latitude = inst.site.latitude_dd,
                            longitude = inst.site.longitude_dd,
                            siteDescription = inst.site.site_description,
                            networkNames = inst.site.network_name_site.Count > 0 ? (inst.site.network_name_site.Where(ns => ns.site_id == inst.site.site_id).ToList()).Select(x => x.network_name.name).Distinct().Aggregate((x, j) => x + ", " + j) : "",
                            stateName = inst.site.state,
                            countyName = inst.site.county,
                            siteWaterbody = inst.site.waterbody,
                            siteHDatum = inst.site.horizontal_datums != null ? inst.site.horizontal_datums.datum_name : "",
                            sitePriorityName = inst.site.deployment_priority != null ? inst.site.deployment_priority.priority_name : "",
                            siteZone = inst.site.zone,
                            siteHCollectMethod = inst.site.horizontal_collect_methods != null ? inst.site.horizontal_collect_methods.hcollect_method : "",
                            sitePermHousing = inst.site.is_permanent_housing_installed == null || inst.site.is_permanent_housing_installed == "No" ? "No" : "Yes"
                        }).ToList<instrument>();

                return entities;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<peak_summary> GetFilteredPeaks(string eventIds, string eventTypeIDs, string eventStatusID, string states, string counties, string startDate, string endDate)
        {
            List<peak_summary> entities = null;
            try
            {
                char[] delimiterChars = { ';', ',', ' ' };
                char[] countydelimiterChars = { ';', ',' };
                //parse the requests
                List<decimal> eventIdList = !string.IsNullOrEmpty(eventIds) ? eventIds.ToUpper().Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries).Select(decimal.Parse).ToList() : null;
                List<decimal> eventTypeList = !string.IsNullOrEmpty(eventTypeIDs) ? eventTypeIDs.ToUpper().Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries).Select(decimal.Parse).ToList() : null;
                List<string> stateList = !string.IsNullOrEmpty(states) ? states.ToUpper().Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries).ToList() : null;
                List<string> countyList = !string.IsNullOrEmpty(counties) ? counties.Split(countydelimiterChars, StringSplitOptions.RemoveEmptyEntries).ToList() : null;

                DateTime? FromDate = ValidDate(startDate);
                DateTime? ToDate = ValidDate(endDate);
                if (!ToDate.HasValue) ToDate = DateTime.Now;

                IQueryable<peak_summary> query;
                query = this.Select<peak_summary>().Include(p => p.hwms).ThenInclude(h=> h.site).ThenInclude(s=> s.network_name_site).ThenInclude(nns=> nns.network_name)
                    .Include(p=> p.data_file).ThenInclude(d => d.instrument).ThenInclude(i=> i.site).ThenInclude(s => s.network_name_site).ThenInclude(nns => nns.network_name)
                    .Include(p => p.vertical_datums).Include(p => p.members).Where(s => s.peak_summary_id > 0);

                if (eventIdList != null && eventIdList.Count > 0)
                    query = query.Where(ps => (ps.hwms.Any(hwm => eventIdList.Contains(hwm.event_id.Value)) || (ps.data_file.Any(d => eventIdList.Contains(d.instrument.event_id.Value)))));

                if (eventTypeList != null && eventTypeList.Count > 0)
                    query = query.Where(ps => (ps.hwms.Any(hwm => eventTypeList.Contains(hwm.@event.event_type_id.Value)) || (ps.data_file.Any(d => eventTypeList.Contains(d.instrument.@event.event_type_id.Value)))));

                if (!string.IsNullOrEmpty(eventStatusID))
                {
                    if (Convert.ToInt32(eventStatusID) > 0)
                    {
                        Int32 evStatID = Convert.ToInt32(eventStatusID);
                        query = query.Where(ps => (ps.hwms.Any(hwm => hwm.@event.event_status_id.Value == evStatID)) || (ps.data_file.Any(d => d.instrument.@event.event_status_id.Value == evStatID)));
                    }
                }

                if (stateList != null && stateList.Count > 0)
                    query = query.Where(ps => (ps.hwms.Any(hwm => stateList.Contains(hwm.site.state)) || (ps.data_file.Any(d => stateList.Contains(d.instrument.site.state)))));

                if (countyList != null && countyList.Count > 0)
                    query = query.Where(ps => (ps.hwms.Any(hwm => countyList.Contains(hwm.site.county)) || (ps.data_file.Any(d => countyList.Contains(d.instrument.site.county)))));

                if (FromDate.HasValue)
                    query = query.Where(ps => ps.peak_date >= FromDate);

                if (ToDate.HasValue)
                    query = query.Where(ps => ps.peak_date.Value <= ToDate.Value);

                entities = query.AsEnumerable().Select(
                    p => new PeakResource
                    {
                        peak_summary_id = p.peak_summary_id,
                        member_name = p.member_id.HasValue ? p.members.fname + " " + p.members.lname : "",
                        peak_date = p.peak_date,
                        is_peak_estimated = p.is_peak_estimated,
                        time_zone = p.time_zone,
                        is_peak_time_estimated = p.is_peak_time_estimated,
                        peak_stage = p.peak_stage,
                        is_peak_stage_estimated = p.is_peak_stage_estimated,
                        peak_discharge = p.peak_discharge,
                        is_peak_discharge_estimated = p.is_peak_discharge_estimated,
                        height_above_gnd = p.height_above_gnd,
                        is_hag_estimated = p.is_hag_estimated,
                        aep = p.aep,
                        aep_lowci = p.aep_lowci,
                        aep_upperci = p.aep_upperci,
                        aep_range = p.aep_range,
                        calc_notes = p.calc_notes,
                        vdatum = p.vertical_datums != null ? p.vertical_datums.datum_name : "",
                        site_id = GetThisPeaksSiteID(p),
                        site_no = globalPeakSite.site_no,
                        latitude_dd = globalPeakSite.latitude_dd,
                        longitude_dd = globalPeakSite.longitude_dd,
                        description = globalPeakSite.site_description,
                        networks = globalPeakSite.network_name_site.Count > 0 ? globalPeakSite.network_name_site.Select(x => x.network_name.name).Distinct().Aggregate((x, j) => x + ", " + j) : "",
                        state = globalPeakSite.state,
                        county = globalPeakSite.county,
                        waterbody = globalPeakSite.waterbody,
                        horizontal_datum = globalPeakSite.horizontal_datums != null ? globalPeakSite.horizontal_datums.datum_name : "",
                        priority = globalPeakSite.deployment_priority != null ? globalPeakSite.deployment_priority.priority_name : "",
                        zone = globalPeakSite.zone,
                        horizontal_collection_method = globalPeakSite.horizontal_collect_methods != null ? globalPeakSite.horizontal_collect_methods.hcollect_method : "",
                        perm_housing_installed = globalPeakSite.is_permanent_housing_installed == null || globalPeakSite.is_permanent_housing_installed == "No" ? "No" : "Yes"
                    }).ToList<peak_summary>();

                return entities;
                //sm(MessageType.info, "Count: " + entities.Count());
                //sm(sa.Messages);

            }
            catch (Exception ex)
            {
                throw;
            }
        }
               
        public List<STNDB.Resources.Isensor> GetSensorView(string ViewType, string Event, string EventType, string EventStatus, string States, string County, string CurrentStatus, string CollectionCondition, string SensorType, string DeploymentType)
        {
            try
            {
                List<STNDB.Resources.Isensor> aViewTable = null;
                IQueryable<STNDB.Resources.Isensor> query;

                //query = this.getTable<STNDB.Resources.sensor_view>(new Object[1] { ViewType.ToString() });
                //'baro_view', 'met_view', 'rdg_view', 'stormTide_view' or 'waveheight_view'");
                switch (ViewType)
                {
                    case "baro_view":
                        query = this.Select<STNDB.Resources.sensor>().Where(s => s.sensor_type_id == 1 && s.deployment_type_id == 3);
                        break;
                    case "met_view":
                        query = this.Select<STNDB.Resources.sensor>().Where(s => s.sensor_type_id == 2);
                        break;
                    case "rdg_view":
                        query = this.Select<STNDB.Resources.sensor>().Where(s => s.sensor_type_id == 5);
                        break;
                    case "stormTide_view":
                        query = this.Select<STNDB.Resources.sensor>().Where(s => s.sensor_type_id == 1 && (s.deployment_type_id == 1 || s.deployment_type_id == 2));
                        break;
                    case "waveheight_view":
                        query = this.Select<STNDB.Resources.sensor>().Where(s => s.sensor_type_id == 1 && s.deployment_type_id == 2);
                        break;
                    default:
                        //sensor_base_view maps to default
                        query = this.Select<STNDB.Resources.sensor_event>();
                        break;
                }
                //query = this.Select<STNDB.Resources.sensor_event>();

                char[] delimiterChars = { ';', ',', ' ' }; char[] countydelimiterChars = { ';', ',' };
                //parse the requests
                List<string> eventValueList = !string.IsNullOrEmpty(Event) ? Event.ToUpper().Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries).ToList() : null;
                List<decimal> eventTypeList = !string.IsNullOrEmpty(EventType) ? EventType.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries).Select(decimal.Parse).ToList() : null;
                List<string> stateList = !string.IsNullOrEmpty(States) ? States.ToUpper().Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries).ToList() : null;
                List<String> countyList = !string.IsNullOrEmpty(County) ? County.ToUpper().Split(countydelimiterChars, StringSplitOptions.RemoveEmptyEntries).ToList() : null;
                List<decimal> sensorTypeIdList = !string.IsNullOrEmpty(SensorType) ? SensorType.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries).Select(decimal.Parse).ToList() : null;
                List<decimal> statusIdList = !string.IsNullOrEmpty(CurrentStatus) ? CurrentStatus.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries).Select(decimal.Parse).ToList() : null;
                List<decimal> collectionConditionIdList = !string.IsNullOrEmpty(CollectionCondition) ? CollectionCondition.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries).Select(decimal.Parse).ToList() : null;
                List<decimal> deploymentTypeIdList = !string.IsNullOrEmpty(DeploymentType) ? DeploymentType.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries).Select(decimal.Parse).ToList() : null;

                if (eventValueList != null && eventValueList.Count > 0)
                    query = query.Where(e => eventValueList.Contains(e.event_name.Trim().Replace(" ", "").ToUpper()) || eventValueList.Contains(e.event_id.ToString()));

                if (eventTypeList != null && eventTypeList.Count > 0)
                    query = query.Where(i => i.event_type_id.HasValue && eventTypeList.Contains(i.event_type_id.Value));

                if (!string.IsNullOrEmpty(EventStatus))
                {
                    if (Convert.ToInt32(EventStatus) > 0)
                    {
                        Int32 evStatID = Convert.ToInt32(EventStatus);
                        query = query.Where(i => i.event_status_id.HasValue && i.event_status_id.Value == evStatID);
                    }
                }

                if (stateList != null && stateList.Count > 0)
                    query = query.Where(i => stateList.Contains(i.state));

                if (countyList != null && countyList.Count > 0)
                    query = query.Where(i => countyList.Contains(i.county));

                if (collectionConditionIdList != null && collectionConditionIdList.Count > 0)
                    query = query.Where(i => i.inst_collection_id.HasValue && collectionConditionIdList.Contains(i.inst_collection_id.Value));

                if (deploymentTypeIdList != null && deploymentTypeIdList.Count > 0)
                    query = query.Where(i => i.deployment_type_id.HasValue && deploymentTypeIdList.Contains(i.deployment_type_id.Value));

                if (sensorTypeIdList != null && sensorTypeIdList.Count > 0)
                    query = query.Where(i => i.sensor_type_id.HasValue && sensorTypeIdList.Contains(i.sensor_type_id.Value));

                if (statusIdList != null && statusIdList.Count > 0)
                    query = query.Where(i => i.status_type_id.HasValue && statusIdList.Contains(i.status_type_id.Value));

                aViewTable = query.ToList();
                //sm(agent.Messages);
                return aViewTable;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<ReportResource> GetFiltedReportsModel(int eventId, string stateNames, string aDate)
        {
            List<ReportResource> ReportList = null;
            IQueryable<reporting_metrics> query = null;

            try
            {
                DateTime? thisDate = (DateTime?)Convert.ToDateTime(aDate);
                List<string> states = new List<string>();
                char[] delimiter = { ',' };
                //stateNames will be a list that is comma separated. Need to parse out
                if (stateNames != null && stateNames != string.Empty)
                    states = stateNames.Split(delimiter, StringSplitOptions.RemoveEmptyEntries).ToList();

                Int32 eventID = eventId > 0 ? eventId : -1;

                //get all the reports first to then narrow down
                query = this.Select<reporting_metrics>().Include(r => r.reportmetric_contact).ThenInclude(rmc=> rmc.contact_type);

                //do we have an EVENT?
                if (eventID > 0)
                {
                    query = this.Select<reporting_metrics>().Where(e => e.event_id == eventID);
                }

                //query DATE
                query = query.Where(e => e.report_date == thisDate.Value);

                if (states.Count >= 2)
                {

                    //multiple STATES
                    query = from q in query where states.Any(s => q.state.Contains(s.Trim())) select q;
                }

                if (states.Count == 1)
                {
                    if (states[0] != "All States" && states[0] != "0")
                    {
                        string thisState = states[0];
                        thisState = thisState.ToUpper();
                        query = query.Where(r => r.state == thisState);
                    }
                }

                query = query.Where(x => x.complete == 1);


                ReportList = query.Select(rm => new ReportResource
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
                }).ToList();

                return ReportList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<reporting_metrics> GetFiltedReports(string eventId, string aDate, string stateNames)
        {
            List<reporting_metrics> ReportList = null;
            IQueryable<reporting_metrics> query = null;

            try
            {
                DateTime? thisDate = (DateTime?)Convert.ToDateTime(aDate);
                List<string> states = new List<string>();
                char[] delimiter = { ',' };
                //stateNames will be a list that is comma separated. Need to parse out
                if (!string.IsNullOrEmpty(stateNames))
                {
                    states = stateNames.Split(delimiter, StringSplitOptions.RemoveEmptyEntries).ToList();
                }

                Int32 eventID = !string.IsNullOrEmpty(eventId) ? Convert.ToInt32(eventId) : -1;

                //get all the reports first to then narrow down
                query = this.Select<reporting_metrics>();

                //do we have an EVENT?
                if (eventID > 0)
                {
                    query = this.Select<reporting_metrics>().Where(e => e.event_id == eventID);
                }

                //do we have an EVENT?
                if (eventID > 0)
                {
                    query = query.Where(e => e.event_id == eventID);
                }

                if (states.Count >= 2)
                {
                    //multiple STATES
                    query = from q in query where states.Any(s => q.state.Contains(s.Trim())) select q;
                }

                if (states.Count == 1)
                {
                    if (states[0] != "All States" && states[0] != "0")
                    {
                        string thisState = states[0];
                        thisState = thisState.ToUpper();
                        query = query.Where(r => r.state == thisState);
                    }
                }

                //only completed reports please
                ReportList = query.AsEnumerable().Where(x => x.complete == 1).ToList();

                return ReportList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        #region s3 bucket stuff
      /*  public FileStream GetFileItem(file afile)
        {
            
            //file aFile = null;
            //S3Bucket aBucket = null;
            aBucket = null;
            FileStream fileItem = null;
            try
            {
                if (afile == null || afile.path == null || String.IsNullOrEmpty(afile.path)) throw new WiM.Exceptions.NotFoundRequestException();

                string directoryName = string.Empty;
                aBucket = new S3Bucket(Configuration.AppSettings["AWSBucket"], ConfigurationManager.AppSettings["AWSAccessKey"],
                                        ConfigurationManager.AppSettings["AWSSecretKey"]);
                directoryName = afile.path + "/" + afile.name;
                var fileStream = aBucket.GetObject(directoryName);

                fileItem = new InMemoryFile(fileStream);
                fileItem.ContentType = GetContentType(afile.name);
                fileItem.Length = fileStream != null ? fileStream.Length : 0;
                fileItem.FileName = afile.name;
                return fileItem;

            }
            catch (Exception ex)
            {
                sm(WiM.Resources.MessageType.error, "Failed to include item: " + afile.path + " exception: " + ex.Message);
                throw;
            }
        }*/
        /*
             internal InMemoryFile GetHWMSpreadsheetItem()
            {
                S3Bucket aBucket = null;
                InMemoryFile fileItem = null;
                try
                {
                    string directoryName = string.Empty;
                    aBucket = new S3Bucket(ConfigurationManager.AppSettings["AWSBucket"], ConfigurationManager.AppSettings["AWSAccessKey"],
                                            ConfigurationManager.AppSettings["AWSSecretKey"]);
                    directoryName = "cleanHistoricHWMUploadSpreadsheet.xlsx";
                    var fileStream = aBucket.GetObject(directoryName);

                    fileItem = new InMemoryFile(fileStream);
                    fileItem.ContentType = GetContentType(".XLSX");
                   // fileItem.Length = fileStream != null ? fileStream.Length : 0;
                    fileItem.FileName = "cleanHistoricHWMUploadSpreadsheet.xlsx";
                    return fileItem;

                }
                catch (Exception ex)
                {
                    sm(WiM.Resources.MessageType.error, "Failed to include item: cleanHistoricHWMUploadSpreadsheet.xlsx exception: " + ex.Message);
                    throw;
                }
            }
         */
        #endregion

        #region Universal
        public new IQueryable<T> Select<T>() where T : class, new()
        {
            return base.Select<T>();
        }
        public new Task<T> Find<T>(Int32 pk) where T : class, new()
        {
            return base.Find<T>(pk);
        }
        public new Task<T> Add<T>(T item) where T : class, new()
        {
            return base.Add<T>(item);
        }
        public new Task<IEnumerable<T>> Add<T>(List<T> items) where T : class, new()
        {
            return base.Add<T>(items);
        }
        public new Task<T> Update<T>(Int32 pkId, T item) where T : class, new()
        {
            return base.Update<T>(pkId, item);
        }
        public new Task Delete<T>(T item) where T : class, new()
        {
            return base.Delete<T>(item);
        }
        #endregion
        
        #region Roles
        public IQueryable<roles> GetRoles() {
            try
            {
                return this.Select<roles>();
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
 
        #region Member
        public IBasicUser GetUserByUsername(string username) {
            try
            {

                return Select<members>().Include(p => p.roles).Where(u => string.Equals(u.username, username, StringComparison.OrdinalIgnoreCase))
                    .Select(u => new User() { FirstName = u.fname, LastName = u.lname,
                     Email = u.email, Role= u.roles.role_name, RoleID = u.role_id, ID= u.role_id, Username = u.username, Salt=u.salt, password = u.password} ).FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        
        #region HELPER METHODS
        public IQueryable<T> getTable<T>(object[] args) where T : class, new()
        {
            try
            {
                string sql = string.Empty;
                if (args[0] != null)
                {
                    if (args[0].ToString() == "baro_view" || args[0].ToString() == "met_view" || args[0].ToString() == "rdg_view" || args[0].ToString() == "stormtide_view" || args[0].ToString() == "waveheight_view" ||
                         args[0].ToString() == "pressuretemp_view" || args[0].ToString() == "therm_view" || args[0].ToString() == "webcam_view" || args[0].ToString() == "raingage_view" || args[0].ToString() == "peak_view")
                        sql = String.Format(getSQLStatement(args[0].ToString()));
                }
                else
                    sql = @"SELECT * FROM events";
                    //sql = String.Format(getSQLStatement(typeof(T).Name), args);

                return FromSQL<T>(sql);// .Database.SqlQuery<T>(sql).AsQueryable(); ////TODO::: NOT working ///////
                //return x;

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private string getSQLStatement(string type)
        {
            string sql = string.Empty;
            switch (type)
            {
                case "peak_view":
                    return @"SELECT DISTINCT ON (pk.peak_summary_id) pk.peak_summary_id, pk.peak_stage, pk.peak_date, vd.datum_name, 
                        CASE
                            WHEN NOT hwm_v.peak_summary_id IS NULL THEN hwm_v.latitude_dd
                            WHEN NOT df_v.peak_summary_id IS NULL THEN df_v.latitude_dd
                            ELSE NULL::double precision
                            END AS latitude,
                        CASE
                            WHEN NOT hwm_v.peak_summary_id IS NULL THEN hwm_v.site_id
                            WHEN NOT df_v.peak_summary_id IS NULL THEN df_v.site_id
                            ELSE NULL::integer
                            END AS site_id,
                        CASE
                            WHEN NOT hwm_v.peak_summary_id IS NULL THEN hwm_v.longitude_dd
                            WHEN NOT df_v.peak_summary_id IS NULL THEN df_v.longitude_dd
                            ELSE NULL::double precision
                            END AS longitude,
                        CASE
                            WHEN NOT hwm_v.peak_summary_id IS NULL THEN hwm_v.event_name
                            WHEN NOT df_v.peak_summary_id IS NULL THEN df_v.event_name
                            ELSE NULL::character varying
                            END AS event_name
                    FROM peak_summary pk
                    LEFT JOIN vertical_datums vd ON pk.vdatum_id = vd.datum_id
                    LEFT JOIN ( SELECT df.peak_summary_id, sv.latitude_dd, sv.longitude_dd, sv.site_id, sv.event_name
                                FROM sensor_view sv, data_file df
                                WHERE df.instrument_id = sv.instrument_id AND NOT df.peak_summary_id IS NULL) df_v ON pk.peak_summary_id = df_v.peak_summary_id
                    LEFT JOIN ( SELECT hwm.peak_summary_id, hwm.latitude_dd, hwm.longitude_dd, hwm.site_id, e.event_name
                                FROM hwm hwm, events e
                                WHERE hwm.event_id = e.event_id AND NOT hwm.peak_summary_id IS NULL) hwm_v ON pk.peak_summary_id = hwm_v.peak_summary_id;";
                case "baro_view":
                    return @"SELECT sensor_view.site_id, sensor_view.site_no, sensor_view.site_name, sensor_view.latitude_dd, sensor_view.longitude_dd, sensor_view.county, sensor_view.state, sensor_view.city, sensor_view.instrument_id,
                            sensor_view.event_id, sensor_view.event_name, sensor_view.status, sensor_view.time_stamp, sensor_view.inst_collection_id, sensor_view.event_type_id, sensor_view.event_status_id, sensor_view.status_type_id,
                            sensor_view.deployment_type_id, sensor_view.sensor_type_id
                            FROM (SELECT s.site_id, s.site_no, s.site_name, s.latitude_dd,s.longitude_dd,s.county,s.state,s.city,i.instrument_id,e.event_id,e.event_name,e.event_start_date, e.event_end_date as event_end_date,i.sensor_type_id,sent.sensor,
                                i.deployment_type_id, dept.method, st.status, ins1.time_stamp, e.event_type_id, e.event_status_id, i.inst_collection_id, st.status_type_id
                                FROM fradmin.sites s, fradmin.status_type st, fradmin.events e, ((((fradmin.instrument i
                                JOIN fradmin.instrument_status ins1 ON ((i.instrument_id = ins1.instrument_id)))
                                LEFT JOIN fradmin.instrument_status ins2 ON (((i.instrument_id = ins2.instrument_id) AND ((ins1.time_stamp < ins2.time_stamp) OR ((ins1.time_stamp = ins2.time_stamp) AND (ins1.instrument_status_id < ins2.instrument_status_id))))))
                                LEFT JOIN fradmin.sensor_type sent ON ((sent.sensor_type_id = i.sensor_type_id)))
                                LEFT JOIN fradmin.deployment_type dept ON ((dept.deployment_type_id = i.deployment_type_id)))
                                WHERE ((((ins2.instrument_status_id IS NULL) AND (s.site_id = i.site_id)) AND (ins1.status_type_id = st.status_type_id)) AND (i.event_id = e.event_id))) AS sensor_view
                            WHERE ((sensor_view.sensor_type_id = 1) AND(sensor_view.deployment_type_id = 3))"; 
                    /*
                    return @"SELECT sensor_view.site_id,
                        sensor_view.site_no,
                        sensor_view.site_name,
                        sensor_view.latitude_dd,
                        sensor_view.longitude_dd,
                        sensor_view.county,
                        sensor_view.state,
                        sensor_view.city,
                        sensor_view.instrument_id,
                        sensor_view.event_id,
                        sensor_view.event_name,
                        sensor_view.status,
                        sensor_view.time_stamp,
                        sensor_view.inst_collection_id,
                        sensor_view.event_type_id,
                        sensor_view.event_status_id,
                        sensor_view.status_type_id,
                        sensor_view.deployment_type_id,
                        sensor_view.sensor_type_id
                        FROM sensor_view
                        WHERE ((sensor_view.sensor_type_id = 1) AND (sensor_view.deployment_type_id = 3))"; 
                */
            
            //return "SELECT* FROM fradmin.barometric_view; ";
                case "met_view":
                    return @"SELECT * FROM meteorological_view;";
                case "rdg_view":
                    return @"SELECT * FROM rapid_deployment_view;";
                case "stormtide_view":
                    return @"SELECT * FROM storm_tide_view;";
                case "waveheight_view":
                    return @"SELECT * FROM wave_height_view;";
                case "pressuretemp_view":
                    return @"SELECT * FROM pressuretemp_view;";
                case "therm_view":
                    return @"SELECT * FROM therm_view;";
                case "webcam_view":
                    return @"SELECT * FROM webcam_view;";
                case "raingage_view":
                    return @"SELECT * FROM raingage_view;";
                case "dataFile_view":
                    return @"SELECT s.site_id, s.site_no, s.latitude_dd, s.longitude_dd, s.county, s.state, s.waterbody, i.instrument_id, i.event_id, i.sensor_type_id, st.sensor, i.deployment_type_id, dt.method,
                                i.location_description, i.sensor_brand_id, sb.brand_name, f.file_id, f.name, f.file_date, f.script_parent, f.data_file_id, df.good_start, df.good_end, df.collect_date
                            FROM sites s
                            LEFT JOIN instrument i ON s.site_id = i.site_id
                            LEFT JOIN files f ON i.instrument_id = f.instrument_id
                            LEFT JOIN sensor_type st ON i.sensor_type_id = st.sensor_type_id
                            LEFT JOIN deployment_type dt ON i.deployment_type_id = dt.deployment_type_id
                            LEFT JOIN sensor_brand sb ON i.sensor_brand_id = sb.sensor_brand_id
                            LEFT JOIN data_file df ON f.data_file_id = df.data_file_id
                            WHERE i.sensor_type_id = 1 AND i.deployment_type_id = 3 AND f.filetype_id = 2;";
                default:
                    throw new Exception("No sql for table " + type);
            }//end switch;

        }
        public DateTime? ValidDate(string date)
        {
            DateTime tempDate;
            try
            {
                if (date == null) return null;
                if (!DateTime.TryParse(date, out tempDate))
                {
                    //try oadate
                    tempDate = DateTime.FromOADate(Convert.ToDouble(date));

                }


                return tempDate;
                // 
            }
            catch (Exception)
            {

                return null;
            }

        }//end ValidDate
        private string getApprovalName(hwm hw)
        {
            try
            {
                return hw.approval.members.fname + " " + hw.approval.members.lname;
            }
            catch
            {
                return "Missing Approval";
            }            
        }
        private decimal GetThisPeaksSiteID(peak_summary peak)
        {
            decimal siteID = 0;
            if (peak.hwms.Count > 0)
            {
                globalPeakSite = peak.hwms.FirstOrDefault().site;
                siteID = Convert.ToDecimal(peak.hwms.FirstOrDefault().site_id);
            }
            else
            {
                globalPeakSite = peak.data_file.FirstOrDefault().instrument.site;
                siteID = Convert.ToDecimal(peak.data_file.FirstOrDefault().instrument.site_id);
            }

            return siteID;
        }
        private List<string> getSiteEvents(sites s)
        {
            List<string> eventNames = new List<string>();
            if (s.hwms.Count >= 1)
            {
                eventNames.AddRange(s.hwms.Where(h => h.event_id != null).Select(h => h.@event.event_name).ToList());
            }
            if (s.instruments.Count >= 1)
            {
                // make sure this isn't just a proposed sensor with no event
                eventNames.AddRange(s.instruments.Where(i => i.event_id != null && i.event_id > 0).Select(i => i.@event.event_name).ToList());
            }

            return eventNames.Distinct().ToList();
        }

        private recent_op getRecentOP(sites s)
        {
            recent_op recentop = null;

            if (s.objective_points.Count > 0)
                recentop = s.objective_points.OrderByDescending(x => x.date_established).Select(o => new recent_op
                {
                    name = o.name,
                    date_established = o.date_established.Value
                }).FirstOrDefault<recent_op>();

            return recentop;
        }
        /*               
            private string getSQLStatement(sqlTypes type) {

                switch (type)
                {
                    case sqlTypes.e_sourcebygeojson:
                        return @"select * from ""public"".""Sources""
                                    where ST_Within(
                                        ""Location"",
                                        st_makevalid(
                                            st_transform(
                                                st_setsrid(
                                                    ST_GeomFromGeoJSON('{{{0}}}'),
                                                {1}),
                                            4269)))";
                    case sqlTypes.e_source:
                        return @"SELECT * FROM ""public"".""Sources"" 
                                    WHERE ""ID"" IN ('{0}') OR 
                                    LOWER(""FacilityCode"") IN ('{1}')";


                    default:
                        throw new Exception("No sql for table " + type);
                }
            }
      */
        #endregion
        #region Enumerations
        private enum sqlTypes
        {
            e_sourcebygeojson,
            e_source
        }
        #endregion
    }
}
