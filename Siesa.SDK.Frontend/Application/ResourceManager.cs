using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Siesa.SDK.Frontend.Services;
using Siesa.SDK.Shared.Json;
using Siesa.SDK.Shared.Services;

namespace Siesa.SDK.Frontend.Application
{
    public interface IResourceManager {
        public Task<string> GetResource(Int64 resourceRowid, Int64 cultureRowid);
        public Task<string> GetResource(string resourceTag, Int64 cultureRowid);

        public Task<string> GetResource(Int64 resourceRowid, IAuthenticationService authenticationService);
        public Task<string> GetResource(string resourceTag, IAuthenticationService authenticationService);

        public Task<string> GetResourcesByModule(Int64 moduleRowid, Int64 cultureRowid);
        public Task<string> GetEnumValues(string enumName, Int64 cultureRowid, Int64 moduleRowid);

    }
    public class ResourceManager: IResourceManager
    {
        Dictionary<Int64, Dictionary<string, string>> resourceValuesDict = new Dictionary<Int64, Dictionary<string, string>>();

        Dictionary<Int64, string> resourceDict = new Dictionary<Int64, string>();

        public SDKBusinessModel Backend {get { return BackendRouterService.Instance.GetSDKBusinessModel("BLResource", null); } }

        public ResourceManager()
        {
        }

        public async Task<string> GetResource(Int64 resourceRowid, Int64 cultureRowid)
        {
            //check if cultureRowid is in resourceValuesDict
            if (!resourceValuesDict.ContainsKey(cultureRowid))
            {
                resourceValuesDict[cultureRowid] = new Dictionary<string, string>();
            }

            //check if resourceRowid is in resourceDict
            if (!resourceDict.ContainsKey(resourceRowid))
            {
                var request = await Backend.Call("GetResourceId", resourceRowid);
                if(request.Success)
                {
                    resourceDict[resourceRowid] = request.Data;
                }else{
                    return "Resource Not Found";
                }
            }

            string resourceTag = resourceDict[resourceRowid];
            return await GetResource(resourceTag, cultureRowid);
        }
        public async Task<string> GetResource(string resourceTag, Int64 cultureRowid)
        {
            //check if cultureRowid is in resourceValuesDict
            if (!resourceValuesDict.ContainsKey(cultureRowid))
            {
                resourceValuesDict[cultureRowid] = new Dictionary<string, string>();
            }

            //check if resourceTag is in cache
            if (!resourceValuesDict[cultureRowid].ContainsKey(resourceTag))
            {
                var request = await Backend.Call("GetResource", resourceTag, cultureRowid);
                if(request.Success)
                {
                    resourceValuesDict[cultureRowid][resourceTag] = request.Data;
                }else{
                    return $"{resourceTag}";
                }
            }

            return resourceValuesDict[cultureRowid][resourceTag];
        }

        public async Task<string> GetResource(Int64 resourceRowid, IAuthenticationService authenticationService)
        {
            if(authenticationService != null & authenticationService.User != null && authenticationService.GetRoiwdCulture() != 0){
                return await GetResource(resourceRowid, authenticationService.GetRoiwdCulture());
            }
            return  "Invalid User";
        }

        public async Task<string> GetResource(string resourceTag, IAuthenticationService authenticationService)
        {
            if(authenticationService != null & authenticationService.User != null && authenticationService.GetRoiwdCulture() != 0){
                return await GetResource(resourceTag, authenticationService.GetRoiwdCulture());
            }
            return "Invalid User";
        }
        
        public async Task<string> GetResourcesByModule(Int64 moduleRowid, Int64 cultureRowid)
        {
            return "";
        }
        public async Task<string> GetEnumValues(string enumName, Int64 cultureRowid, Int64 moduleRowid)
        {
            return "";
        }
    }
}
