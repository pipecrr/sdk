using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Siesa.SDK.Protos;
using Grpc.Core;
using Siesa.SDK.Shared.Business;
using Siesa.SDK.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
namespace Siesa.SDK.Shared.Backend
{
    public class BackendRegistry
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public BusinessesResponse businessRegisters = new BusinessesResponse();
        private readonly IServiceScopeFactory _scopeFactory;

        public BackendRegistry(string name, string url, IServiceScopeFactory scopeFactory)
        {
            this.Name = name;
            this.Url = url;
            _scopeFactory = scopeFactory;
            this.GetBusinesses();
        }

        public BackendRegistry(string name, string url, Google.Protobuf.Collections.RepeatedField<Protos.BusinessModel> businesses, IServiceScopeFactory scopeFactory)
        {
            this.Name = name;
            this.Url = url;
            _scopeFactory = scopeFactory;
            var businessResponse = new BusinessesResponse();
            businessResponse.Businesses.AddRange(businesses);
            businessRegisters = businessResponse;
            addBusiness();
        }

        private IAuthenticationService GetAuthenticationService(){
            using(var scope = _scopeFactory.CreateScope())
            {
                IAuthenticationService authenticationService  = (IAuthenticationService)scope.ServiceProvider.GetService(typeof(IAuthenticationService));
                return authenticationService;
            }
        }

        private void addBusiness()
        {
            foreach (var business in this.businessRegisters.Businesses)
            {
                BusinessManager.Instance.AddBusiness(business);
            }
        }

        public void GetBusinesses()
        {
            var authenticationService = GetAuthenticationService();
            using var channel = GrpcChannel.ForAddress(this.Url);
            var client = new Protos.SDK.SDKClient(channel);
            var response = client.GetBusinesses(new Protos.GetBusinessesRequest{
                CurrentUserToken = authenticationService.UserToken,
                CurrentUserRowid = authenticationService.User?.Rowid
            });
            this.businessRegisters = response;
            addBusiness();
        }

        public async Task<ValidateAndSaveBusinessObjResponse> ValidateAndSaveBusiness(string business_name, dynamic obj)
        {
            var authenticationService = GetAuthenticationService();
            using var channel = GrpcChannel.ForAddress(this.Url);
            var client = new Protos.SDK.SDKClient(channel);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var request = new Protos.ValidateAndSaveBusinessObjRequest
            {
                Business = json,
                BusinessName = business_name,
                CurrentUserToken = authenticationService.UserToken,
                CurrentUserRowid = authenticationService.User?.Rowid
            };
            var response = await client.ValidateAndSaveBusinessObjAsync(request);
            return response;
        }
        public async Task<Protos.LoadResult> EntityFieldSearch(string business_name, string searchText)
        {
            var authenticationService = GetAuthenticationService();
            var channel = GrpcChannel.ForAddress(this.Url);
            var client = new Protos.SDK.SDKClient(channel);
            var request = new Protos.EntityFieldSearchRequest
            {
                BusinessName = business_name,
                SearchText = searchText,
                CurrentUserToken = authenticationService.UserToken,
                CurrentUserRowid = authenticationService.User?.Rowid
            };
            try
            {
                var response = await client.EntityFieldSearchAsync(request);
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
            
        }

        public async Task<int> SaveBusiness(string business_name, dynamic obj)
        {
            var authenticationService = GetAuthenticationService();
            using var channel = GrpcChannel.ForAddress(this.Url);
            var client = new Protos.SDK.SDKClient(channel);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var request = new Protos.SaveBusinessObjRequest
            {
                Business = json,
                BusinessName = business_name,
                CurrentUserToken = authenticationService.UserToken,
                CurrentUserRowid = authenticationService.User?.Rowid
            };
            var response = await client.SaveBusinessObjAsync(request);
            return response.Id;

        }

        public async Task<dynamic> GetBusinessObj(string business_name, int id)
        {
            var authenticationService = GetAuthenticationService();
            using var channel = GrpcChannel.ForAddress(this.Url);
            var client = new Protos.SDK.SDKClient(channel);
            var request = new Protos.GetBusinessObjRequest
            {
                Id = id,
                BusinessName = business_name,
                CurrentUserToken = authenticationService.UserToken,
                CurrentUserRowid = authenticationService.User?.Rowid
            };
            var response = await client.GetBusinessObjAsync(request);
            return response.Response;

        }

        public async Task<int> DeleteBusinessObj(string business_name, int id)
        {
            var authenticationService = GetAuthenticationService();
            using var channel = GrpcChannel.ForAddress(this.Url);
            var client = new Protos.SDK.SDKClient(channel);
            var request = new Protos.DeleteBusinessObjRequest
            {
                Id = id,
                BusinessName = business_name,
                CurrentUserToken = authenticationService.UserToken,
                CurrentUserRowid = authenticationService.User?.Rowid
            };
            var response = await client.DeleteBusinessObjAsync(request);
            return response.Id;

        }

        public async Task<Protos.LoadResult> GetDataBusinessObj(string business_name, int? skip, int? take, string filter = "", string orderBy = "")
        {
            var authenticationService = GetAuthenticationService();
            var channel = GrpcChannel.ForAddress(this.Url);
            var client = new Protos.SDK.SDKClient(channel);
            var request = new Protos.GetDataBusinessObjRequest
            {
                BusinessName = business_name,
                Skip = skip,
                Take = take,
                Filter = filter,
                OrderBy = orderBy,
                CurrentUserToken = authenticationService.UserToken,
                CurrentUserRowid = authenticationService.User?.Rowid
            };
            try
            {
                var response = await client.GetDataBusinessObjAsync(request);
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
            
        }

        public async Task<Protos.MenuGroupsResponse> GetMenuGroupsAsync()
        {
            var authenticationService = GetAuthenticationService();
            using var channel = GrpcChannel.ForAddress(this.Url);
            var client = new Protos.SDK.SDKClient(channel);
            var request = new Protos.GetMenuGroupsRequest{
                CurrentUserToken = authenticationService.UserToken,
                CurrentUserRowid = authenticationService.User?.Rowid
            };
            var response = await client.GetMenuGroupsAsync(request);
            return response;
        }

        public async Task<Protos.MenuItemsResponse> GetMenuItemsAsync(int groupId)
        {
            var authenticationService = GetAuthenticationService();
            using var channel = GrpcChannel.ForAddress(this.Url);
            var client = new Protos.SDK.SDKClient(channel);
            var request = new Protos.GetMenuItemsRequest
            {
                GroupId = groupId,
                CurrentUserToken = authenticationService.UserToken,
                CurrentUserRowid = authenticationService.User?.Rowid
            };
            var response = await client.GetMenuItemsAsync(request);
            return response;
        }

        public Protos.MenuGroupsResponse GetMenuGroups()
        {
            return GetMenuGroupsAsync().GetAwaiter().GetResult();
        }

        public Protos.MenuItemsResponse GetMenuItems(int groupId)
        {
            return GetMenuItemsAsync(groupId).GetAwaiter().GetResult();
        }

        public async Task<Protos.ExposedMethodResponse> CallBusinessMethod(string business_name, string method, ICollection<ExposedMethodParam> parameters)
        {
            var authenticationService = GetAuthenticationService();
            var channel = GrpcChannel.ForAddress(this.Url);
            var client = new Protos.SDK.SDKClient(channel);
            var request = new Protos.ExposedMethodRequest
            {
                BusinessName = business_name,
                MethodName = method,
                CurrentUserToken = authenticationService.UserToken,
                CurrentUserRowid = authenticationService.User?.Rowid
            };
            if(parameters != null)
            {
                request.Parameters.AddRange(parameters);
            }
            try
            {
                var response = await client.ExecuteExposedMethodAsync(request);
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
            
        }
    }
}
