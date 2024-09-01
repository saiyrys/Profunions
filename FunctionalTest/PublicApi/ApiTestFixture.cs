using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Profunion.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Profunion.Core.Entitities.UserModels;

namespace FunctionalTest.PublicApi
{
    public class TestApiApplication : WebApplicationFactory<Program>
    {
        private readonly string _enviroment = "Testing";

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.UseEnvironment(_enviroment);

            builder.ConfigureServices(services =>
            {
                services.AddScoped(sp =>
                {
                   return  new DbContextOptionsBuilder<DataContext>()
                    .UseInMemoryDatabase("DbTest")
                    .UseApplicationServiceProvider(sp)
                    .Options;


                });
            });

            return base.CreateHost(builder);
        }
        
    }
}