using System;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Backend.Services;
using Microsoft.EntityFrameworkCore;
using Siesa.SDK.Backend.Access;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Castle.Core.Configuration;
using Siesa.SDK.Shared.Configurations;
using Microsoft.Extensions.Options;

namespace Siesa.SDK.Utils.Test.Utils
{
    public class TestUtils
    {
        public static T GetBusiness<T>()
        {
            var serviceProvider = GetProvider<T>();

            dynamic Business = ActivatorUtilities.CreateInstance(serviceProvider, typeof(T));
            Business.SetProvider(serviceProvider);
            return Business;
        }

        public static IServiceProvider GetProvider<T>()
        {
            var ServiceConf = Options.Create(new ServiceConfiguration()); 

            //Instanciar los servicios que se necesitan para el test
            var auth = new Mock<AuthenticationService>();
            var tenant = new Mock<ITenantProvider>();
            var resourceManager = new Mock<IResourceManager>();
            var backRouter = new Mock<BackendRouterService>(ServiceConf);
            var email = new Mock<EmailService>(backRouter.Object, auth.Object);
            
            Mock<IConfigurationSection> mockSection = new Mock<IConfigurationSection>();
            mockSection.Setup(x=>x.Value).Returns("false");

            Mock<Microsoft.Extensions.Configuration.IConfiguration> mockConfig = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            mockConfig.Setup(x=>x.GetSection("AWS:UseS3")).Returns(mockSection.Object);
            
            var mockLogger = new Mock<ILogger<T>>();
            mockLogger.Setup(
                m => m.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.IsAny<object>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<object, Exception, string>>()));

            var mockLoggerFactory = new Mock<ILoggerFactory>();
            mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(() => mockLogger.Object);

            var mockDbFactory = new Mock<IDbContextFactory<SDKContext>>();
                mockDbFactory.Setup(f => f.CreateDbContext())
                    .Returns(() => new SDKContext(new DbContextOptionsBuilder<SDKContext>()
                        .UseInMemoryDatabase("InMemoryTest")
                        .Options));
            var services = new ServiceCollection();


            //Agregar las instancias simuladas a ServicesCollection
            services.AddScoped<ITenantProvider>(sp => tenant.Object);
            services.AddScoped<IAuthenticationService>(sp => auth.Object);
            services.AddScoped<IBackendRouterService>(sp => backRouter.Object);
            services.AddScoped<EmailService>(sp => email.Object);
            services.AddScoped<Microsoft.Extensions.Configuration.IConfiguration>(sp => mockConfig.Object);
            services.AddScoped<IResourceManager>(sp => resourceManager.Object);
            services.AddScoped<Microsoft.Extensions.Logging.ILoggerFactory>(sp => mockLoggerFactory.Object);
            services.AddScoped<IDbContextFactory<SDKContext>>(sp => mockDbFactory.Object);

            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider;
        }
    }
}