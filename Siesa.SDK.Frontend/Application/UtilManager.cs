using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazor.IndexedDB.Framework;
using Blazored.LocalStorage;
using Newtonsoft.Json;
using Siesa.SDK.Frontend.Application;
using Siesa.SDK.Frontend.Data;
using Siesa.SDK.Frontend.Utils;
using Siesa.SDK.Shared.Services;

namespace Siesa.SDK.Frontend.Components.FormManager.Model
{
    public class UtilsManager
    {
        private IAuthenticationService AuthenticationService;
        private IResourceManager ResourceManager;
        private ILocalStorageService _localStorageService;
        private IIndexedDbFactory _dbFactoryService;
        private Dictionary<Int64, Dictionary<string, string>> resourceValuesDictUtil = new Dictionary<Int64, Dictionary<string, string>>();
        public UtilsManager(IAuthenticationService authenticationService, IResourceManager resourceManager, ILocalStorageService localStorageService, IIndexedDbFactory dbFactoryService)
        {
            AuthenticationService = authenticationService;
            ResourceManager = resourceManager;
            _localStorageService = localStorageService;
            _dbFactoryService = dbFactoryService;
        }

        public async Task<string> GetResource(Int64 resourceRowid, Int64 cultureRowid = 0){

            if(cultureRowid != 0){
                return await this.ResourceManager.GetResource(resourceRowid, cultureRowid);
            }            
            if(AuthenticationService.User != null){
                cultureRowid = AuthenticationService.GetRowidCulture();
                return await ResourceManager.GetResource(resourceRowid, cultureRowid);
            }
            return "";
        }

        public async Task<string> GetResource(string resourceTag, Int64 rowidculture = 0){
            if (rowidculture != 0)
            {
                return await ResourceManager.GetResource(resourceTag, rowidculture);
            }else if(AuthenticationService.User != null){
                Int64 cultureRowid = AuthenticationService.GetRowidCulture();
                return await ResourceManager.GetResource(resourceTag, cultureRowid);
            }
            return resourceTag;
        }

        public async Task<string> GetResourceFlex(string resourceTag, Int64 cultureRowid = 0){
            resourceValuesDictUtil = ResourceManager.GetResourceValuesDict();
            if(resourceValuesDictUtil.Count == 0){
                return resourceTag;
            }
            if(cultureRowid != 0){
                if(resourceValuesDictUtil.ContainsKey(cultureRowid)){
                    if(resourceValuesDictUtil[cultureRowid].ContainsKey(resourceTag)){
                        return resourceValuesDictUtil[cultureRowid][resourceTag];
                    }
                }
            }
            if(AuthenticationService.User != null){
                cultureRowid = AuthenticationService.GetRowidCulture();
                if(resourceValuesDictUtil.ContainsKey(cultureRowid)){
                    if(resourceValuesDictUtil[cultureRowid].ContainsKey(resourceTag)){
                        return resourceValuesDictUtil[cultureRowid][resourceTag];
                    }
                }
            }
            return resourceTag;
        }       

        public async Task AddResourceLocalStorage(int rowidCulture){
            var resource = ResourceManager.GetResourceByCulture(rowidCulture);
            var resourceJson = JsonConvert.SerializeObject(resource);
            string encriptResource = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(resourceJson));
            await _localStorageService.SetItemAsync("resources", encriptResource);
        }

        public async Task<Dictionary<byte,string>> GetEnumValues(string enumName, Int64 rowidCulture = 0){

            if(rowidCulture != 0){
                return await ResourceManager.GetEnumValues(enumName, rowidCulture);
            }
            if(AuthenticationService.User != null){
                rowidCulture = AuthenticationService.GetRowidCulture();
                return await ResourceManager.GetEnumValues(enumName, rowidCulture);
            }
            return null;
        }
    }
}