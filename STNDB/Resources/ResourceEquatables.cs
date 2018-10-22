//------------------------------------------------------------------------------
//----- Equality ---------------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2017 WiM - USGS

//    authors:  Jeremy K. Newson USGS Web Informatics and Mapping
//              
//  
//   purpose:   Overrides Equatable
//
//discussion:   https://blogs.msdn.microsoft.com/ericlippert/2011/02/28/guidelines-and-rules-for-gethashcode/    
//              http://www.aaronstannard.com/overriding-equality-in-dotnet/
//
//              var hashCode = 13;
//              hashCode = (hashCode * 397) ^ MyNum;
//              var myStrHashCode = !string.IsNullOrEmpty(MyStr) ?
//                                      MyStr.GetHashCode() : 0;
//              hashCode = (hashCode * 397) ^ MyStr;
//              hashCode = (hashCode * 397) ^ Time.GetHashCode();
//               
using System;
using System.Collections.Generic;
using System.Text;

namespace STNDB.Resources
{
    public partial class agency : IEquatable<agency>
    {
        public bool Equals(agency other)
        {
            if (other == null) return false;
            return string.Equals(this.agency_name.ToLower(), other.agency_name.ToLower()) &&
                (this.address?.ToLower() ?? "") == (other.address?.ToLower() ?? "") &&
                (this.city?.ToLower() ?? "") == (other.city?.ToLower() ?? "") &&
                (this.state?.ToLower() ?? "") == (other.state?.ToLower() ?? "") &&
                (this.zip?.ToLower() ?? "") == (other.zip?.ToLower() ?? "") &&
                (this.phone?.ToLower() ?? "") == (other.phone?.ToLower() ?? "");
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as agency);
        }
        public override int GetHashCode()
        {
            return (this.agency_id + this.agency_name).GetHashCode();
        }
    }
    public partial class approval : IEquatable<approval>
    {
        public bool Equals(approval other)
        {
            return this.member_id == other.member_id && DateTime.Equals(this.approval_date, other.approval_date);
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as approval);
        }
        public override int GetHashCode()
        {
            return (this.member_id.Value + this.approval_date.ToString()).GetHashCode();
        }
    }
    public partial class contact : IEquatable<contact>
    {
        public bool Equals(contact other)
        {
            return String.Equals(this.fname.ToLower(), other.fname.ToLower()) &&
                String.Equals(this.lname.ToLower(), other.lname.ToLower()) &&
                String.Equals(this.phone.ToLower(), other.phone.ToLower()) &&
                (this.alt_phone?.ToLower() ?? "") == (other.alt_phone?.ToLower() ?? "") &&
                (this.email?.ToLower() ?? "") == (other.email?.ToLower() ?? "") &&
                DateTime.Equals(this.last_updated, other.last_updated) &&
                this.last_updated_by == other.last_updated_by;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as contact);
        }
        public override int GetHashCode()
        {
            return (this.fname + this.lname + this.phone + this.last_updated.ToString() + this.last_updated_by).GetHashCode();
        }
    }
    public partial class contact_type : IEquatable<contact_type>
    {
        public bool Equals(contact_type other)
        {
            return String.Equals(this.type.ToLower(),other.type.ToLower());
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as contact_type);
        }
        public override int GetHashCode()
        {
            return (this.type).GetHashCode();
        }
    }
    public partial class county : IEquatable<county>
    {
        public bool Equals(county other)
        {
            return String.Equals(this.county_name.ToLower(), other.county_name.ToLower()) &&
                this.state_fip == other.state_fip && this.state_fip == other.state_fip &&
                this.county_fip == other.county_fip;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as county);
        }
        public override int GetHashCode()
        {
            return (this.county_name + this.state_fip + this.state_fip + this.county_fip).GetHashCode();
        }
    }
    public partial class data_file : IEquatable<data_file>
    {
        public bool Equals(data_file other)
        {
            return DateTime.Equals(this.start, other.start) &&
                DateTime.Equals(this.end, other.end) &&
                DateTime.Equals(this.good_start, other.good_start) &&
                DateTime.Equals(this.good_end, other.good_end) &&
                this.processor_id == other.processor_id &&
                this.instrument_id == other.instrument_id &&
                this.approval_id == other.approval_id &&
                DateTime.Equals(this.collect_date, other.collect_date) &&
                this.peak_summary_id == other.peak_summary_id &&
                (this.elevation_status?.ToLower() ?? "") == (other.elevation_status?.ToLower() ?? "") &&
                (this.time_zone?.ToLower() ?? "") == (other.time_zone?.ToLower() ?? "") &&
                DateTime.Equals(this.last_updated, other.last_updated) &&
                this.last_updated_by == other.last_updated_by;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as data_file);
        }
        public override int GetHashCode()
        {
            return (this.good_start.ToString() + this.good_end.ToString() + this.processor_id + this.instrument_id + this.collect_date.ToString() +
                this.last_updated.ToString() + this.last_updated_by).GetHashCode();
        }
    }
    public partial class deployment_priority : IEquatable<deployment_priority>
    {
        public bool Equals(deployment_priority other)
        {
            return String.Equals(this.priority_name.ToLower(), other.priority_name.ToLower());
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as deployment_priority);
        }
        public override int GetHashCode()
        {
            return (this.priority_name).GetHashCode();
        }
    }
    public partial class deployment_type : IEquatable<deployment_type>
    {
        public bool Equals(deployment_type other)
        {
            return String.Equals(this.method.ToLower(), other.method.ToLower());
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as deployment_type);
        }
        public override int GetHashCode()
        {
            return (this.method).GetHashCode();
        }
    }
    public partial class event_status : IEquatable<event_status>
    {
        public bool Equals(event_status other)
        {
            return String.Equals(this.status.ToLower(), other.status.ToLower());
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as event_status);
        }
        public override int GetHashCode()
        {
            return (this.status).GetHashCode();
        }
    }
    public partial class event_type : IEquatable<event_type>
    {
        public bool Equals(event_type other)
        {
            return String.Equals(this.type.ToLower(), other.type.ToLower());
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as event_type);
        }
        public override int GetHashCode()
        {
            return (this.type).GetHashCode();
        }
    }
    public partial class events : IEquatable<events>
    {
        public bool Equals(events other)
        {
            return String.Equals(this.event_name.ToLower(), other.event_name.ToLower()) &&
                DateTime.Equals(this.event_start_date, other.event_start_date) &&
                DateTime.Equals(this.event_end_date, other.event_end_date) &&
                (this.event_description?.ToLower() ?? "") == (other.event_description?.ToLower() ?? "") &&
                this.event_type_id == other.event_type_id &&
                this.event_status_id == other.event_status_id &&
                this.event_coordinator == other.event_coordinator;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as events);
        }
        public override int GetHashCode()
        {
            return (this.event_name + this.event_type_id + this.event_status_id + this.event_coordinator).GetHashCode();
        }
    }
    public partial class file : IEquatable<file>
    {
        public bool Equals(file other)
        {
            return (this.name?.ToLower() ?? "") == (other.name?.ToLower() ?? "") &&
                    (this.description?.ToLower() ?? "") == (other.description?.ToLower() ?? "") &&
                    (this.photo_direction?.ToLower() ?? "") == (other.photo_direction?.ToLower() ?? "") &&
                    this.latitude_dd == other.latitude_dd && this.longitude_dd == other.longitude_dd &&
                    DateTime.Equals(this.file_date, other.file_date) &&
                    this.hwm_id == other.hwm_id && this.site_id == other.site_id &&
                    this.filetype_id == other.filetype_id && this.source_id == other.source_id &&
                    (this.path?.ToLower() ?? "") == (other.path?.ToLower() ?? "") &&
                    this.data_file_id == other.data_file_id && this.instrument_id == other.instrument_id &&
                    DateTime.Equals(this.photo_date, other.photo_date) &&
                    this.is_nwis == other.is_nwis && this.objective_point_id == other.objective_point_id;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as file);
        }
        public override int GetHashCode()
        {
            return (this.filetype_id + this.file_date.GetHashCode() + site_id).GetHashCode();
        }
    }
    public partial class file_type : IEquatable<file_type>
    {
        public bool Equals(file_type other)
        {
            return String.Equals(this.filetype.ToLower(), other.filetype.ToLower());
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as file_type);
        }
        public override int GetHashCode()
        {
            return (this.filetype).GetHashCode();
        }
    }
    public partial class horizontal_collect_methods : IEquatable<horizontal_collect_methods>
    {
        public bool Equals(horizontal_collect_methods other)
        {
            return String.Equals(this.hcollect_method.ToLower(), other.hcollect_method.ToLower());
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as horizontal_collect_methods);
        }
        public override int GetHashCode()
        {
            return (this.hcollect_method).GetHashCode();
        }
    }
    public partial class horizontal_datums : IEquatable<horizontal_datums>
    {
        public bool Equals(horizontal_datums other)
        {
            return String.Equals(this.datum_name.ToLower(), other.datum_name.ToLower()) &&
                (this.datum_abbreviation?.ToLower() ?? "") == (other.datum_abbreviation?.ToLower() ?? "");
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as horizontal_datums);
        }
        public override int GetHashCode()
        {
            return (this.datum_name + this.datum_abbreviation).GetHashCode();
        }
    }
    public partial class housing_type : IEquatable<housing_type>
    {
        public bool Equals(housing_type other)
        {
            return String.Equals(this.type_name.ToLower(), other.type_name.ToLower());
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as housing_type);
        }
        public override int GetHashCode()
        {
            return (this.type_name).GetHashCode();
        }
    }
    public partial class hwm : IEquatable<hwm>
    {
        public bool Equals(hwm other)
        {
            return (this.waterbody?.ToLower() ?? "") == (other.waterbody?.ToLower() ?? "") && 
                this.site_id == other.site_id && this.event_id == other.event_id &&
                this.hwm_type_id == other.hwm_type_id && this.hwm_quality_id == other.hwm_quality_id &&
                (this.hwm_locationdescription?.ToLower() ?? "") == (other.hwm_locationdescription?.ToLower() ?? "") &&
                this.latitude_dd == other.latitude_dd && this.longitude_dd == other.longitude_dd &&
                DateTime.Equals(this.survey_date, other.survey_date) &&
                this.elev_ft == other.elev_ft && this.vdatum_id == other.vdatum_id &&
                this.vcollect_method_id == other.vcollect_method_id &&
                (this.bank?.ToLower() ?? "") == (other.bank?.ToLower() ?? "") &&
                this.approval_id == other.approval_id && this.marker_id == other.marker_id &&
                this.height_above_gnd == other.height_above_gnd && this.hcollect_method_id == other.hcollect_method_id &&
                this.peak_summary_id == other.peak_summary_id &&
                (this.hwm_notes?.ToLower() ?? "") == (other.hwm_notes?.ToLower() ?? "") &&
                (this.hwm_environment?.ToLower() ?? "") == (other.hwm_environment?.ToLower() ?? "") &&
                DateTime.Equals(this.flag_date, other.flag_date) &&
                this.stillwater == other.stillwater && this.hdatum_id == other.hdatum_id &&
                this.flag_member_id == other.flag_member_id && this.survey_member_id == other.survey_member_id &&
                this.uncertainty == other.uncertainty && this.hwm_uncertainty == other.hwm_uncertainty &&
                (this.hwm_label?.ToLower() ?? "") == (other.hwm_label?.ToLower() ?? "");
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as hwm);
        }
        public override int GetHashCode()
        {
            return (this.site_id + this.event_id + this.hwm_type_id + this.flag_date.GetHashCode() +
                this.hwm_quality_id + this.hwm_environment + this.hdatum_id + this.flag_member_id + this.hcollect_method_id).GetHashCode();
        }
    }
    public partial class hwm_qualities : IEquatable<hwm_qualities>
    {
        public bool Equals(hwm_qualities other)
        {
            return String.Equals(this.hwm_quality.ToLower(), other.hwm_quality.ToLower()) &&
                this.min_range == other.min_range && this.max_range == other.max_range;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as hwm_qualities);
        }
        public override int GetHashCode()
        {
            return (this.hwm_quality + this.min_range + this.max_range).GetHashCode();
        }
    }
    public partial class hwm_types : IEquatable<hwm_types>
    {
        public bool Equals(hwm_types other)
        {
            return String.Equals(this.hwm_type.ToLower(), other.hwm_type.ToLower());
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as hwm_types);
        }
        public override int GetHashCode()
        {
            return (this.hwm_type).GetHashCode();
        }
    }
    public partial class instr_collection_conditions : IEquatable<instr_collection_conditions>
    {
        public bool Equals(instr_collection_conditions other)
        {
            return String.Equals(this.condition.ToLower(), other.condition.ToLower());
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as instr_collection_conditions);
        }
        public override int GetHashCode()
        {
            return (this.condition).GetHashCode();
        }
    }
    public partial class instrument : IEquatable<instrument>
    {
        public bool Equals(instrument other)
        {
            return this.sensor_type_id == other.sensor_type_id && this.deployment_type_id == other.deployment_type_id &&
                (this.location_description?.ToLower() ?? "") == (other.location_description?.ToLower() ?? "") &&
                (this.serial_number?.ToLower() ?? "") == (other.serial_number?.ToLower() ?? "") &&
                (this.housing_serial_number?.ToLower() ?? "") == (other.housing_serial_number?.ToLower() ?? "") &&
                this.interval == other.interval && this.site_id == other.site_id &&
                this.event_id == other.event_id && this.inst_collection_id == other.inst_collection_id &&
                this.housing_type_id == other.housing_type_id && this.sensor_brand_id == other.sensor_brand_id &&
                (this.vented?.ToLower() ?? "") == (other.vented?.ToLower() ?? "");
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as instrument);
        }
        public override int GetHashCode()
        {
            return (this.sensor_type_id + this. site_id).GetHashCode();
        }
    }
    public partial class instrument_status : IEquatable<instrument_status>
    {
        public bool Equals(instrument_status other)
        {
            return this.status_type_id == other.status_type_id && this.instrument_id == other.instrument_id &&
                DateTime.Equals(this.time_stamp, other.time_stamp) &&
                (this.notes?.ToLower() ?? "") == (other.notes?.ToLower() ?? "") &&
                (this.time_zone?.ToLower() ?? "") == (other.time_zone?.ToLower() ?? "") &&
                this.member_id == other.member_id && this.sensor_elevation == other.sensor_elevation &&
                this.ws_elevation == other.ws_elevation && this.gs_elevation == other.gs_elevation && this.vdatum_id == other.vdatum_id;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as instrument_status);
        }
        public override int GetHashCode()
        {
            return (this.instrument_id + this.time_stamp.GetHashCode() + this.status_type_id + this.member_id + this.time_zone).GetHashCode();
        }
    }
    public partial class landownercontact : IEquatable<landownercontact>
    {
        public bool Equals(landownercontact other)
        {
            return (this.fname?.ToLower() ?? "") == (other.fname?.ToLower() ?? "") &&
                (this.lname?.ToLower() ?? "") == (other.lname?.ToLower() ?? "") &&
                (this.address?.ToLower() ?? "") == (other.address?.ToLower() ?? "") &&
                (this.city?.ToLower() ?? "") == (other.city?.ToLower() ?? "") &&
                (this.state?.ToLower() ?? "") == (other.state?.ToLower() ?? "") &&
                (this.zip?.ToLower() ?? "") == (other.zip?.ToLower() ?? "") &&
                (this.primaryphone?.ToLower() ?? "") == (other.primaryphone?.ToLower() ?? "") &&
                (this.secondaryphone?.ToLower() ?? "") == (other.secondaryphone?.ToLower() ?? "") &&
                (this.email?.ToLower() ?? "") == (other.email?.ToLower() ?? "") &&
                (this.title?.ToLower() ?? "") == (other.title?.ToLower() ?? "");
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as landownercontact);
        }
        public override int GetHashCode()
        {
            return (this.fname + this.lname).GetHashCode();
        }
    }
    public partial class markers : IEquatable<markers>
    {
        public bool Equals(markers other)
        {
            return (this.marker?.ToLower() ?? "") == (other.marker?.ToLower() ?? "");
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as markers);
        }
        public override int GetHashCode()
        {
            return (this.marker).GetHashCode();
        }
    }
    public partial class members : IEquatable<members>
    {
        public bool Equals(members other)
        {
            return String.Equals(this.fname.ToLower(), other.fname.ToLower()) &&
                String.Equals(this.lname.ToLower(), other.lname.ToLower()) &&
                this.agency_id == other.agency_id &&
                String.Equals(this.phone.ToLower(), other.phone.ToLower()) &&
                String.Equals(this.email.ToLower(), other.email.ToLower()) &&
                (this.emergency_contact_name?.ToLower() ?? "") == (other.emergency_contact_name?.ToLower() ?? "") &&
                (this.emergency_contact_phone?.ToLower() ?? "") == (other.emergency_contact_phone?.ToLower() ?? "") &&
                this.role_id == other.role_id && String.Equals(this.username.ToLower(), other.username.ToLower()) &&
                this.resetFlag == other.resetFlag;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as members);
        }
        public override int GetHashCode()
        {
            return (this.fname + this.lname + this.username + this.phone + this.email + this.agency_id).GetHashCode();
        }
    }
    public partial class network_name : IEquatable<network_name>
    {
        public bool Equals(network_name other)
        {
            return String.Equals(this.name.ToLower(), other.name.ToLower());
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as network_name);
        }
        public override int GetHashCode()
        {
            return (this.name).GetHashCode();
        }
    } 
    public partial class network_type : IEquatable<network_type>
    {
        public bool Equals(network_type other)
        {
            return String.Equals(this.network_type_name.ToLower(), other.network_type_name.ToLower());
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as network_type);
        }
        public override int GetHashCode()
        {
            return (this.network_type_name).GetHashCode();
        }
    } 
    public partial class objective_point : IEquatable<objective_point>
    {
        public bool Equals(objective_point other)
        {
            return String.Equals(this.name.ToLower(), other.name.ToLower()) &&
                String.Equals(this.description.ToLower(), other.description.ToLower()) &&
                this.elev_ft == other.elev_ft && DateTime.Equals(this.date_established, other.date_established) &&
                this.op_is_destroyed == other.op_is_destroyed &&
                (this.op_notes?.ToLower() ?? "") == (other.op_notes?.ToLower() ?? "") &&
                this.site_id == other.site_id && this.vdatum_id == other.vdatum_id && this.latitude_dd == other.latitude_dd &&
                this.longitude_dd == other.longitude_dd && this.hdatum_id == other.hdatum_id &&
                this.hcollect_method_id == other.hcollect_method_id && this.vcollect_method_id == other.vcollect_method_id &&
                this.op_type_id == other.op_type_id && DateTime.Equals(this.date_recovered, other.date_recovered) &&
                this.uncertainty == other.uncertainty &&
                (this.unquantified?.ToLower() ?? "") == (other.unquantified?.ToLower() ?? "") && 
                this.op_quality_id == other.op_quality_id; 
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as objective_point);
        }
        public override int GetHashCode()
        {
            return (this.name + this.description + this.op_type_id + this.date_established.GetHashCode() + this.site_id + this.latitude_dd +
                this.longitude_dd + this.vdatum_id).GetHashCode();
        }
    } 
    public partial class objective_point_type : IEquatable<objective_point_type>
    {
        public bool Equals(objective_point_type other)
        {
            return String.Equals(this.op_type.ToLower(), other.op_type.ToLower());
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as objective_point_type);
        }
        public override int GetHashCode()
        {
            return (this.op_type).GetHashCode();
        }
    } 
    public partial class op_control_identifier : IEquatable<op_control_identifier>
    {
        public bool Equals(op_control_identifier other)
        {
            return this.objective_point_id == other.objective_point_id &&
                String.Equals(this.identifier.ToLower(), other.identifier.ToLower()) &&
                String.Equals(this.identifier_type.ToLower(), other.identifier_type.ToLower());
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as op_control_identifier);
        }
        public override int GetHashCode()
        {
            return (this.objective_point_id + this.identifier + this.identifier_type).GetHashCode();
        }
    } 
    public partial class op_measurements : IEquatable<op_measurements>
    {
        public bool Equals(op_measurements other)
        {
            return this.objective_point_id == other.objective_point_id && this.instrument_status_id == other.instrument_status_id &&
                this.water_surface == other.water_surface && this.ground_surface == other.ground_surface &&
                this.offset_correction == other.offset_correction;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as op_measurements);
        }
        public override int GetHashCode()
        {
            return (this.objective_point_id + this.instrument_status_id).GetHashCode();
        }
    } 
    public partial class op_quality : IEquatable<op_quality>
    {
        public bool Equals(op_quality other)
        {
            return String.Equals(this.quality.ToLower(), other.quality.ToLower());
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as op_quality);
        }
        public override int GetHashCode()
        {
            return (this.quality).GetHashCode();
        }
    } 
    public partial class peak_summary : IEquatable<peak_summary>
    {
        public bool Equals(peak_summary other)
        {
            return this.member_id == other.member_id && DateTime.Equals(this.peak_date, other.peak_date) &&
                this.is_peak_estimated == other.is_peak_estimated && this.is_peak_time_estimated == other.is_peak_time_estimated &&
                this.peak_stage == other.peak_stage && this.is_peak_stage_estimated == other.is_peak_stage_estimated &&
                this.peak_discharge == other.peak_discharge && this.is_peak_discharge_estimated == other.is_peak_discharge_estimated &&
                this.vdatum_id == other.vdatum_id && this.height_above_gnd == other.height_above_gnd &&
                this.is_hag_estimated == other.is_hag_estimated && String.Equals(this.time_zone.ToLower(), other.time_zone.ToLower()) &&
                this.aep == other.aep && this.aep_lowci == other.aep_lowci && this.aep_upperci == other.aep_upperci && 
                this.aep_range == other.aep_range && 
                (this.calc_notes?.ToLower() ?? "") == (other.calc_notes?.ToLower() ?? "");
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as peak_summary);
        }
        public override int GetHashCode()
        {
            return (this.member_id + this.peak_date.GetHashCode() + this.time_zone).GetHashCode();
        }
    } 
    public partial class reporting_metrics : IEquatable<reporting_metrics>
    {
        public bool Equals(reporting_metrics other)
        {
            return DateTime.Equals(this.report_date, other.report_date) && this.event_id == other.event_id &&
                String.Equals(this.state.ToLower(), other.state.ToLower()) && this.sw_fieldpers_notacct == other.sw_fieldpers_notacct &&
                this.wq_fieldpers_notacct == other.wq_fieldpers_notacct && this.gage_visit == other.gage_visit &&
                this.gage_down == other.gage_down && this.tot_discharge_meas == other.tot_discharge_meas && 
                this.plan_discharge_meas == other.plan_discharge_meas && this.plan_indirect_meas == other.plan_indirect_meas &&
                this.rating_extens == other.rating_extens && this.gage_peak_record == other.gage_peak_record && 
                this.plan_rapdepl_gage == other.plan_rapdepl_gage && this.dep_rapdepl_gage == other.dep_rapdepl_gage &&
                this.rec_rapdepl_gage == other.rec_rapdepl_gage && this.lost_rapdepl_gage == other.lost_rapdepl_gage &&
                this.plan_wtrlev_sensor == other.plan_wtrlev_sensor && this.dep_wtrlev_sensor == other.dep_wtrlev_sensor &&
                this.rec_wtrlev_sensor == other.rec_wtrlev_sensor && this.lost_wtrlev_sensor == other.lost_wtrlev_sensor &&
                this.plan_wv_sens == other.plan_wv_sens && this.dep_wv_sens == other.dep_wv_sens &&
                this.rec_wv_sens == other.rec_wv_sens && this.lost_wv_sens == other.lost_wv_sens &&
                this.plan_barometric == other.plan_barometric && this.dep_barometric == other.dep_barometric &&
                this.rec_barometric == other.rec_barometric && this.lost_barometric == other.lost_barometric &&
                this.plan_meteorological == other.plan_meteorological && this.dep_meteorological == other.dep_meteorological &&
                this.rec_meteorological == other.rec_meteorological && this.lost_meteorological == other.lost_meteorological &&
                this.hwm_flagged == other.hwm_flagged && this.hwm_collected == other.hwm_collected &&
                this.qw_discr_samples == other.qw_discr_samples && this.coll_sedsamples == other.coll_sedsamples &&
                (this.notes?.ToLower() ?? "") == (other.notes?.ToLower() ?? "") && this.member_id == other.member_id &&
                this.complete == other.complete && this.yest_fieldpers == other.yest_fieldpers &&
                this.tod_fieldpers == other.tod_fieldpers && this.tmw_fieldpers == other.tmw_fieldpers &&
                this.yest_officepers == other.yest_officepers && this.tod_officepers == other.tod_officepers &&
                this.tmw_officepers == other.tmw_officepers;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as reporting_metrics);
        }
        public override int GetHashCode()
        {
            return (this.report_date.GetHashCode() + this.event_id + this.state + this.member_id).GetHashCode();
        }
    } 
    public partial class roles : IEquatable<roles>
    {
        public bool Equals(roles other)
        {
            return String.Equals(this.role_name.ToLower(), other.role_name.ToLower()) &&
                 String.Equals(this.role_description.ToLower(), other.role_description.ToLower());
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as roles);
        }
        public override int GetHashCode()
        {
            return (this.role_name + this.role_description).GetHashCode();
        }
    } 
    public partial class sensor_brand : IEquatable<sensor_brand>
    {
        public bool Equals(sensor_brand other)
        {
            return String.Equals(this.brand_name.ToLower(), other.brand_name.ToLower());
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as sensor_brand);
        }
        public override int GetHashCode()
        {
            return (this.brand_name).GetHashCode();
        }
    } 
    public partial class sensor_deployment : IEquatable<sensor_deployment>
    {
        public bool Equals(sensor_deployment other)
        {
            return this.sensor_type_id == other.sensor_type_id && this.deployment_type_id == other.deployment_type_id;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as sensor_deployment);
        }
        public override int GetHashCode()
        {
            return (this.sensor_type_id + this.deployment_type_id).GetHashCode();
        }
    } 
    public partial class sensor_type : IEquatable<sensor_type>
    {
        public bool Equals(sensor_type other)
        {
            return String.Equals(this.sensor.ToLower(), other.sensor.ToLower());
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as sensor_type);
        }
        public override int GetHashCode()
        {
            return (this.sensor).GetHashCode();
        }
    } 
    public partial class site_housing : IEquatable<site_housing>
    {
        public bool Equals(site_housing other)
        {
            return this.site_id == other.site_id && this.housing_type_id == other.housing_type_id &&
                this.length == other.length &&
                (this.material?.ToLower() ?? "") == (other.material?.ToLower() ?? "") &&
                (this.notes?.ToLower() ?? "") == (other.notes?.ToLower() ?? "") && this.amount == other.amount;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as site_housing);
        }
        public override int GetHashCode()
        {
            return (this.site_id + this.housing_type_id).GetHashCode();
        }
    } 
    public partial class sites : IEquatable<sites>
    {
        public bool Equals(sites other)
        {
            return String.Equals(this.site_no.ToLower(), other.site_no.ToLower()) &&
                String.Equals(this.site_name.ToLower(), other.site_name.ToLower()) &&
                String.Equals(this.site_description.ToLower(), other.site_description.ToLower()) &&
                (this.address?.ToLower() ?? "") == (other.address?.ToLower() ?? "") &&
                (this.city?.ToLower() ?? "") == (other.city?.ToLower() ?? "") &&
                (this.state_fips) == (other.state_fips) &&
                (this.zip?.ToLower() ?? "") == (other.zip?.ToLower() ?? "") && 
                (this.other_sid?.ToLower() ?? "") == (other.other_sid?.ToLower() ?? "") &&
                (this.county_fips == (other.county_fips)) &&
                String.Equals(this.waterbody.ToLower(), other.waterbody.ToLower()) &&
                this.latitude_dd == other.latitude_dd && this.longitude_dd == other.longitude_dd &&
                this.hdatum_id == other.hdatum_id && this.drainage_area_sqmi == other.drainage_area_sqmi &&
                this.landownercontact_id == other.landownercontact_id && this.priority_id == other.priority_id &&
                (this.zone?.ToLower() ?? "") == (other.zone?.ToLower() ?? "") &&
                (this.is_permanent_housing_installed?.ToLower() ?? "") == (other.is_permanent_housing_installed?.ToLower() ?? "") &&
                (this.usgs_sid?.ToLower() ?? "") == (other.usgs_sid?.ToLower() ?? "") &&
                (this.noaa_sid?.ToLower() ?? "") == (other.noaa_sid?.ToLower() ?? "") &&
                this.hcollect_method_id == other.hcollect_method_id &&
                (this.site_notes?.ToLower() ?? "") == (other.site_notes?.ToLower() ?? "") &&
                (this.safety_notes?.ToLower() ?? "") == (other.safety_notes?.ToLower() ?? "") &&
                (this.access_granted?.ToLower() ?? "") == (other.access_granted?.ToLower() ?? "") &&
                this.member_id == other.member_id && this.sensor_not_appropriate == other.sensor_not_appropriate;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as sites);
        }
        public override int GetHashCode()
        {
            return (this.site_no + this.site_name + this.site_description + this.latitude_dd + this.longitude_dd + this.hdatum_id + this.hcollect_method_id +
                this.state + this.county + this.waterbody + this.member_id).GetHashCode();
        }
    } 
    public partial class sources : IEquatable<sources>
    {
        public bool Equals(sources other)
        {
            return String.Equals(this.source_name.ToLower(), other.source_name.ToLower()) && this.agency_id == other.agency_id;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as sources);
        }
        public override int GetHashCode()
        {
            return (this.source_name + this.agency_id).GetHashCode();
        }
    } 
    public partial class states : IEquatable<states>
    {
        public bool Equals(states other)
        {
            return String.Equals(this.state_name.ToLower(), other.state_name.ToLower()) &&
                String.Equals(this.state_abbrev.ToLower(), other.state_abbrev.ToLower());
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as states);
        }
        public override int GetHashCode()
        {
            return (this.state_name + this.state_abbrev).GetHashCode();
        }
    }//end 
    public partial class status_type : IEquatable<status_type>
    {
        public bool Equals(status_type other)
        {
            return String.Equals(this.status.ToLower(), other.status.ToLower());
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as status_type);
        }
        public override int GetHashCode()
        {
            return (this.status).GetHashCode();
        }
    }
    public partial class vertical_collect_methods : IEquatable<vertical_collect_methods>
    {
        public bool Equals(vertical_collect_methods other)
        {
            return String.Equals(this.vcollect_method.ToLower(), other.vcollect_method.ToLower());
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as vertical_collect_methods);
        }
        public override int GetHashCode()
        {
            return (this.vcollect_method).GetHashCode();
        }
    }
    public partial class vertical_datums : IEquatable<vertical_datums>
    {
        public bool Equals(vertical_datums other)
        {
            return String.Equals(this.datum_name.ToLower(), other.datum_name.ToLower()) &&
                String.Equals(this.datum_abbreviation.ToLower(), other.datum_abbreviation.ToLower());
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as vertical_datums);
        }
        public override int GetHashCode()
        {
            return (this.datum_name + this.datum_abbreviation).GetHashCode();
        }
    }
}
