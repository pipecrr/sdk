using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Siesa.SDK.Protos;
using Siesa.SDK.Frontend;

namespace Siesa.SDK.Frontend.Backend
{
    public class BackendRegistry
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public BusinessesResponse businessRegisters = new BusinessesResponse();

        public BackendRegistry(string name, string url)
        {
            this.Name = name;
            this.Url = url;
            this.GetBusinesses();

        }

        public async void GetBusinesses()
        {
            using var channel = GrpcChannel.ForAddress(this.Url);
            var client = new Protos.SDK.SDKClient(channel);
            var response = await client.GetBusinessesAsync(new Protos.GetBusinessesRequest());
            this.businessRegisters = response;
            foreach (var business in this.businessRegisters.Businesses)
            {
                BusinessManager.Instance.AddBusiness(business, this.Name);

            }

        }

        public async Task<int> SaveBusiness(string business_name, dynamic obj)
        {
            using var channel = GrpcChannel.ForAddress(this.Url);
            var client = new Protos.SDK.SDKClient(channel);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var request = new Protos.SaveBusinessObjRequest
            {
                Business = json,
                BusinessName = business_name
            };
            var response = await client.SaveBusinessObjAsync(request);
            return response.Id;

        }

        public async Task<dynamic> GetBusinessObj(string business_name, int id)
        {
            using var channel = GrpcChannel.ForAddress(this.Url);
            var client = new Protos.SDK.SDKClient(channel);
            var request = new Protos.GetBusinessObjRequest
            {
                Id = id,
                BusinessName = business_name
            };
            var response = await client.GetBusinessObjAsync(request);
            return response.Response;

        }

        public async Task<int> DeleteBusinessObj(string business_name, int id)
        {
            using var channel = GrpcChannel.ForAddress(this.Url);
            var client = new Protos.SDK.SDKClient(channel);
            var request = new Protos.DeleteBusinessObjRequest
            {
                Id = id,
                BusinessName = business_name
            };
            var response = await client.DeleteBusinessObjAsync(request);
            return response.Id;

        }
    }
}
