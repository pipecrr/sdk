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
using System.Collections.Concurrent;

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

        /// <summary>
        /// Registra un servicio en el maestro del backend.
        /// </summary>
        /// <param name="businessNames">Lista de nombres de negocio.</param>
        /// <param name="_isFrontendService">Indica si el servicio es de frontend.</param>
        /// <returns>Lista de modelos de negocio (BL) registrados.</returns>
        public Task<List<BusinessModel>> RegisterServiceInMaster(List<BusinessModel> businessNames = null, bool _isFrontendService = false);
        public BackendRegistry GetBackendRegistry(string backendName, IAuthenticationService authenticationService);
        public SDKBusinessModel GetSDKBusinessModel(string backendName, IAuthenticationService authenticationService);
        public string GetViewdef(string businessName, string viewName);

        public List<BusinessModel> GetBusinessModelList();

        /// <summary>
        /// Abre un canal de comunicación asincrónica entre el frontend y el backend.
        /// </summary>
        /// <param name="Callback">Acción a ejecutar al recibir un mensaje en el canal.</param>
        /// <returns>Tarea que devuelve la llamada de streaming dúplex asincrónica.</returns>
        public Task<AsyncDuplexStreamingCall<OpeningChannelToBackRequest, QueueMessageDTO>> OpenChannelFrontToBack(Action<QueueMessageDTO> Callback);

        /// <summary>
        /// Establece los canales de comunicación para un nombre de cola dado.
        /// </summary>
        /// <param name="_queueName">Nombre de la cola.</param>
        /// <param name="_channel">Canal de mensajes.</param>
        public void SetChannels(string _queueName, System.Threading.Channels.Channel<QueueMessageDTO> _channel);

        /// <summary>
        /// Obtiene un diccionario de nombres de cola y listas de canales de mensajes.
        /// </summary>
        /// <returns>Diccionario de nombres de cola y listas de canales de mensajes.</returns>
        public ConcurrentDictionary<string, List<System.Threading.Channels.Channel<QueueMessageDTO>>> GetChannels();

        /// <summary>
        /// Elimina los canales de comunicación para un nombre de cola dado.
        /// </summary>
        /// <param name="_queueName">Nombre de la cola.</param>
        /// <param name="_channel">Canal de mensajes a eliminar.</param>
        public void RemoveChannels(string _queueName, System.Threading.Channels.Channel<QueueMessageDTO> _channel);

    }

    public abstract class BackendRouterServiceBase : IBackendRouterService
    {
        private readonly IServiceConfiguration serviceConfiguration;
        private ConcurrentDictionary<string, BusinessModel> _backendBusinesses = new ConcurrentDictionary<string, BusinessModel>();

        private List<BackendInfo> _observers = new List<BackendInfo>();
        private string _masterBackendURL;
        public static BackendRouterServiceBase Instance { get; private set; }
        private ConcurrentDictionary<string, List<System.Threading.Channels.Channel<QueueMessageDTO>>> Channels { get; set; } = new ConcurrentDictionary<string, List<System.Threading.Channels.Channel<QueueMessageDTO>>>();

        
        /// <summary>
        /// Establece los canales de comunicación para un nombre de cola dado.
        /// </summary>
        /// <param name="_queueName">Nombre de la cola.</param>
        /// <param name="_channel">Canal de mensajes.</param>
        public void SetChannels(string _queueName, System.Threading.Channels.Channel<QueueMessageDTO> _channel)
        {
            if (!Channels.ContainsKey(_queueName))
            {
                Channels.TryAdd(_queueName, new List<System.Threading.Channels.Channel<QueueMessageDTO>>());
            }

            var queueToChannel = Channels[_queueName];

            if (queueToChannel.Exists(x => x == _channel))
            {
                return;
            }
            queueToChannel.Add(_channel);

        }

        /// <summary>
        /// Obtiene un diccionario de nombres de cola y listas de canales de mensajes.
        /// </summary>
        /// <returns>Diccionario de nombres de cola y listas de canales de mensajes.</returns>

        public ConcurrentDictionary<string, List<System.Threading.Channels.Channel<QueueMessageDTO>>> GetChannels()
        {
            return Channels;
        }

                /// <summary>
        /// Elimina los canales de comunicación para un nombre de cola dado.
        /// </summary>
        /// <param name="_queueName">Nombre de la cola.</param>
        /// <param name="_channel">Canal de mensajes a eliminar.</param>

        public void RemoveChannels(string _queueName, System.Threading.Channels.Channel<QueueMessageDTO> _channel)
        {
            if (Channels.ContainsKey(_queueName))
            {
                var queueToChannel = Channels[_queueName];
                if (queueToChannel.Exists(x => x == _channel))
                {
                    queueToChannel.Remove(_channel);
                }
            }
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
                _backendBusinesses.TryAdd(businessName, businessModel);
        }

        public void SetBackendBusinesses(Dictionary<string, BusinessModel> backendBusinesses)
        {
            _backendBusinesses = new ConcurrentDictionary<string, BusinessModel>(backendBusinesses);
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

        /// <summary>
        /// Abre un canal de comunicación asincrónica entre el frontend y el backend.
        /// </summary>
        /// <param name="Callback">Acción a ejecutar al recibir un mensaje en el canal.</param>
        /// <returns>Tarea que devuelve la llamada de streaming dúplex asincrónica.</returns>
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
        
        /// <summary>
        /// Registra un servicio en el maestro del backend.
        /// </summary>
        /// <param name="businessNames">Lista de nombres de negocio.</param>
        /// <param name="_isFrontendService">Indica si el servicio es de frontend.</param>
        /// <returns>Lista de modelos de negocio (BL) registrados.</returns>
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
                Console.WriteLine($"BADGATEWAY - {ex.Message}");
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