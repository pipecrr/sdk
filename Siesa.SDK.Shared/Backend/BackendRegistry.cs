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
using Siesa.SDK.Shared.GRPCServices;
namespace Siesa.SDK.Shared.Backend
{
    public class BackendRegistry
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public BusinessesResponse businessRegisters = new BusinessesResponse();
        private readonly IServiceScopeFactory _scopeFactory;

        private IAuthenticationService AuthenticationService { get; set; }

        public BackendRegistry(string name, string url)
        {
            this.Name = name;
            this.Url = url;
        }

        public BackendRegistry(string name, string url, Google.Protobuf.Collections.RepeatedField<Protos.BusinessModel> businesses)
        {
            this.Name = name;
            this.Url = url;
            var businessResponse = new BusinessesResponse();
            businessResponse.Businesses.AddRange(businesses);
            businessRegisters = businessResponse;
        }

        public void SetAuthenticationService(IAuthenticationService authenticationService)
        {
            AuthenticationService = authenticationService;
        }

        public async Task<ValidateAndSaveBusinessObjResponse> ValidateAndSaveBusiness(string business_name, dynamic obj)
        {
            using var channel = GrpcUtils.GetChannel(this.Url);
            var client = new Protos.SDK.SDKClient(channel);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var request = new Protos.ValidateAndSaveBusinessObjRequest
            {
                Business = json,
                BusinessName = business_name,
                CurrentUserToken = (AuthenticationService != null && AuthenticationService.UserToken != null ? AuthenticationService.UserToken : ""),
                CurrentUserRowid = (AuthenticationService != null && AuthenticationService.User != null ? AuthenticationService.User.Rowid: 0)
            };
            var response = await client.ValidateAndSaveBusinessObjAsync(request);
            return response;
        }
        public async Task<Protos.LoadResult> EntityFieldSearch(string business_name, string searchText, string filters, int? top = null, string orderBy = "", List<string> extraFields = null)
        {
            var channel = GrpcUtils.GetChannel(this.Url);
            var client = new Protos.SDK.SDKClient(channel);
            List<string> fields = extraFields ?? new List<string>();
            var request = new Protos.EntityFieldSearchRequest
            {
                BusinessName = business_name,
                SearchText = searchText,
                Filters = filters,
                CurrentUserToken = (AuthenticationService != null && AuthenticationService.UserToken != null ? AuthenticationService.UserToken : ""),
                CurrentUserRowid = (AuthenticationService != null && AuthenticationService.User != null ? AuthenticationService.User.Rowid: 0),
                Top = top,
                OrderBy = orderBy,
                ExtraFields = {fields}
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

        public async Task<Int64> SaveBusiness(string business_name, dynamic obj)
        {
            using var channel = GrpcUtils.GetChannel(this.Url);
            var client = new Protos.SDK.SDKClient(channel);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var request = new Protos.SaveBusinessObjRequest
            {
                Business = json,
                BusinessName = business_name,
                CurrentUserToken = (AuthenticationService != null && AuthenticationService.UserToken != null ? AuthenticationService.UserToken : ""),
                CurrentUserRowid = (AuthenticationService != null && AuthenticationService.User != null ? AuthenticationService.User.Rowid: 0)
            };
            var response = await client.SaveBusinessObjAsync(request);
            return response.Id;

        }

        public async Task<dynamic> GetBusinessObj(string business_name, Int64 id,List<string> extraFields = null)
        {
            using var channel = GrpcUtils.GetChannel(this.Url);
            var client = new Protos.SDK.SDKClient(channel);
            List<string> fields = extraFields ?? new List<string>();
            var request = new Protos.GetBusinessObjRequest
            {
                Id = id,
                BusinessName = business_name,
                CurrentUserToken = (AuthenticationService != null && AuthenticationService.UserToken != null ? AuthenticationService.UserToken : ""),
                CurrentUserRowid = (AuthenticationService != null && AuthenticationService.User != null ? AuthenticationService.User.Rowid: 0),
                ExtraFields = {fields}

            };
            var response = await client.GetBusinessObjAsync(request);
            return response.Response;
        }

        public async Task<DeleteBusinessObjResponse> DeleteBusinessObj(string business_name, Int64 id)
        {
            using var channel = GrpcUtils.GetChannel(this.Url);
            var client = new Protos.SDK.SDKClient(channel);
            var request = new Protos.DeleteBusinessObjRequest
            {
                Id = id,
                BusinessName = business_name,
                CurrentUserToken = (AuthenticationService != null && AuthenticationService.UserToken != null ? AuthenticationService.UserToken : ""),
                CurrentUserRowid = (AuthenticationService != null && AuthenticationService.User != null ? AuthenticationService.User.Rowid: 0)
            };
            var response = await client.DeleteBusinessObjAsync(request);
            return response;

        }

        public async Task<Protos.LoadResult> GetDataBusinessObj(string business_name, int? skip, int? take, string filter = "", string orderBy = "", bool includeCount = false,  List<string> extraFields = null)
        {
            var channel = GrpcUtils.GetChannel(this.Url);
            var client = new Protos.SDK.SDKClient(channel);
            List<string> fields = extraFields ?? new List<string>();
            var request = new Protos.GetDataBusinessObjRequest
            {
                BusinessName = business_name,
                Skip = skip,
                Take = take,
                Filter = filter,
                OrderBy = orderBy,
                CurrentUserToken = (AuthenticationService != null && AuthenticationService.UserToken != null ? AuthenticationService.UserToken : ""),
                CurrentUserRowid = (AuthenticationService != null && AuthenticationService.User != null ? AuthenticationService.User.Rowid: 0),
                IncludeCount = includeCount,
                ExtraFields = {fields}
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
            using var channel = GrpcUtils.GetChannel(this.Url);
            var client = new Protos.SDK.SDKClient(channel);
            var request = new Protos.GetMenuGroupsRequest{
                CurrentUserToken = (AuthenticationService != null && AuthenticationService.UserToken != null ? AuthenticationService.UserToken : ""),
                CurrentUserRowid = (AuthenticationService != null && AuthenticationService.User != null ? AuthenticationService.User.Rowid: 0)
            };
            var response = await client.GetMenuGroupsAsync(request);
            return response;
        }

        public async Task<Protos.MenuItemsResponse> GetMenuItemsAsync(int groupId)
        {
            using var channel = GrpcUtils.GetChannel(this.Url);
            var client = new Protos.SDK.SDKClient(channel);
            var request = new Protos.GetMenuItemsRequest
            {
                GroupId = groupId,
                CurrentUserToken = (AuthenticationService != null && AuthenticationService.UserToken != null ? AuthenticationService.UserToken : ""),
                CurrentUserRowid = (AuthenticationService != null && AuthenticationService.User != null ? AuthenticationService.User.Rowid: 0)
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
            var channel = GrpcUtils.GetChannel(this.Url);
            var client = new Protos.SDK.SDKClient(channel);
            var request = new Protos.ExposedMethodRequest
            {
                BusinessName = business_name,
                MethodName = method,
                CurrentUserToken = (AuthenticationService != null && AuthenticationService.UserToken != null ? AuthenticationService.UserToken : ""),
                CurrentUserRowid = (AuthenticationService != null && AuthenticationService.User != null ? AuthenticationService.User.Rowid: 0)
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
