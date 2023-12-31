using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Shared.Json;

namespace Siesa.SDK.Shared.Services
{
    public interface IResourceManager {
        public Task<string> GetResource(Int64 resourceRowid, Int64 cultureRowid);
        public Task<string> GetResource(string resourceTag, Int64 cultureRowid);

        public Task<string> GetResource(Int64 resourceRowid, IAuthenticationService authenticationService);
        public Task<string> GetResource(string resourceTag, IAuthenticationService authenticationService);

        //public Task<string> GetResourcesByModule(Int64 moduleRowid, Int64 cultureRowid);
        public Dictionary<string, string> GetResourceByCulture(int rowidCulture);
        //public Task<string> GetEnumValues(string enumName, Int64 cultureRowid, Int64 moduleRowid);
        public Task<Dictionary<byte,string>> GetEnumValues(string enumName, Int64 cultureRowid);
        Dictionary<long, Dictionary<string, string>> GetResourceValuesDict();
    }
    public class ResourceManager: IResourceManager
    {
        Dictionary<Int64, Dictionary<string, string>> resourceValuesDict = new Dictionary<Int64, Dictionary<string, string>>();

        Dictionary<Int64, string> resourceDict = new Dictionary<Int64, string>();

        public SDKBusinessModel Backend {get { return BackendRouterServiceBase.Instance.GetSDKBusinessModel("BLResource", null); } }

        private IConfiguration Configuration;

        private bool IsInitialized = false;
        public ResourceManager(IConfiguration configuration, bool getAllResource = true)
        {
            if(getAllResource)
            {
                Configuration = configuration;
                try
                {
                    var defaultRowidCulture = Configuration["DefaultRowidCulture"] ?? "1";
                    var cultureRowid = Convert.ToInt64(defaultRowidCulture);
                    _ = this.GetAllResources(cultureRowid);
                }
                catch (System.Exception)
                {
                    Console.WriteLine("no se pudo obtener la cultura por defecto");
                    IsInitialized = true;
                }
            }
            else
            {
                IsInitialized = true;
            }

        }

        public async Task GetAllResources(Int64 cultureRowid)
        {
            try
            {
                var request = await Backend.Call("GetAllResources", cultureRowid);
                if(request.Success)
                {
                    var resources = request.Data as List<Tuple<Int64, string, string>>;
                    if(resources != null)
                    {
                        if (!resourceValuesDict.ContainsKey(cultureRowid))
                        {
                            resourceValuesDict[cultureRowid] = new Dictionary<string, string>();
                        }
                        foreach (var resource in resources)
                        {
                            resourceValuesDict[cultureRowid][resource.Item2] = resource.Item3;
                            resourceDict[resource.Item1] = resource.Item2;
                        }
                    }
                }
            }
            catch (System.Exception)
            {
                Console.WriteLine("failed to get all resources");
            }finally
            {
                IsInitialized = true;
            }
        }

        public async Task<string> GetResource(Int64 resourceRowid, Int64 cultureRowid)
        {
            if(!IsInitialized)
            {
                //wait until initialized is true
                while(!IsInitialized)
                {
                    await Task.Delay(100);
                }
            }
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
            if(resourceTag.StartsWith("SDKDev-"))
            {
                //remove SDKDev- prefix
                return resourceTag.Substring(7);
            }
            if(!IsInitialized)
            {
                //wait until initialized is true
                while(!IsInitialized)
                {
                    await Task.Delay(100);
                }
            }
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
            if(authenticationService != null && authenticationService.GetRowidCulture() != 0){
                return await GetResource(resourceRowid, authenticationService.GetRowidCulture());
            }
            return  "Invalid User";
        }

        public async Task<string> GetResource(string resourceTag, IAuthenticationService authenticationService)
        {
            if(authenticationService != null && authenticationService.GetRowidCulture() != 0){
                return await GetResource(resourceTag, authenticationService.GetRowidCulture());
            }
            return "Invalid User";
        }
        
        // public async Task<string> GetResourcesByModule(Int64 moduleRowid, Int64 cultureRowid)
        // {
        //     return "";
        // }
        // public async Task<string> GetEnumValues(string enumName, Int64 cultureRowid, Int64 moduleRowid)
        // {
        //     return "";
        // }

        public async Task<Dictionary<byte, string>> GetEnumValues(string enumName, Int64 cultureRowid)
        {
            var request = await Backend.Call("GetEnumValues", enumName, cultureRowid);

            if(request.Success)
            {
                return request.Data as Dictionary<byte, string>;
            }
            return null;
        }

        public Dictionary<string, string> GetResourceByCulture(int rowidCulture)
        {            
            var resource = resourceValuesDict[rowidCulture];
            return resource;
        }

        public Dictionary<long, Dictionary<string, string>> GetResourceValuesDict()
        {
            return resourceValuesDict;
        }
    }
}
