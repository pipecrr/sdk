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
using Siesa.SDK.Entities;


namespace Siesa.SDK.Utils.Test
{
    public class TestUtils
    {
        public static T GetBusiness<T>(Type DbContext, Dictionary<string, List<string>> ListPermission = null, string BdName = "")
        {
            var serviceProvider = GetProvider<T>(DbContext, ListPermission, BdName);

            dynamic Business = ActivatorUtilities.CreateInstance(serviceProvider, typeof(T));
            Business.SetProvider(serviceProvider);

            using(SDKContext context = Business.CreateDbContext())
            {
                var existCompanyGroup = context.Set<E00200_CompanyGroup>().Where(x => x.Rowid == 1).FirstOrDefault();
                var existUser = context.Set<E00220_User>().Where(x => x.Rowid == 1).FirstOrDefault();

                if (existCompanyGroup == null)
                {
                    context.Add(new E00200_CompanyGroup
                    {
                        Rowid = 1,
                        Id = "CompanyGroupTest",
                        Name = "CompanyGroupTest"
                    });
                }

                if (existUser == null)
                {
                    context.Add(new E00220_User
                    {
                        Rowid = 1,
                        Id = "UserTest",
                        Path = "Path",
                        Password = "Password",
                        Name = "Test User",
                        RowidCulture = 1,
                        PasswordAssignmentDate = DateTime.Now,
                        PasswordLastUpdate = DateTime.Now,
                        ChangePasswordFirstLogin = false,
                        StartDateValidity = DateTime.Now,
                    });
                }

                if (context.ChangeTracker.HasChanges())
                {
                    context.SaveChanges(); 
                }
                
            }
            return Business;
        }
        public static IServiceProvider GetProvider<T>(Type _tDbContext, Dictionary<string, List<string>> ListPermission, string BdName = "")
        {
            var ServiceConf = Options.Create(new ServiceConfiguration());

            if (string.IsNullOrEmpty(BdName))
            {
                BdName  = Guid.NewGuid().ToString();
            }

            var ActionsList = new List<string>()
            {
                "Action.Access",
                "Action.Create",
                "Action.Edit",
                "Action.Delete",
                "Action.Detail"
            };

            Dictionary<string, List<int>> PermissionsUser = new Dictionary<string, List<int>>();

            var auth = new Mock<IAuthenticationService>();
            var sdkjwt = new Mock<ISDKJWT>();
            var tenant = new Mock<ITenantProvider>();
            var resourceManager = new Mock<IResourceManager>();
            var backRouter = new Mock<BackendRouterService>(ServiceConf);
            var email = new Mock<EmailService>(backRouter.Object, auth.Object);
            var featurePermissions = new Mock<FeaturePermissionService>();

            //Configure Mock Services:
            if (ListPermission != null && ListPermission.Any())
            {
                foreach (var item in ListPermission.Values)
                {
                    foreach (var action in item)
                    {
                        if (!ActionsList.Contains(action))
                        {
                            ActionsList.Add(action);
                        }
                    }
                }
                
                PermissionsUser = GetPermissionsUser(ListPermission, ActionsList);
            }

            var UserData = GetUser(PermissionsUser);

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

            tenant.Setup(x => x.GetTenant()).ReturnsAsync(tenantOption);
            
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
            mockSection.Setup(x => x.Value).Returns("false");

            Mock<Microsoft.Extensions.Configuration.IConfiguration> mockConfig = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            mockConfig.Setup(x => x.GetSection("AWS:UseS3")).Returns(mockSection.Object);

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
                .Returns(() => (SDKContext)Activator.CreateInstance(_tDbContext, new DbContextOptionsBuilder<SDKContext>().UseInMemoryDatabase($"InMemoryTest_{BdName}").Options));


            var services = new ServiceCollection();
            services.AddScoped<ISDKJWT>(sp => sdkjwt.Object);
            services.AddScoped<ITenantProvider>(sp => tenant.Object);
            services.AddScoped<IAuthenticationService>(sp => auth.Object);
            services.AddScoped<IBackendRouterService>(sp => backRouter.Object);
            services.AddScoped<EmailService>(sp => email.Object);
            services.AddScoped<Microsoft.Extensions.Configuration.IConfiguration>(sp => mockConfig.Object);
            services.AddScoped<IResourceManager>(sp => resourceManager.Object);
            services.AddScoped<Microsoft.Extensions.Logging.ILoggerFactory>(sp => mockLoggerFactory.Object);
            services.AddScoped<IDbContextFactory<SDKContext>>(sp => mockDbFactory.Object);
            services.AddScoped<IFeaturePermissionService>(sp => featurePermissions.Object);

            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider;
        }

        public static JwtUserData GetUser(Dictionary<string, List<int>> PermissionsUser)
        {

            PortalUserJwt portalUser = new PortalUserJwt()
            {
                Rowid = 1,
                Id = "IdExternalUser",
                RowidMainRecord = 1,
                Email = "EmailExternalUser@siesa.com"
            };
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
                PortalUser = portalUser,
                FeaturePermissions = PermissionsUser ?? new Dictionary<string, List<int>>()
            };
            return UserTest;
        }

        public static Dictionary<string, List<int>> GetPermissionsUser(Dictionary<string, List<string>> ListPermission, List<string> ListActions)
        {
            Dictionary<string, List<int>> PermissionsUser = new Dictionary<string, List<int>>();

            foreach (var item in ListPermission)
            {
                List<int> ListActionsUser = new List<int>();
                foreach (var action in item.Value)
                {
                    if (ListActions.Contains(action))
                    {
                        ListActionsUser.Add(ListActions.IndexOf(action) + 1);
                    }
                }
                PermissionsUser.Add(item.Key, ListActionsUser);
            }

            return PermissionsUser;
        }
    }
}