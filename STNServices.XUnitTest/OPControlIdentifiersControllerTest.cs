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
    public class OPControlIdentifiersTest
    {
        public OPControlIdentifiersController controller { get; private set; }
        public OPControlIdentifiersTest() {
            //Arrange
            controller = new OPControlIdentifiersController(new InMemoryOPControlIdentifiersAgent());
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
            var result = Assert.IsType<EnumerableQuery<op_control_identifier>>(okResult.Value);

            Assert.Equal(2, result.Count());
            Assert.Equal("id2", result.LastOrDefault().identifier);
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
            var result = Assert.IsType<op_control_identifier>(okResult.Value);
            
            Assert.Equal("id1", result.identifier);
        }

        [Fact]
        public async Task Post()
        {
            //Arrange
            var entity = new op_control_identifier() { identifier = "id3", identifier_type = "type3", objective_point_id = 3 };
   

            //Act
            var response = await controller.Post(entity);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response);
            var result = Assert.IsType<op_control_identifier>(okResult.Value);
            

            Assert.Equal("id3", result.identifier);
        }

        [Fact]
        public async Task Put()
        {
            //Arrange
            var get = await controller.Get(1);
            var okgetResult = Assert.IsType<OkObjectResult>(get);
            var entity = Assert.IsType<op_control_identifier>(okgetResult.Value);
            

            var newEntity = new op_control_identifier();
            newEntity.identifier = "ID1";
            newEntity.identifier_type = "type1";
            newEntity.objective_point_id = 1;
            //should test the equals Equatable for all these too
            var huh = entity.Equals(newEntity);

            entity.identifier = "id4";
            //Act
            var response = await controller.Put(1, entity);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response);
            var result = Assert.IsType<op_control_identifier>(okResult.Value);

            Assert.Equal(entity.identifier, result.identifier);
        }

        [Fact]
        public async Task Delete()
        {
            //Act
            await controller.Delete(1);

            var response = await controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response);
            var result = Assert.IsType<EnumerableQuery<op_control_identifier>>(okResult.Value);

            Assert.Equal(1, result.Count());
            Assert.Equal("id2", result.LastOrDefault().identifier);
        }
    }

    public class InMemoryOPControlIdentifiersAgent : ISTNServicesAgent
    {
        private List<op_control_identifier> entityList { get; set; }

        public List<Message> Messages { get; set; }// => throw new NotImplementedException();

        public InMemoryOPControlIdentifiersAgent() {
           this.entityList = new List<op_control_identifier>()
           {
               new op_control_identifier() { op_control_identifier_id = 1, identifier = "id1", identifier_type = "type1", objective_point_id= 1 },
               new op_control_identifier() { op_control_identifier_id = 2, identifier = "id2", identifier_type = "type2", objective_point_id= 2  }
           };                 
        }

        public IQueryable<T> Select<T>() where T : class, new()
        {
            if (typeof(T) == typeof(op_control_identifier))
                return this.entityList.AsQueryable() as IQueryable<T>;

            throw new Exception("not of correct type");
        }

        public Task<T> Find<T>(int pk) where T : class, new()
        {
            if (typeof(T) == typeof(op_control_identifier))
                return Task.Run(()=> { return entityList.Find(i => i.op_control_identifier_id == pk) as T; });

            throw new Exception("not of correct type");
        }

        public Task<T> Add<T>(T item) where T : class, new()
        {
            if (typeof(T) == typeof(op_control_identifier))
            {
                entityList.Add(item as op_control_identifier);
            }
            return Task.Run(()=> { return item; });
        }

        public Task<IEnumerable<T>> Add<T>(List<T> items) where T : class, new()
        {
            if (typeof(T) == typeof(op_control_identifier))
            {
                entityList.AddRange(items.Cast<op_control_identifier>());
            }
            return Task.Run(() => { return entityList.Cast<T>(); });
        }

        public Task<T> Update<T>(int pkId, T item) where T : class, new()
        {
            if (typeof(T) == typeof(op_control_identifier))
            {
                var index = this.entityList.FindIndex(x => x.op_control_identifier_id == pkId);
                (item as op_control_identifier).op_control_identifier_id = pkId;
                this.entityList[index] = item as op_control_identifier;
                return Task.Run(() => { return this.entityList[index] as T; });
            }
            else
                throw new Exception("not of correct type");
        }

        public Task Delete<T>(T item) where T : class, new()
        {
            if (typeof(T) == typeof(op_control_identifier))
            {                
                return Task.Run(()=> { this.entityList.Remove(item as op_control_identifier); });
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
