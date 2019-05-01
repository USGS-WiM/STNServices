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
    public class DataFilesTest
    {
        public DataFilesController controller { get; private set; }
        public DataFilesTest() {
            //Arrange
            controller = new DataFilesController(new InMemoryDataFilesAgent());
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
            var result = Assert.IsType<EnumerableQuery<data_file>>(okResult.Value);

            Assert.Equal(2, result.Count());
            Assert.Equal(new DateTime(2016, 02, 15), result.LastOrDefault().good_start);
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
            var result = Assert.IsType<data_file>(okResult.Value);
            
            Assert.Equal(new DateTime(2015, 01, 14), result.good_start);
        }

        [Fact]
        public async Task Post() //not working because loggedinmember == null. 
        {
            //Arrange
            var entity = new data_file() {
                good_start = new DateTime(2017, 01, 14),
                good_end = new DateTime(2017, 01, 25),
                processor_id = 2,
                instrument_id = 155,
                collect_date = new DateTime(2017, 08, 16)
            };
   

            //Act
            var response = await controller.Post(entity);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response);
            var result = Assert.IsType<data_file>(okResult.Value);
            

            Assert.Equal(new DateTime(2017, 01, 14), result.good_start);
        }

        [Fact]
        public async Task Put() //not working because loggedinmember == null. 
        {
            //Arrange
            var get = await controller.Get(1);
            var okgetResult = Assert.IsType<OkObjectResult>(get);
            var entity = Assert.IsType<data_file>(okgetResult.Value);
            entity.good_start = new DateTime(2015, 03, 14);
            //Act
            var response = await controller.Put(1, entity);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response);
            var result = Assert.IsType<data_file>(okResult.Value);

            Assert.Equal(entity.good_start, result.good_start);
        }

        [Fact]
        public async Task Delete()
        {
            //Act
            await controller.Delete(1);

            var response = await controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response);
            var result = Assert.IsType<EnumerableQuery<data_file>>(okResult.Value);

            Assert.Equal(1, result.Count());
            Assert.Equal(new DateTime(2016, 02, 15), result.LastOrDefault().good_start);
            Assert.Equal(new DateTime(2015, 02, 28), result.LastOrDefault().good_end);            
        }
    }

    public class InMemoryDataFilesAgent : ISTNServicesAgent
    {
        private List<data_file> entityList { get; set; }

        public List<Message> Messages { get; set; }// => throw new NotImplementedException();

        public InMemoryDataFilesAgent() {
           this.entityList = new List<data_file>()
           {
               new data_file() {
                   data_file_id = 1, good_start= new DateTime(2015, 01, 14), good_end= new DateTime(2015, 01, 25),
               processor_id = 1, instrument_id = 123, collect_date = new DateTime(2017, 08, 16), approval_id = 12 },
               new data_file() { data_file_id = 2, good_start= new DateTime(2016, 02, 15), good_end= new DateTime(2015, 02, 28),
               processor_id = 1, instrument_id = 222, collect_date = new DateTime(2017, 03, 05),approval_id = 33 }
           };                 
        }

        public IQueryable<T> Select<T>() where T : class, new()
        {
            if (typeof(T) == typeof(data_file))
                return this.entityList.AsQueryable() as IQueryable<T>;

            throw new Exception("not of correct type");
        }

        public Task<T> Find<T>(int pk) where T : class, new()
        {
            if (typeof(T) == typeof(data_file))
                return Task.Run(()=> { return entityList.Find(i => i.data_file_id == pk) as T; });

            throw new Exception("not of correct type");
        }

        public Task<T> Add<T>(T item) where T : class, new()
        {
            if (typeof(T) == typeof(data_file))
            {
                entityList.Add(item as data_file);
            }
            return Task.Run(()=> { return item; });
        }

        public Task<IEnumerable<T>> Add<T>(List<T> items) where T : class, new()
        {
            if (typeof(T) == typeof(data_file))
            {
                entityList.AddRange(items.Cast<data_file>());
            }
            return Task.Run(() => { return entityList.Cast<T>(); });
        }

        public Task<T> Update<T>(int pkId, T item) where T : class, new()
        {
            if (typeof(T) == typeof(data_file))
            {
                var index = this.entityList.FindIndex(x => x.data_file_id == pkId);
                (item as data_file).data_file_id = pkId;
                this.entityList[index] = item as data_file;
                return Task.Run(() => { return this.entityList[index] as T; });
            }
            else
                throw new Exception("not of correct type");
        }

        public Task Delete<T>(T item) where T : class, new()
        {
            if (typeof(T) == typeof(data_file))
            {                
                return Task.Run(()=> { this.entityList.Remove(item as data_file); });
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
