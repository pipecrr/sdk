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
        private Dictionary<string, int> BLNameToRowid { get; set; } = new Dictionary<string, int>();
        public BusinessFrontendModel Backend {get { return Frontend.BusinessManagerFrontend.Instance.GetBusiness("BLFeature", null); } }

        public bool CheckUserActionPermission(int rowidFeature, int actionRowid, IAuthenticationService authenticationService)
        {
            return Utilities.CheckUserActionPermission(rowidFeature, actionRowid, authenticationService);
        }

        public async Task<bool> CheckUserActionPermission(string featureBLName, int actionRowid, IAuthenticationService authenticationService)
        {
            if (!BLNameToRowid.ContainsKey(featureBLName))
            {
               var request = await Backend.Call("GetFeatureRowid", featureBLName);
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