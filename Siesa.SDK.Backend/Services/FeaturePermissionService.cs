using Siesa.SDK.Entities;
using Siesa.SDK.Shared.Criptography;
using System;
using System.Threading.Tasks;
using System.Linq;
using Siesa.SDK.Shared.Services;
using System.Collections.Generic;
using Siesa.SDK.Shared.Utilities;

namespace Siesa.SDK.Backend.Services
{
    public class FeaturePermissionService : IFeaturePermissionService
    {
        // private readonly IAuthenticationService _authenticationService;
        // private readonly ILocalStorageService _localStorageService;
        private IBackendRouterService _BackendRouter;
        public FeaturePermissionService(IBackendRouterService backendRouter)
        {
            _BackendRouter = backendRouter;
        }

        private Dictionary<string, int> BLNameToRowid { get; set; } = new Dictionary<string, int>();
        public bool CheckUserActionPermission(int rowidFeature, int actionRowid, IAuthenticationService authenticationService)
        {
            return Utilities.CheckUserActionPermission(rowidFeature, actionRowid, authenticationService);
        }

        public async Task<bool> CheckUserActionPermission(string featureBLName, int actionRowid, IAuthenticationService authenticationService)
        {
            //return true;
            if (!BLNameToRowid.ContainsKey(featureBLName))
            {
                var request = await _BackendRouter.GetSDKBusinessModel("BLFeature", authenticationService).Call("GetFeatureRowid", featureBLName);
                 if(request.Success)
                 {
                     BLNameToRowid[featureBLName] = request.Data;
                 }else{
                     return false;
                 }
            }
            return Utilities.CheckUserActionPermission(BLNameToRowid[featureBLName], actionRowid, authenticationService); 
        }
    }
}