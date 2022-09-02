using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Siesa.SDK.Frontend.Application;
using Siesa.SDK.Shared.Services;

namespace Siesa.SDK.Frontend.Components.FormManager.Model
{
    public class UtilsManager
    {
        private IAuthenticationService AuthenticationService;
        private IResourceManager ResourceManager;
        public UtilsManager(IAuthenticationService authenticationService, IResourceManager resourceManager)
        {
            this.AuthenticationService = authenticationService;
            this.ResourceManager = resourceManager;
        }

        public async Task<string> GetResource(Int64 resourceRowid, Int64 cultureRowid = 0){

            if(cultureRowid == 0){
                return await ResourceManager.GetResource(resourceRowid, cultureRowid);
            }
            if(AuthenticationService.User != null){
                //cultureRowid = await ResourceManager.GetCurrentCulture();
                cultureRowid = AuthenticationService.User.RowidCulture;
                return await ResourceManager.GetResource(resourceRowid, cultureRowid);
            }
            return "";
        }

        public async Task<string> GetResource(string resourceTag, Int64 cultureRowid = 0){
            if(cultureRowid != 0){
                return await ResourceManager.GetResource(resourceTag, cultureRowid);
            }
            if(AuthenticationService.User != null){
                //TODO: en el merge cambiar por GetRowidCulture
                cultureRowid = AuthenticationService.User.RowidCulture;
                return await ResourceManager.GetResource(resourceTag, cultureRowid);
            }
            return resourceTag;
        }
    }
}