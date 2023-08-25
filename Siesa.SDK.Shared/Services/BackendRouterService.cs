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
using Grpc.Core;
using Siesa.SDK.Shared.DTOS;
using System.IO;

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
        public Task<List<BusinessModel>> RegisterServiceInMaster(List<BusinessModel> businessNames = null, bool _isFrontendService = false);
        public BackendRegistry GetBackendRegistry(string backendName, IAuthenticationService authenticationService);
        public SDKBusinessModel GetSDKBusinessModel(string backendName, IAuthenticationService authenticationService);
        public string GetViewdef(string businessName, string viewName);

        public List<BusinessModel> GetBusinessModelList();

        public Task<AsyncDuplexStreamingCall<OpeningChannelToBackRequest, QueueMessageDTO>> OpenChannelFrontToBack(Action<QueueMessageDTO> Callback);

        public Dictionary<string, List<Tuple<AsyncDuplexStreamingCall<OpeningChannelToBackRequest, QueueMessageDTO>, Action<QueueMessageDTO>>>> GetStreamingCalls();

        public void SetChannels(string _queueName, System.Threading.Channels.Channel<QueueMessageDTO> _channel);

        public Dictionary<string, List<System.Threading.Channels.Channel<QueueMessageDTO>>> GetChannels();

    }

    public abstract class BackendRouterServiceBase : IBackendRouterService
    {
        private readonly IServiceConfiguration serviceConfiguration;
        private Dictionary<string, BusinessModel> _backendBusinesses = new Dictionary<string, BusinessModel>();

        private List<BackendInfo> _observers = new List<BackendInfo>();
        private string _masterBackendURL;
        public static BackendRouterServiceBase Instance { get; private set; }

        public Dictionary<string, List<Tuple<AsyncDuplexStreamingCall<OpeningChannelToBackRequest, QueueMessageDTO>, Action<QueueMessageDTO>>>> StreamingCalls { get; set; } = new Dictionary<string, List<Tuple<AsyncDuplexStreamingCall<OpeningChannelToBackRequest, QueueMessageDTO>, Action<QueueMessageDTO>>>>();

        public Dictionary<string, List<System.Threading.Channels.Channel<QueueMessageDTO>>> Channels { get; set; } = new Dictionary<string, List<System.Threading.Channels.Channel<QueueMessageDTO>>>();

        public void SetChannels(string _queueName, System.Threading.Channels.Channel<QueueMessageDTO> _channel)
        {
            if (!Channels.ContainsKey(_queueName))
            {
                Channels.Add(_queueName, new List<System.Threading.Channels.Channel<QueueMessageDTO>>());
            }
            
            var queueToChannel = Channels[_queueName];

            if (queueToChannel.Exists(x => x == _channel))
            {
                return;
            }
            queueToChannel.Add(_channel);

        }

        public Dictionary<string, List<System.Threading.Channels.Channel<QueueMessageDTO>>> GetChannels()
        {
            return Channels;
        }

        public Dictionary<string, List<Tuple<AsyncDuplexStreamingCall<OpeningChannelToBackRequest, QueueMessageDTO>, Action<QueueMessageDTO>>>> GetStreamingCalls()
        {
            return StreamingCalls;
        }

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
            _ = NotifyObservers();
        }

        public void RemoveObserver(BackendInfo observer)
        {
            if (_observers.Contains(observer))
            {

                _observers.Remove(observer);
            }
            _ = NotifyObservers();
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

        public async Task<AsyncDuplexStreamingCall<OpeningChannelToBackRequest, QueueMessageDTO>> OpenChannelFrontToBack(Action<QueueMessageDTO> Callback)
        {
            var channel = GrpcUtils.GetChannel(_masterBackendURL);
            var client = new Protos.GRPCBackendManagerService.GRPCBackendManagerServiceClient(channel);
            var streamingCall = client.OpeningChannelToBack();

            _ = Task.Run(async () =>
            {
                try
                {
                    await foreach (var response in streamingCall.ResponseStream.ReadAllAsync())
                    {
                        Callback(response);
                    }
                }
                catch (System.Exception)
                {

                    Console.WriteLine("Error GRPC Bidireccional");
                }
            });

            return streamingCall;
        }

        public async Task<List<BusinessModel>> RegisterServiceInMaster(List<BusinessModel> businessNames = null, bool _isFrontendService = false)
        {
            try
            {
                if (this.serviceConfiguration.GetCurrentUrl() == _masterBackendURL)
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
                        BackendUrl = this.serviceConfiguration.GetCurrentUrl(),
                        IsFrontendService = _isFrontendService
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