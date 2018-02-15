using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace STNDB.Test
{
    [TestClass]
    public class STNDBTest
    {
        private string connectionstring = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build().GetConnectionString("stnConnection");

        [TestMethod]
        public void ConnectionTest()
        {
            using (STNDBContext context = new STNDBContext(new DbContextOptionsBuilder<STNDBContext>().UseNpgsql(this.connectionstring).Options))
            {
                try
                {
                    if (!(context.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists()) throw new Exception("db does ont exist");
                }
                catch (Exception ex)
                {
                    Assert.IsTrue(false, ex.Message);
                }
            }
        }
        [TestMethod]
        public void QueryTest()
        {
            using (STNDBContext context = new STNDBContext(new DbContextOptionsBuilder<STNDBContext>().UseNpgsql(this.connectionstring).Options))
            {
                try
                { 
                    var testQuery = context.markers.ToList();
                    Assert.IsNotNull(testQuery, testQuery.Count.ToString());
                }
                catch (Exception ex)
                {
                    Assert.IsTrue(false, ex.Message);
                }
                finally
                {
                }

            }
        }
    }
}
