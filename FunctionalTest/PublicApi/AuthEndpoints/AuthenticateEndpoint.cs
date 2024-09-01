using Microsoft.AspNetCore.Mvc.Testing;
using MySqlX.XDevAPI;
using Profunion.Anexs.Dto.AuthDto;
using System.Text;
using System.Text.Json;


namespace FunctionalTest.PublicApi.AuthEndpoints
{
    [Collection("Sequential")]
    public class AuthenticateEndpoint : IClassFixture<TestApiApplication>
    {
        private readonly HttpClient Client;
        
        public AuthenticateEndpoint(TestApiApplication factory)
        {
            Client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            }); 
        }

        [Fact]
        public async Task ReturnsSignInScreenOnGet()
        {
            var response = await Client.GetAsync("/api/event?isActive=false");
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.Contains("items", stringResponse);
        }
    }
}
