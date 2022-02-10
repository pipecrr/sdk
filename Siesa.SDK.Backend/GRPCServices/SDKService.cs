using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Siesa.SDK.Business;
using Siesa.SDK.Protos;
using System.Reflection;
using Siesa.SDK.Shared.Business;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Siesa.SDK.Backend.Access;
using Siesa.SDK.Entities;

namespace Siesa.SDK.GRPCServices
{
    public class SDKService : Siesa.SDK.Protos.SDK.SDKBase
    {
        private readonly ILogger<SDKService> _logger;
        private readonly IServiceProvider _provider;
        public SDKService(ILogger<SDKService> logger, IServiceProvider provider)
        {
            _logger = logger;
            _provider = provider;
        }

        public override Task<Protos.BusinessesResponse> GetBusinesses(Protos.GetBusinessesRequest request, ServerCallContext context)
        {
            var response = new Protos.BusinessesResponse();
            response.Businesses.AddRange(BusinessManager.Instance.Businesses.Values);
            return Task.FromResult(response);
        }

        public override Task<Protos.BusinessObjResponse> GetBusinessObj(Protos.GetBusinessObjRequest request, ServerCallContext context)
        {
            BusinessModel businessRegistry = BusinessManager.Instance.GetBusiness(request.BusinessName);
            var businessType = FindType(businessRegistry.Namespace + "." + businessRegistry.Name);
            dynamic businessObj = Activator.CreateInstance(businessType);
            businessObj.SetProvider(_provider);

            var response = new Protos.BusinessObjResponse();
            var result = businessObj.Get(request.Id);

            businessObj.DetachedBaseObj();


            response.Response = Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Task.FromResult(response);
        }

        private Type FindType(string name)
        {
            Type businessType = null;
            foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (ass.FullName.StartsWith("System.") || ass.FullName.StartsWith("Microsoft.") || ass.FullName.StartsWith("mscorlib") || ass.FullName.StartsWith("netstandard") || ass.FullName.StartsWith("GRPC."))
                    continue;
                businessType = ass.GetType(name);
                if (businessType != null)
                    break;
            }
            return businessType;
        }

        public override Task<Protos.SaveBusinessObjResponse> SaveBusinessObj(Protos.SaveBusinessObjRequest request, ServerCallContext context)
        {
            var response = new Protos.SaveBusinessObjResponse();
            BusinessModel businessRegistry = BusinessManager.Instance.GetBusiness(request.BusinessName);
            var businessType = FindType(businessRegistry.Namespace + "." + businessRegistry.Name);
            //dynamic x = Activator.CreateInstance(businessType);
            //json deserialize using Newtonsoft.Json
            dynamic businessObj = Newtonsoft.Json.JsonConvert.DeserializeObject(request.Business, businessType);
            businessObj.SetProvider(_provider);
            response.Id = businessObj.Save();
            return Task.FromResult(response);
        }

        public override Task<Protos.DeleteBusinessObjResponse> DeleteBusinessObj(Protos.DeleteBusinessObjRequest request, ServerCallContext context)
        {
            BusinessModel businessRegistry = BusinessManager.Instance.GetBusiness(request.BusinessName);
            var businessType = FindType(businessRegistry.Namespace + "." + businessRegistry.Name);
            dynamic businessObj = Activator.CreateInstance(businessType);
            businessObj.SetProvider(_provider);

            var response = new Protos.DeleteBusinessObjResponse();
            var result = businessObj.Get(request.Id);
            businessObj.BaseObj = result;
            response.Id = businessObj.Delete();
            return Task.FromResult(response);
        }

        public override Task<Protos.LoadResult> GetDataBusinessObj(Protos.GetDataBusinessObjRequest request, ServerCallContext context)
        {
            BusinessModel businessRegistry = BusinessManager.Instance.GetBusiness(request.BusinessName);
            var businessType = FindType(businessRegistry.Namespace + "." + businessRegistry.Name);
            dynamic businessObj = Activator.CreateInstance(businessType);
            businessObj.SetProvider(_provider);

            var result = businessObj.GetData(request.Skip, request.Take, request.Filter, request.OrderBy);
            var response = new Protos.LoadResult();
            response.TotalCount = result.TotalCount;
            response.GroupCount = result.GroupCount;
            response.Data.AddRange(((IEnumerable<object>)result.Data).Select(x => Newtonsoft.Json.JsonConvert.SerializeObject(x)));
            return Task.FromResult(response);
        }

        public override Task<ValidateAndSaveBusinessObjResponse> ValidateAndSaveBusinessObj(ValidateAndSaveBusinessObjRequest request, ServerCallContext context)
        {
            BusinessModel businessRegistry = BusinessManager.Instance.GetBusiness(request.BusinessName);
            var businessType = FindType(businessRegistry.Namespace + "." + businessRegistry.Name);
            //dynamic x = Activator.CreateInstance(businessType);
            //json deserialize using Newtonsoft.Json
            dynamic businessObj = Newtonsoft.Json.JsonConvert.DeserializeObject(request.Business, businessType);
            businessObj.SetProvider(_provider);

            var response = businessObj.ValidateAndSave();
            return Task.FromResult(response);

        }

        public override Task<Protos.MenuGroupsResponse> GetMenuGroups(Protos.GetMenuGroupsRequest request, ServerCallContext context)
        {
            dynamic dbFactory = _provider.GetService(typeof(IDbContextFactory<SDKContext>));
            List<E00130_MenuGroup> menuGroups = new();
            using (SDKContext dbContext = dbFactory.CreateDbContext())
            {
                menuGroups = dbContext.Set<E00130_MenuGroup>().AsQueryable().ToList();
            }

            var response = new Protos.MenuGroupsResponse
            {
                Response = JsonConvert.SerializeObject(menuGroups, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })
            };
            return Task.FromResult(response);
        }

        public override Task<Protos.MenuItemsResponse> GetMenuItems(Protos.GetMenuItemsRequest request, ServerCallContext context)
        {
            dynamic dbFactory = _provider.GetService(typeof(IDbContextFactory<SDKContext>));
            List<E00131_Menu> menuItems = new();
            using (SDKContext dbContext = dbFactory.CreateDbContext())
            {
                menuItems = dbContext.Set<E00132_MenuGroupDetail>().AsQueryable().Where(x => x.MenuGroup.Rowid == request.GroupId).Include(x => x.Menu.SubMenus).ThenInclude(x=>x.SubMenus).Select(x => x.Menu).ToList();
            }

            var response = new Protos.MenuItemsResponse
            {
                Response = JsonConvert.SerializeObject(menuItems, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })
            };
            return Task.FromResult(response);
        }

    }
}
