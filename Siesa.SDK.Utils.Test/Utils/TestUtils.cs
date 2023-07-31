using System;
using Siesa.SDK.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Siesa.SDK.Backend.Access;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Castle.Core.Configuration;
using Siesa.SDK.Shared.Configurations;
using Microsoft.Extensions.Options;
using Siesa.SDK.Backend.Services;
using Siesa.SDK.Shared.Criptography;
using System.Collections.Generic;
using Siesa.SDK.Shared.DTOS;

namespace Siesa.SDK.Utils.Test
{
    public class TestUtils
    {
        public static T GetBusiness<T>(Type DbContext)
        {
            var serviceProvider = GetProvider<T>(DbContext);

            dynamic Business = ActivatorUtilities.CreateInstance(serviceProvider, typeof(T));
            Business.SetProvider(serviceProvider);
            return Business;
        }

        public static IServiceProvider GetProvider<T>(Type _tDbContext)
        {
            var ServiceConf = Options.Create(new ServiceConfiguration()); 

            //Instanciar los servicios que se necesitan para el test
            var auth = new Mock<IAuthenticationService>();
            var sdkjwt = new Mock<ISDKJWT>();
            var tenant = new Mock<ITenantProvider>();
            var resourceManager = new Mock<IResourceManager>();
            var backRouter = new Mock<BackendRouterService>(ServiceConf);
            var email = new Mock<EmailService>(backRouter.Object, auth.Object);

            //Configure Mock Services:
            var UserData = GetUser();
            auth.Setup(x => x.User).Returns(UserData);

            var tenantOption = new SDKDbConnection()
            {
                Rowid = 1,
                Name = "Tenant",
                ConnectionString = "InMemoryTest",
                ProviderName = EnumDBType.InMemory,
                LogoUrl = "LogoUrl",
                StyleUrl = "StyleUrl",
                IsTest = true
            };

            tenant.Setup(x => x.GetTenant()).Returns(tenantOption);

            sdkjwt.Setup(x => x.Generate(It.IsAny<object>(), It.IsAny<long>()))
            .Returns((object obj, long min) =>
            {
                return "jwt_token_generated";
            });

            // Configurar el método Validate
            // sdkjwt.Setup(x => x.Validate<T>(It.IsAny<string>()))
            //     .Returns((string token) =>
            //     {   
            //     });
                    
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
//(new DbContextOptionsBuilder<SDKContext>().UseInMemoryDatabase("InMemoryTest").Options));
            var mockDbFactory = new Mock<IDbContextFactory<SDKContext>>();
                mockDbFactory.Setup(f => f.CreateDbContext())
                    .Returns(() =>  (SDKContext)Activator.CreateInstance(_tDbContext, new DbContextOptionsBuilder<SDKContext>().UseInMemoryDatabase("InMemoryTest").Options));

           
            
            var services = new ServiceCollection();
            //Agregar las instancias simuladas a ServicesCollection
            services.AddScoped<ISDKJWT>(sp => sdkjwt.Object);
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

        public static JwtUserData GetUser()
        {
            var UserTest = new JwtUserData() 
            { 
                Rowid = 1,
                Path = "Path",
                PasswordRecoveryEmail = "TestEmail", 
                Name = "Test User",
                Id = "UserTest",
                Description = "Usuario de Prueba",
                RowidCulture = 1,
                Roles = new List<SessionRol>() { new SessionRol() { Rowid = 1, Name = "RolTest" } },
                RowidCompanyGroup = 1,
                RowIdDBConnection = 1,
                IsAdministrator = true,
                PortalName = "PortalTest",
                IdExternalUser = "IdExternalUser",
                RowidExternalUser = 1,
                RowidMainRecord = 1,
                FeaturePermissions = new Dictionary<string, List<int>>()
                {
                    { "BLSDKCompany", new List<int>() { 1, 2, 3,4,5 } },
                    { "BLSDKCompanyGroup", new List<int>(){ 1, 2, 3,4,5 }},
                    { "BLCompany", new List<int>(){ 1, 2, 3,4,5 }},
                    { "BLCompanyGroup", new List<int>(){ 1, 2, 3,4,5 }},
                }
            };

            //string UserToken = JWTUtils.Generate<JwtUserData>(UserTest, Siesa.SDK.Backend.Criptography.SDKRsaKeys.PrivateKey, 30);

            return UserTest;
        }
    }
}