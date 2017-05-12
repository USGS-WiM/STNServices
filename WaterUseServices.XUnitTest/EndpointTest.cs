using System;
using Xunit;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using System.Threading.Tasks;
using WaterUseServices;

namespace WaterUseServices.XUnitTest
{
    public class EndpointTest
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;
        public EndpointTest()
        {
            // Arrange
            _server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task Test1()
        {
            //Act
            var response = await _client.GetAsync("/wateruse/roles");
            response.EnsureSuccessStatusCode();
        }
    }
}
