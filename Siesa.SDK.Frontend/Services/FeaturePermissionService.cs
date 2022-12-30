using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Entities;
using Siesa.SDK.Shared.Criptography;
using System;
using System.Threading.Tasks;
using System.Linq;
using Siesa.SDK.Shared.Services;
using System.Collections.Generic;
using Siesa.SDK.Shared.Utilities;

namespace Siesa.SDK.Frontend.Services
{
    public class FeaturePermissionService : IFeaturePermissionService
    {
        // private readonly IAuthenticationService _authenticationService;
        // private readonly ILocalStorageService _localStorageService;
        protected IBackendRouterService _BackendRouter {get; set;}

        public FeaturePermissionService(IBackendRouterService backendRouterService)
        {
            _BackendRouter = backendRouterService;
            // _authenticationService = authenticationService;
            // _localStorageService = localStorageService;
        }
        private Dictionary<string, int> BLNameToRowid { get; set; } = new Dictionary<string, int>();

        public bool CheckUserActionPermission(int rowidFeature, int actionRowid, IAuthenticationService authenticationService)
        {
            return Utilities.CheckUserActionPermission(rowidFeature, actionRowid, authenticationService);
        }

        public async Task<bool> CheckUserActionPermission(string featureBLName, int actionRowid, IAuthenticationService authenticationService)
        {
            if (!BLNameToRowid.ContainsKey(featureBLName))
            {
                var backend = _BackendRouter.GetSDKBusinessModel("BLFeature", authenticationService);
                var request = await backend.Call("GetFeatureRowid", featureBLName);
                if(request.Success)
                {
                    BLNameToRowid[featureBLName] = request.Data;
                }else{
                    return false;
                }
            }
            return Utilities.CheckUserActionPermission(BLNameToRowid[featureBLName], actionRowid, authenticationService); 
        }

        public async Task<bool> CheckUserActionPermissions(string FeatureBLName, List<int> permissions, IAuthenticationService authenticationService){
            var result = false;
            foreach(var item in permissions){
                result = await CheckUserActionPermission(FeatureBLName, item, authenticationService);
                if(!result){
                    break;
                }
            }
            return result;
        }
    }
}