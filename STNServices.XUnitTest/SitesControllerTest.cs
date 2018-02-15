//Unit testing involves testing a part of an app in isolation from its infrastructure and dependencies. 
//When unit testing controller logic, only the contents of a single action is tested, not the behavior of 
//its dependencies or of the framework itself. As you unit test your controller actions, make sure you focus 
//only on its behavior. A controller unit test avoids things like filters, routing, or model binding. By focusing 
//on testing just one thing, unit tests are generally simple to write and quick to run. A well-written set of unit 
//tests can be run frequently without much overhead. However, unit tests do not detect issues in the interaction 
//between components, which is the purpose of integration testing.

using System;
using Xunit;
using System.Threading.Tasks;
using STNAgent;
using STNServices.Controllers;
using Microsoft.AspNetCore.Mvc;
using STNDB.Resources;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using STNAgent.Resources;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using WiM.Security.Authentication.Basic;
using WiM.Resources;
using Microsoft.Extensions.Configuration;

namespace STNServices.XUnitTest
{
    public class SitesTest
    {
        public SitesController controller { get; private set; }
        public SitesTest() {
            IConfiguration config = null;
            //Arrange
            controller = new SitesController(new InMemorySitesAgent(), config);
            //must set explicitly for tests to work
            controller.ObjectValidator = new InMemoryModelValidator();


        }
        [Fact]
        public async Task GetAll()
        {
            //Act
            var response = await controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response);
            var result = Assert.IsType<EnumerableQuery<sites>>(okResult.Value);
            
            Assert.Equal(2, result.Count());
            Assert.Equal("a2", result.LastOrDefault().site_no);
        }

        [Fact]
        public async Task Get()
        {
            //Arrange
            var id = 1;
            
            //Act
            var response = await controller.Get(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response);
            var result = Assert.IsType<sites>(okResult.Value);
            
            Assert.Equal("a1", result.site_no);
        }

        [Fact]
        public async Task Post()
        {
            //Arrange
            var entity = new sites() { site_id = 5, site_no = "a3", site_name = "nameA1", site_description = "test12", state = "WI", county = "Fort", waterbody = "test", latitude_dd = 35, longitude_dd = -91, hdatum_id = 3, hcollect_method_id = 1, member_id = 44 };


            //Act
            var response = await controller.Post(entity);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response);
            var result = Assert.IsType<sites>(okResult.Value);
            

            Assert.Equal("a3", result.site_no);
        }

        [Fact]
        public async Task Put()
        {
            //Arrange
            var get = await controller.Get(1);
            var okgetResult = Assert.IsType<OkObjectResult>(get);

            var entity = Assert.IsType<sites>(okgetResult.Value);
            var newEntity = new sites() { site_no = "A1", site_name = "name1", site_description = "test12", state = "WI", county = "Fort", waterbody = "test", latitude_dd = 34, longitude_dd = -90, hdatum_id = 3, hcollect_method_id = 1, member_id = 44 };
            
            //should test the equals Equatable for all these too
            var huh = entity.Equals(newEntity);

            entity.site_no = "A5";
            //Act
            var response = await controller.Put(1, entity);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response);
            var result = Assert.IsType<sites>(okResult.Value);

            Assert.Equal(entity.site_no, result.site_no);
        }

        [Fact]
        public async Task Delete()
        {
            //Act
            await controller.Delete(1);

            var response = await controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response);
            var result = Assert.IsType<EnumerableQuery<sites>>(okResult.Value);

            Assert.Equal(1, result.Count());
            Assert.Equal(2, result.LastOrDefault().site_id);
        }
    }

    public class InMemorySitesAgent : ISTNServicesAgent
    {
        private List<sites> entityList { get; set; }

        public List<Message> Messages { get; set; }// => throw new NotImplementedException();
        IConfiguration _config;
        public InMemorySitesAgent() {
           this.entityList = new List<sites>()
           {
               new sites() { site_id = 1, site_no= "a1", site_name = "name1", site_description = "test12", state = "WI", county = "Fort", waterbody = "test", latitude_dd = 34, longitude_dd = -90, hdatum_id = 3, hcollect_method_id = 1, member_id = 44},
               new sites() { site_id = 2, site_no= "a2", site_name = "name2", site_description= "test23", state = "MN", county = "Scott", waterbody = "test1", latitude_dd = 45,  longitude_dd = -88, hdatum_id = 2, hcollect_method_id = 1, member_id = 22 }

           };                    
        }

        public IQueryable<T> Select<T>() where T : class, new()
        {
            if (typeof(T) == typeof(sites))
                return this.entityList.AsQueryable() as IQueryable<T>;

            throw new Exception("not of correct type");
        }

        public Task<T> Find<T>(int pk) where T : class, new()
        {
            if (typeof(T) == typeof(sites))
                return Task.Run(()=> { return entityList.Find(i => i.site_id == pk) as T; });

            throw new Exception("not of correct type");
        }

        public Task<T> Add<T>(T item) where T : class, new()
        {
            if (typeof(T) == typeof(sites))
            {
                entityList.Add(item as sites);
            }
            return Task.Run(()=> { return item; });
        }

        public Task<IEnumerable<T>> Add<T>(List<T> items) where T : class, new()
        {
            if (typeof(T) == typeof(sites))
            {
                entityList.AddRange(items.Cast<sites>());
            }
            return Task.Run(() => { return entityList.Cast<T>(); });
        }

        public Task<T> Update<T>(int pkId, T item) where T : class, new()
        {
            if (typeof(T) == typeof(sites))
            {
                var index = this.entityList.FindIndex(x => x.site_id == pkId);
                (item as sites).site_id = pkId;
                this.entityList[index] = item as sites;
                return Task.Run(() => { return this.entityList[index] as T; });
            }
            else
                throw new Exception("not of correct type");
        }

        public Task Delete<T>(T item) where T : class, new()
        {
            if (typeof(T) == typeof(sites))
            {                
                return Task.Run(()=> { this.entityList.Remove(item as sites); });
            }

            else
                throw new Exception("not of correct type");
        }


        #region interface requirements
        public IBasicUser GetUserByUsername(string username)
        {
            throw new NotImplementedException();
        }
        public IQueryable<roles> GetRoles()
        {
            throw new NotImplementedException();
        }
        public IQueryable<data_file> GetFilterDataFiles(string approved, string eventId, string state, string counties)
        {
            throw new NotImplementedException();
        }
        public IQueryable<T> getTable<T>(object[] args) where T : class, new()
        {
            throw new NotImplementedException();
        }
        public IQueryable<events> GetFiltedEvents(string date, string eventTypeId, string stateName)
        {
            throw new NotImplementedException();
        }
        public DateTime? ValidDate(string date)
        {
            throw new NotImplementedException();
        }
        public List<hwm> GetFilterHWMs(string eventIds, string eventTypeIDs, string eventStatusID, string states, string counties, string hwmTypeIDs, string hwmQualIDs, string hwmEnvironment, string surveyComplete, string stillWater)
        {
            throw new NotImplementedException();
        }

        public List<instrument> GetFiltedInstruments(string Event, string EventType, string EventStatus, string States, string County, string CurrentStatus, string CollectionCondition, string SensorType, string DeploymentType)
        {
            throw new NotImplementedException();
        }

        public List<sensor_view> GetSensorView(string ViewType, string Event, string EventType, string EventStatus, string States, string County, string CurrentStatus, string CollectionCondition, string SensorType, string DeploymentType)
        {
            throw new NotImplementedException();
        }

        public List<peak_summary> GetFiltedPeaks(string @event, string eventType, string eventStatus, string states, string county, string startDate, string endDate)
        {
            throw new NotImplementedException();
        }

        public List<ReportResource> GetFiltedReportsModel(int ev, string state, string date)
        {
            throw new NotImplementedException();
        }

        public List<reporting_metrics> GetFiltedReports(string ev, string date, string states)
        {
            throw new NotImplementedException();
        }

        public List<sites> GetFilterSites(string @event, string state, string sensorType, string networkName, string oPDefined, string hWMOnly, string hWMSurveyed, string sensorOnly, string rDGOnly)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
