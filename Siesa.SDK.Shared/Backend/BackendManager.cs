using Grpc.Net.Client;
using Microsoft.Extensions.Options;
using Siesa.SDK.Shared.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siesa.SDK.Shared.Backend
{
    public interface IBackendManager {
        public void RegisterBackend(string backendName, string backendUrl);
        public void RegisterBackend(string backendName, string backendUrl, Google.Protobuf.Collections.RepeatedField<Protos.BusinessModel> businesses);
        public void RegisterBackendInMaster(string backendName, string backendUrl);
        public void SyncWithMasterBackend();
        public BackendRegistry GetBackend(string backendName);
        public bool IsBackendRegistered(string backendName);
        public BackendRegistry GetBackendByBusinessName(string businessName);
        public Dictionary<string, BackendRegistry> GetBackendDict();
    }
    public class BackendManager: IBackendManager
    {
        private Dictionary<string, BackendRegistry> backendDict = new Dictionary<string, BackendRegistry>();

        private readonly IServiceConfiguration serviceConfiguration;

        public string _masterBackendURL;

        public static BackendManager Instance { get; private set; }

        public BackendManager(IOptions<ServiceConfiguration> serviceConfiguration)
        {
            this.serviceConfiguration = serviceConfiguration.Value;
            _masterBackendURL = this.serviceConfiguration.MasterBackendUrl;
            Instance = this;
        }

        public void RegisterBackend(string backendName, string backendUrl)
        {
            if (!backendDict.ContainsKey(backendName))
            {
                backendDict.Add(backendName, new BackendRegistry(backendName, backendUrl));
            }
        }

        public void RegisterBackend(string backendName, string backendUrl, Google.Protobuf.Collections.RepeatedField<Protos.BusinessModel> businesses)
        {
            if (!backendDict.ContainsKey(backendName))
            {
                backendDict.Add(backendName, new BackendRegistry(backendName, backendUrl, businesses));
            }
        }

        public async void RegisterBackendInMaster(string backendName, string backendUrl)
        {
            try
            {
                using var channel = GrpcChannel.ForAddress(_masterBackendURL);
                var client = new Protos.GRPCBackendManagerService.GRPCBackendManagerServiceClient(channel);
                var request = new Protos.RegisterBackendRequest { BackendInfo = new Protos.BackendInfo { BackendName = backendName, BackendUrl = backendUrl } };
                var response = await client.RegisterBackendAsync(request);
            }catch (Exception ex)
            {

            }
            
                       
        }

        public void SyncWithMasterBackend()
        {
            using var channel = GrpcChannel.ForAddress(_masterBackendURL);
            var client = new Protos.GRPCBackendManagerService.GRPCBackendManagerServiceClient(channel);
            var request = new Protos.GetBackendsRequest();
            var response = client.GetBackends(request);
            foreach (var backend in response.Backends)
            {
                if (backend.Businesses.Count() > 0) {
                    RegisterBackend(backend.BackendName, backend.BackendUrl, backend.Businesses);
                }
                else
                {
                    RegisterBackend(backend.BackendName, backend.BackendUrl);
                }
                
            }
        }

        public BackendRegistry GetBackend(string backendName)
        {
            if (backendDict.ContainsKey(backendName))
            {
                return backendDict[backendName];
            }
            else
            {
                return null;
            }
        }

        public Dictionary<string, BackendRegistry> GetBackendDict()
        {
            return backendDict;
        }

        public bool IsBackendRegistered(string backendName)
        {
            return backendDict.ContainsKey(backendName);
        }

        public BackendRegistry GetBackendByBusinessName(string businessName)
        {
            //TODO: Implementar BusinessManager al igual que el front para no buscar todo el tiempo
            foreach (var backend in backendDict)
            {
                foreach (var business in backend.Value.businessRegisters.Businesses)
                {
                    if (business.Name == businessName)
                    {
                        return backend.Value;
                    }
                }
            }
            return null;
        }
    }
}
