using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Newtonsoft.Json;
using Siesa.SDK.Frontend.Application;
using Siesa.SDK.Shared.Services;

namespace Siesa.SDK.Frontend.Components.FormManager.Model
{
    public class UtilsManager
    {
        private IAuthenticationService AuthenticationService;
        private IResourceManager ResourceManager;
        private ILocalStorageService _localStorageService;
        private Dictionary<Int64, Dictionary<string, string>> resourceValuesDictUtil = new Dictionary<Int64, Dictionary<string, string>>();
        public UtilsManager(IAuthenticationService authenticationService, IResourceManager resourceManager, ILocalStorageService localStorageService)
        {
            AuthenticationService = authenticationService;
            ResourceManager = resourceManager;
            _localStorageService = localStorageService;
        }

        public async Task<string> GetResource(Int64 resourceRowid, Int64 cultureRowid = 0){

            if(cultureRowid != 0){
                return await this.ResourceManager.GetResource(resourceRowid, cultureRowid);
            }            
            if(AuthenticationService.User != null){
                cultureRowid = AuthenticationService.GetRoiwdCulture();
                return await ResourceManager.GetResource(resourceRowid, cultureRowid);
            }
            return "";
        }

        public async Task<string> GetResource(string resourceTag, Int64 rowidculture = 0){
            if (rowidculture != 0)
            {
                return await ResourceManager.GetResource(resourceTag, rowidculture);
            }else if(AuthenticationService.User != null){
                Int64 cultureRowid = AuthenticationService.GetRoiwdCulture();
                return await ResourceManager.GetResource(resourceTag, cultureRowid);
            }
            return resourceTag;
        }

        public async Task GetResourceByContainId(Int64 cultureRowid = 0){                
                if(cultureRowid != 0){
                    await this.ResourceManager.GetResourceByContainId(cultureRowid);
                }
                if(AuthenticationService.User != null){
                    cultureRowid = AuthenticationService.GetRoiwdCulture();
                    await ResourceManager.GetResourceByContainId(cultureRowid);
                }
                resourceValuesDictUtil = ResourceManager.GetResourceValuesDict();
        }

        public async Task<string> GetResourceFlex(string resourceTag, Int64 cultureRowid){
            if(resourceValuesDictUtil.Count == 0){
                return resourceTag;
            }
            if(resourceValuesDictUtil.ContainsKey(cultureRowid)){
                if(resourceValuesDictUtil[cultureRowid].ContainsKey(resourceTag)){
                    return resourceValuesDictUtil[cultureRowid][resourceTag];
                }
            }
            return resourceTag;
        }

        public async Task AddResourceLocalStorage(int rowidCulture){
            var resource = ResourceManager.GetResourceByCulture(rowidCulture).Result;
            var resourceJson = JsonConvert.SerializeObject(resource);
            string encriptResource = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(resourceJson));
            await _localStorageService.SetItemAsync("resources", encriptResource);
        }

        
    }
}   
