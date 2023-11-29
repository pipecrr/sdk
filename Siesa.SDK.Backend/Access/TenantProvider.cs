using System;
using System.Collections.Generic;
using System.Linq;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Shared.DTOS;
using Microsoft.EntityFrameworkCore;
using Siesa.SDK.Backend.Interceptors;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Siesa.SDK.Backend.Services;
using Microsoft.Extensions.Configuration;
using Siesa.SDK.Shared.Configurations;

namespace Siesa.SDK.Backend.Access
{
    public interface ITenantProvider
    {
        void SetTenant(SDKDbConnection tenant);
        Task SetTenantByRowId(short rowId, string hostName = "");
        Guid Register(Action callback);
        void Unregister(Guid id);
        Task<SDKDbConnection> GetTenant();
        Task<List<SDKDbConnection>> GetTenants(string HostName = "");
        string GetTenantShortName();
        bool GetUseLazyLoadingProxies();
        void SetUseLazyLoadingProxies(bool value);
        Task<DbContextOptionsBuilder> GetContextOptionTenant(short rowIdDBConnection, string hostName = "");
        string GetConnectionString(SDKDbConnection _dbConnection);
    }
    public class TenantProvider: ITenantProvider
    {
        private readonly IDictionary<Guid, Action> callbacks = new Dictionary<Guid, Action>();

        private List<SDKDbConnection> tenants = new List<SDKDbConnection>();

        private SDKDbConnection tenant = null;
        private readonly SDKConnectionConfig _config = null;
        private bool _isRemote => _config != null && _config.IsRemote;

        private readonly IAuthenticationService _authenticationService;

        private readonly MemoryService _memoryService;
        private readonly IConfiguration _configuration;

        private bool UseLazyLoadingProxies = false;

        private readonly HttpClient httpClient = new HttpClient();

        public void SetTenant(SDKDbConnection tenant)
        {
            this.tenant = tenant;
            foreach (var callback in callbacks.Values)
            {
                callback();
            }
        }

        public async Task SetTenantByRowId(short rowId, string hostName = "")
        {
            if((tenants == null || tenants.Count == 0) && !string.IsNullOrEmpty(hostName))
            {
                tenants = await GetTenants(hostName);
            }

            var tenant = tenants.FirstOrDefault(x => x.Rowid == rowId);
            if (tenant != null)
            {
                SetTenant(tenant);
            }
        }
    
        public TenantProvider(IAuthenticationService authenticationService, MemoryService memoryService, IConfiguration configuration, List<SDKDbConnection> tenants, SDKConnectionConfig config = null)
        {
            _authenticationService = authenticationService;
            _memoryService = memoryService;
            _configuration = configuration;

            if (config != null)
            { 
                _config = config;

                if(!_config.IsRemote)
                {
                    this.tenants = tenants;
                }
            }else{
                this.tenants = tenants;
            }
        }

        public void Unregister(Guid id)
        {
            if (callbacks.ContainsKey(id))
            {
                callbacks.Remove(id);
            }
        }

        public Guid Register(Action callback)
        {
            var id = Guid.NewGuid();
            callbacks.Add(id, callback);
            return id;
        }

        public async Task<SDKDbConnection> GetTenant() => tenant;
        public async Task<List<SDKDbConnection>> GetTenants(string HostName = "")
        {
            if (_isRemote && !string.IsNullOrEmpty(HostName))
            {
                var request = await GetTenantFromHost(HostName);
                if (!string.IsNullOrEmpty(request))
                {
                    try
                    {
                        var connections = JsonConvert.DeserializeObject<dynamic>(request.ToString())?.DbConnections;

                        if (connections != null && connections.Count > 0)
                        {
                            tenants = connections.ToObject<List<SDKDbConnection>>();
                        } 
                    }
                    catch 
                    {
                        
                        Console.WriteLine("Error al deserializar");
                    }
                }
            }
            return tenants;
        }

        public string GetTenantShortName() => tenant.Name;

        public List<SDKDbConnection> GetTenants() => tenants;

        public bool GetUseLazyLoadingProxies() => UseLazyLoadingProxies;

        public void SetUseLazyLoadingProxies(bool value) => UseLazyLoadingProxies = value;

        public string GetConnectionString(SDKDbConnection _dbConnection)
        {
            var connectionString = _dbConnection.ConnectionString;
            if (_dbConnection.ProviderName == EnumDBType.InMemory)
            {
                return connectionString;
            }else if(_dbConnection.ProviderName == EnumDBType.PostgreSQL)
            {
                return connectionString;
            }else { //Default to SQL Server
                //add Application Name= if not present
                if(!connectionString.Contains("Application Name=")){
                    var serviceConfiguration = _configuration.GetSection("ServiceConfiguration");
                    ServiceConfiguration sc = serviceConfiguration.Get<ServiceConfiguration>();
                    var currentUrl = sc?.GetCurrentUrl();
                    if(!string.IsNullOrEmpty(currentUrl)){
                        //delete http:// or https://
                        currentUrl = currentUrl.Replace("http://", "");
                        currentUrl = currentUrl.Replace("https://", "");
                        //delete last /
                        if(currentUrl.Last() == '/'){
                            currentUrl = currentUrl.Substring(0, currentUrl.Length - 1);
                        }
                    }
                    //get machine name
                    var machineName = Environment.MachineName;
                    //check if last char is ;
                    if(connectionString.Last() != ';'){
                        connectionString += ";";
                    }
                    var appName = $"{machineName}";

                    if(!string.IsNullOrEmpty(currentUrl)){
                        appName += $"-{currentUrl}";
                    }
                    connectionString += $"Application Name=SDK-{appName};";
                }
                //add Encrypt=False if not present
                if(!connectionString.Contains("Encrypt=")){
                    if(connectionString.Last() != ';'){
                        connectionString += ";";
                    }
                    connectionString += "Encrypt=False;";
                }
                return connectionString;
            }
        }

        public async Task<DbContextOptionsBuilder> GetContextOptionTenant(short rowIdDBConnection, string hostName = "")
        {
            try
            {
                if (rowIdDBConnection == 0)
                {
                    throw new Exception("Tenant provider not found");
                }
                if (!tenants.Any() && !string.IsNullOrEmpty(hostName))
                {
                    await GetTenants(hostName);
                }

                this.SetTenantByRowId(rowIdDBConnection);
                var _tenant = await GetTenant().ConfigureAwait(true);
                if (_tenant == null)
                {
                    throw new Exception("Tenant provider not found");
                }

                DbContextOptionsBuilder tenantOptions = new DbContextOptionsBuilder<SDKContext>();

                if (_tenant.ProviderName == EnumDBType.PostgreSQL)
                {
                    tenantOptions.UseNpgsql(this.GetConnectionString(_tenant));
                }
                else
                {
                    //Default to SQL Server
                    tenantOptions.UseSqlServer(this.GetConnectionString(_tenant));
                }

                tenantOptions.AddInterceptors(new SDKDBInterceptor());

                return tenantOptions;
            }
            catch (Exception e)
            {
                throw new Exception($"{e.Message}");
            }
        }
        private async Task<string> GetTenantFromHost(string HostName)
        {
            try
            {
                string content = string.Empty;
                if (_memoryService != null)
                {
                    var _tenants = _memoryService.Get(HostName);
                    if (!string.IsNullOrEmpty(_tenants))
                    {
                        return _tenants;
                    }
                }

                HttpResponseMessage response = await httpClient.PostAsync(_config.ApiUrl, null);

                if (response.IsSuccessStatusCode)
                {
                    content = await response.Content.ReadAsStringAsync();
                    if (_memoryService != null)
                    {
                        _memoryService.Set(HostName, content);
                    }
                }
                return content;
            }
            catch (Exception e)
            {
                throw new Exception($"Error al consumir la API: {e.Message}");
            }
        }
    }
}
