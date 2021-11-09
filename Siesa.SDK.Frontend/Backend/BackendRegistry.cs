using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Siesa.SDK.Protos;
using Siesa.SDK.Frontend;
using Grpc.Core;

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

        public async Task<ValidateAndSaveBusinessObjResponse> ValidateAndSaveBusiness(string business_name, dynamic obj)
        {
            using var channel = GrpcChannel.ForAddress(this.Url);
            var client = new Protos.SDK.SDKClient(channel);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var request = new Protos.ValidateAndSaveBusinessObjRequest
            {
                Business = json,
                BusinessName = business_name
            };
            var response = await client.ValidateAndSaveBusinessObjAsync(request);
            return response;
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

        public async Task<LoadResult> GetListBusinessObj(string business_name, int page, int pageSize, string options)
        {
            using var channel = GrpcChannel.ForAddress(this.Url);
            var client = new Protos.SDK.SDKClient(channel);
            var request = new Protos.GetListBusinessObjRequest
            {
                BusinessName = business_name,
                Page = page,
                PageSize = pageSize,
                Options = options
            };
            var response = await client.GetListBusinessObjAsync(request);
            return response;
        }
    }
}
