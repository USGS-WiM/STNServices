﻿//Unit testing involves testing a part of an app in isolation from its infrastructure and dependencies. 
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

namespace STNServices.XUnitTest
{
    public class LandOwnersTest
    {
        public LandOwnersController controller { get; private set; }
        public LandOwnersTest() {
            //Arrange
            controller = new LandOwnersController(new InMemoryLandOwnersAgent());
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
            var result = Assert.IsType<EnumerableQuery<landownercontact>>(okResult.Value);

            Assert.Equal(2, result.Count());
            Assert.Equal("Jane", result.LastOrDefault().fname);
            Assert.Equal("Smith", result.LastOrDefault().lname);
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
            var result = Assert.IsType<landownercontact>(okResult.Value);
            
            Assert.Equal("Tom", result.fname);
            Assert.Equal("Jones", result.lname);
        }

        [Fact]
        public async Task Post()
        {
            //Arrange
            var entity = new landownercontact() { fname = "Test", lname = "Post" };
   

            //Act
            var response = await controller.Post(entity);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response);
            var result = Assert.IsType<landownercontact>(okResult.Value);
            

            Assert.Equal("Test", result.fname);
            Assert.Equal("Post", result.lname);
        }

        [Fact]
        public async Task Put()
        {
            //Arrange
            var get = await controller.Get(1);
            var okgetResult = Assert.IsType<OkObjectResult>(get);
            var entity = Assert.IsType<landownercontact>(okgetResult.Value);
            

            var newEntity = new landownercontact();
            newEntity.fname = "jane";
            newEntity.lname = "smith";

            //should test the equals Equatable for all these too
            var huh = entity.Equals(newEntity);

            entity.fname = "test";
            entity.lname = "Edit";
            // Act
            var response = await controller.Put(1, entity);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response);
            var result = Assert.IsType<landownercontact>(okResult.Value);

            Assert.Equal(entity.fname, result.fname);
        }

        [Fact]
        public async Task Delete()
        {
            //Act
            await controller.Delete(1);

            var response = await controller.Get(); 

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response);
            var result = Assert.IsType<EnumerableQuery<landownercontact>>(okResult.Value);

            Assert.Equal(1, result.Count());
            Assert.Equal("Jane", result.LastOrDefault().fname);
            Assert.Equal("Smith", result.LastOrDefault().lname);
        }
    }

    public class InMemoryLandOwnersAgent : ISTNServicesAgent
    {
        private List<landownercontact> entityList { get; set; }

        public List<Message> Messages { get; set; } 

        public InMemoryLandOwnersAgent() {
           this.entityList = new List<landownercontact>()
           {
               new landownercontact() { landownercontactid = 1, fname= "Tom", lname = "Jones" },
               new landownercontact() { landownercontactid = 2, fname= "Jane", lname = "Smith"  }
           };                 
        }

        public IQueryable<T> Select<T>() where T : class, new()
        {
            if (typeof(T) == typeof(landownercontact))
                return this.entityList.AsQueryable() as IQueryable<T>;

            throw new Exception("not of correct type");
        }

        public Task<T> Find<T>(int pk) where T : class, new()
        {
            if (typeof(T) == typeof(landownercontact))
                return Task.Run(()=> { return entityList.Find(i => i.landownercontactid == pk) as T; });

            throw new Exception("not of correct type");
        }

        public Task<T> Add<T>(T item) where T : class, new()
        {
            if (typeof(T) == typeof(landownercontact))
            {
                entityList.Add(item as landownercontact);
            }
            return Task.Run(()=> { return item; });
        }

        public Task<IEnumerable<T>> Add<T>(List<T> items) where T : class, new()
        {
            if (typeof(T) == typeof(landownercontact))
            {
                entityList.AddRange(items.Cast<landownercontact>());
            }
            return Task.Run(() => { return entityList.Cast<T>(); });
        }

        public Task<T> Update<T>(int pkId, T item) where T : class, new()
        {
            if (typeof(T) == typeof(landownercontact))
            {
                var index = this.entityList.FindIndex(x => x.landownercontactid == pkId);
                (item as landownercontact).landownercontactid = pkId;
                this.entityList[index] = item as landownercontact;
                return Task.Run(() => { return this.entityList[index] as T; });
            }
            else
                throw new Exception("not of correct type");
        }

        public Task Delete<T>(T item) where T : class, new()
        {
            if (typeof(T) == typeof(landownercontact))
            {                
                return Task.Run(()=> { this.entityList.Remove(item as landownercontact); });
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

        public List<STNAgent.Resources.sensor_view> GetSensorView(string ViewType, string Event, string EventType, string EventStatus, string States, string County, string CurrentStatus, string CollectionCondition, string SensorType, string DeploymentType)
        {
            throw new NotImplementedException();
        }

        public List<peak_summary> GetFilteredPeaks(string @event, string eventType, string eventStatus, string states, string county, string startDate, string endDate)
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
