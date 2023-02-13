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
        public bool CheckUserActionPermission(string businessName, int actionRowid, IAuthenticationService authenticationService)
        {
            return Utilities.CheckUserActionPermission(businessName, actionRowid, authenticationService);
        }
        public bool CheckUserActionPermissions(string businessName, List<int> permissions, IAuthenticationService authenticationService){
            var result = false;
            return true;
            foreach(var item in permissions){
                result = CheckUserActionPermission(businessName, item, authenticationService);
                if(!result){
                    break;
                }
            }
            return result;
        }
    }
}