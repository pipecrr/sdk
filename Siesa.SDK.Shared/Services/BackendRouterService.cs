using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;
using Siesa.SDK.Protos;
using Siesa.SDK.Shared.Backend;
using Siesa.SDK.Shared.Configurations;
using System.Linq;
using Siesa.SDK.Shared.GRPCServices;

namespace Siesa.SDK.Shared.Services
{
    public interface IBackendRouterService
    {
        public BusinessModel GetBackend(string businessName);
        public void AddBackend(string businessName, BusinessModel businessModel);
        public void SetBackendBusinesses(Dictionary<string, BusinessModel> backendBusinesses);
        public void AddObserver(BackendInfo observer);
        public void RemoveObserver(BackendInfo observer);
        public Task NotifyObservers();
        public Task<List<BusinessModel>> RegisterServiceInMaster(List<BusinessModel> businessNames = null);
        public BackendRegistry GetBackendRegistry(string backendName, IAuthenticationService authenticationService);
        public SDKBusinessModel GetSDKBusinessModel(string backendName, IAuthenticationService authenticationService);
        public string GetViewdef(string businessName, string viewName);

        public List<BusinessModel> GetBusinessModelList();

    }

    public abstract class BackendRouterServiceBase: IBackendRouterService
    {
        private readonly IServiceConfiguration serviceConfiguration;
        private Dictionary<string, BusinessModel> _backendBusinesses = new Dictionary<string, BusinessModel>();

        private List<BackendInfo> _observers = new List<BackendInfo>();
        private string _masterBackendURL;
        public static BackendRouterServiceBase Instance { get; private set; }

        public BackendRouterServiceBase(IOptions<ServiceConfiguration> serviceConfiguration)
        {
            this.serviceConfiguration = serviceConfiguration.Value;
            _masterBackendURL = this.serviceConfiguration.MasterBackendUrl;
            Instance = this;
        }

        public BusinessModel GetBackend(string businessName)
        {
            if (_backendBusinesses.ContainsKey(businessName))
                return _backendBusinesses[businessName];
            return null;
        }

        public void AddBackend(string businessName, BusinessModel businessModel)
        {
            if (!_backendBusinesses.ContainsKey(businessName))
                _backendBusinesses.Add(businessName, businessModel);
        }

        public void SetBackendBusinesses(Dictionary<string, BusinessModel> backendBusinesses)
        {
            _backendBusinesses = backendBusinesses;
        }

        public void AddObserver(BackendInfo observer)
        {
            if (observer.BackendUrl == _masterBackendURL)
            {
                return;
            }
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
            NotifyObservers();
        }

        public void RemoveObserver(BackendInfo observer)
        {
            if (_observers.Contains(observer))
            {

                _observers.Remove(observer);
            }
            NotifyObservers();
        }


        public async Task NotifyObservers()
        {
            foreach (var observer in _observers)
            {
                try
                {
                    using var channel = GrpcUtils.GetChannel(observer.BackendUrl);
                    var client = new Protos.Shared.SharedClient(channel);
                    var request = new Protos.SetBackendServicesRequest
                    {
                        Businesses = { _backendBusinesses.Values }
                    };
                    await client.SetBackendServicesAsync(request);
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine("Error notifying observer: " + ex.Message);
                }
            }
        }

        public async Task<List<BusinessModel>> RegisterServiceInMaster(List<BusinessModel> businessNames = null)
        {
            try
            {
                if(this.serviceConfiguration.GetCurrentUrl() == _masterBackendURL)
                {
                    await Task.Delay(5000); //wait for the master backend to be ready
                }

                using var channel = GrpcUtils.GetChannel(_masterBackendURL);
                var client = new Protos.GRPCBackendManagerService.GRPCBackendManagerServiceClient(channel);
                var request = new Protos.RegisterBackendRequest
                {
                    BackendInfo = new Protos.BackendInfo
                    {
                        BackendName = "",
                        BackendUrl = this.serviceConfiguration.GetCurrentUrl()
                    }
                };
                if (businessNames != null)
                {
                    request.BackendInfo.Businesses.AddRange(businessNames);
                }

                var response = await client.RegisterBackendAsync(request);
                return response.Businesses.ToList<BusinessModel>();
                


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public BackendRegistry GetBackendRegistry(string backendName, IAuthenticationService authenticationService)
        {
            if (_backendBusinesses.ContainsKey(backendName))
            {
                var backend = _backendBusinesses[backendName];
                var BackendRegistry = new BackendRegistry(backend.Name, backend.Url);
                BackendRegistry.SetAuthenticationService(authenticationService);
                return BackendRegistry;
            }
            else
            {
                return null;
            }
        }

        public SDKBusinessModel GetSDKBusinessModel(string backendName, IAuthenticationService authenticationService)
        {
            if (_backendBusinesses.ContainsKey(backendName))
            {
                var backend = _backendBusinesses[backendName];
                var response = new SDKBusinessModel(backend.Name, backend.Namespace);
                response.SetAuthenticationService(authenticationService, this);
                return response;
            }
            else
            {
                return null;
            }
        }

        public List<BusinessModel> GetBusinessModelList()
        {
            return new List<BusinessModel>(_backendBusinesses.Values);
        }

        public virtual string GetViewdef(string businessName, string viewName)
        {
            throw new NotImplementedException();
        }
    }
}