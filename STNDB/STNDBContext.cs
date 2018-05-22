//------------------------------------------------------------------------------
//----- DB Context ---------------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2017 WiM - USGS

//    authors:  Jeremy K. Newson USGS Web Informatics and Mapping
//              
//  
//   purpose:   Resonsible for interacting with Database 
//
//discussion:   The primary class that is responsible for interacting with data as objects. 
//              The context class manages the entity objects during run time, which includes 
//              populating objects with data from a database, change tracking, and persisting 
//              data to the database.
//              
//
//   

using Microsoft.EntityFrameworkCore;
using STNDB.Resources;
using Microsoft.EntityFrameworkCore.Metadata;
//specifying the data provider and connection string
namespace STNDB
{
    public class STNDBContext:DbContext
    {
        public virtual DbSet<county> counties { get; set; }
        public virtual DbSet<agency> agencies { get; set; }
        public virtual DbSet<approval> approval { get; set; }
        public virtual DbSet<contact> contact { get; set; }
        public virtual DbSet<contact_type> contact_type { get; set; }
        public virtual DbSet<data_file> data_file { get; set; }
        public virtual DbSet<deployment_priority> deployment_priority { get; set; }
        public virtual DbSet<deployment_type> deployment_type { get; set; }
        public virtual DbSet<event_status> event_status { get; set; }
        public virtual DbSet<event_type> event_type { get; set; }
        public virtual DbSet<events> events { get; set; }
        public virtual DbSet<file_type> file_type { get; set; }
        public virtual DbSet<file> files { get; set; }
        public virtual DbSet<horizontal_collect_methods> horizontal_collect_methods { get; set; }
        public virtual DbSet<horizontal_datums> horizontal_datums { get; set; }
        public virtual DbSet<housing_type> housing_type { get; set; }
        public virtual DbSet<hwm> hwm { get; set; }
        public virtual DbSet<hwm_qualities> hwm_qualities { get; set; }
        public virtual DbSet<hwm_types> hwm_types { get; set; }
        public virtual DbSet<instr_collection_conditions> instr_collection_conditions { get; set; }
        public virtual DbSet<instrument> instrument { get; set; }
        public virtual DbSet<instrument_status> instrument_status { get; set; }
        public virtual DbSet<landownercontact> landownercontact { get; set; }
        public virtual DbSet<markers> markers { get; set; }
        public virtual DbSet<members> members { get; set; }
        public virtual DbSet<network_name> network_name { get; set; }
        public virtual DbSet<network_name_site> network_name_site { get; set; }
        public virtual DbSet<network_type> network_type { get; set; }
        public virtual DbSet<network_type_site> network_type_site { get; set; }
        public virtual DbSet<objective_point> objective_point { get; set; }
        public virtual DbSet<objective_point_type> objective_point_type { get; set; }
        public virtual DbSet<op_control_identifier> op_control_identifier { get; set; }
        public virtual DbSet<op_measurements> op_measurements { get; set; }
        public virtual DbSet<op_quality> op_quality { get; set; }
        public virtual DbSet<peak_summary> peak_summary { get; set; }
        public virtual DbSet<reporting_metrics> reporting_metrics { get; set; }
        public virtual DbSet<reportmetric_contact> reportmetric_contact { get; set; }
        public virtual DbSet<roles> roles { get; set; }
        public virtual DbSet<sensor_brand> sensor_brand { get; set; }
        public virtual DbSet<sensor_deployment> sensor_deployment { get; set; }
        public virtual DbSet<sensor_type> sensor_type { get; set; }
        public virtual DbSet<site_housing> site_housing { get; set; }
        public virtual DbSet<sites> sites { get; set; }
        public virtual DbSet<sources> sources { get; set; }
        public virtual DbSet<states> states { get; set; }
        public virtual DbSet<status_type> status_type { get; set; }
        public virtual DbSet<vertical_collect_methods> vertical_collect_methods { get; set; }
        public virtual DbSet<vertical_datums> vertical_datums { get; set; }

        public STNDBContext() : base()
        {
        }
        public STNDBContext(DbContextOptions<STNDBContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {          
            // handle 2 members related to a hwm
            modelBuilder.Entity<hwm>().HasOne(m => m.flag_member)
                .WithMany(m => m.flag_memberHWMs)
                .HasForeignKey(p => p.flag_member_id);                

            modelBuilder.Entity<hwm>().HasOne(m => m.survey_member)
                .WithMany(m => m.survey_memberHWMs)
                .HasForeignKey(p => p.survey_member_id);


            // Cascade delete handlers
            
            // HWM --> HWM files
            //modelBuilder.Entity<file>()
            //    .HasOne(h => h.hwm)
            //    .WithMany(f => f.files)
            //    .OnDelete(DeleteBehavior.Cascade); 


            // instruments --> instrument_status
            //modelBuilder.Entity<instrument_status>()
            //    .HasOne(p => p.instrument)
            //    .WithMany(b => b.instrument_status)
            //    .OnDelete(DeleteBehavior.Cascade);

            // ObjectivePoints --> opFiles

            //modelBuilder.Entity<file>()
            //    .HasOne(o => o.objective_point)
            //    .WithMany(f => f.files)
            //    .OnDelete(DeleteBehavior.Cascade);

            // ObjectivePoints RemoveFileItems? wat dat


            base.OnModelCreating(modelBuilder);             
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }
    }
}
