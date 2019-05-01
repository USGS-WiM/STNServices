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
    public class ObjectivePointsTest
    {
        public ObjectivePointsController controller { get; private set; }
        public ObjectivePointsTest() {
            //Arrange
            controller = new ObjectivePointsController(new InMemoryObjectivePointsAgent());
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
            var result = Assert.IsType<EnumerableQuery<objective_point>>(okResult.Value);

            Assert.Equal(2, result.Count());
            Assert.Equal("SWaTH", result.LastOrDefault().name);
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
            var result = Assert.IsType<objective_point>(okResult.Value);
            
            Assert.Equal("Not Defined", result.name);
        }

        [Fact]
        public async Task Post()
        {
            //Arrange
            var entity = new objective_point() { name = "TestPost", description = "testpost", date_established = new DateTime(), site_id = 2, vdatum_id = 22, latitude_dd = 44.44, longitude_dd = -89.33, op_type_id = 2 };
   

            //Act
            var response = await controller.Post(entity);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response);
            var result = Assert.IsType<objective_point>(okResult.Value);
            

            Assert.Equal("TestPost", result.name);
        }

        [Fact]
        public async Task Put()
        {
            //Arrange
            var get = await controller.Get(1);
            var okgetResult = Assert.IsType<OkObjectResult>(get);
            var entity = Assert.IsType<objective_point>(okgetResult.Value);
            

            var newEntity = new objective_point();
            newEntity.name = "not Defined";
            newEntity.description = "something";
            newEntity.date_established = new DateTime();
            newEntity.site_id = 1;
            newEntity.vdatum_id = 11;
            newEntity.latitude_dd = 43.33;
            newEntity.longitude_dd = -88.22;
            newEntity.op_type_id = 1;
            //should test the equals Equatable for all these too
            var huh = entity.Equals(newEntity);

            entity.name = "testEdit";
            //Act
            var response = await controller.Put(1, entity);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response);
            var result = Assert.IsType<objective_point>(okResult.Value);

            Assert.Equal(entity.name, result.name);
        }

        [Fact]
        public async Task Delete()
        {
            //Act
            await controller.Delete(1);

            var response = await controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response);
            var result = Assert.IsType<EnumerableQuery<objective_point>>(okResult.Value);

            Assert.Equal(2, result.Count());
            Assert.Equal("SWaTH", result.LastOrDefault().name);
        }
    }

    public class InMemoryObjectivePointsAgent : ISTNServicesAgent
    {
        private List<objective_point> entityList { get; set; }

        public List<Message> Messages { get; set; }// => throw new NotImplementedException();

        public InMemoryObjectivePointsAgent() {
           this.entityList = new List<objective_point>()
           {
               new objective_point() { objective_point_id = 1, name= "Not Defined", description="something", date_established = new DateTime(), site_id = 1, vdatum_id = 11, latitude_dd = 43.33, longitude_dd = -88.22, op_type_id = 1},
               new objective_point() { objective_point_id = 2, name= "SWaTH", description = "something else", date_established = new DateTime(),site_id = 2 , vdatum_id = 22, latitude_dd = 44.44, longitude_dd = -89.33, op_type_id=2 }
           };                 
        }

        public IQueryable<T> Select<T>() where T : class, new()
        {
            if (typeof(T) == typeof(objective_point))
                return this.entityList.AsQueryable() as IQueryable<T>;

            throw new Exception("not of correct type");
        }

        public Task<T> Find<T>(int pk) where T : class, new()
        {
            if (typeof(T) == typeof(objective_point))
                return Task.Run(()=> { return entityList.Find(i => i.objective_point_id == pk) as T; });

            throw new Exception("not of correct type");
        }

        public Task<T> Add<T>(T item) where T : class, new()
        {
            if (typeof(T) == typeof(objective_point))
            {
                entityList.Add(item as objective_point);
            }
            return Task.Run(()=> { return item; });
        }

        public Task<IEnumerable<T>> Add<T>(List<T> items) where T : class, new()
        {
            if (typeof(T) == typeof(objective_point))
            {
                entityList.AddRange(items.Cast<objective_point>());
            }
            return Task.Run(() => { return entityList.Cast<T>(); });
        }

        public Task<T> Update<T>(int pkId, T item) where T : class, new()
        {
            if (typeof(T) == typeof(objective_point))
            {
                var index = this.entityList.FindIndex(x => x.objective_point_id == pkId);
                (item as objective_point).objective_point_id = pkId;
                this.entityList[index] = item as objective_point;
                return Task.Run(() => { return this.entityList[index] as T; });
            }
            else
                throw new Exception("not of correct type");
        }

        public Task Delete<T>(T item) where T : class, new()
        {
            if (typeof(T) == typeof(objective_point))
            {                
                return Task.Run(()=> { this.entityList.Remove(item as objective_point); });
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
        public IQueryable<events> GetFilteredEvents(string date, string eventTypeId, string stateName)
        {
            throw new NotImplementedException();
        }
        public DateTime? ValidDate(string date)
        {
            throw new NotImplementedException();
        }
        public List<hwm> GetFilteredHWMs(string eventIds, string eventTypeIDs, string eventStatusID, string states, string counties, string hwmTypeIDs, string hwmQualIDs, string hwmEnvironment, string surveyComplete, string stillWater)
        {
            throw new NotImplementedException();
        }

        public List<instrument> GetFilteredInstruments(string Event, string EventType, string EventStatus, string States, string County, string CurrentStatus, string CollectionCondition, string SensorType, string DeploymentType)
        {
            throw new NotImplementedException();
        }

        public List<sensor_view> GetSensorView(string ViewType, string Event, string EventType, string EventStatus, string States, string County, string CurrentStatus, string CollectionCondition, string SensorType, string DeploymentType)
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
