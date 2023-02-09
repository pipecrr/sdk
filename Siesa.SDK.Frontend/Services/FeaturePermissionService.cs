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
        public bool CheckUserActionPermission(string businessName, int actionRowid, IAuthenticationService authenticationService)
        {
            
            return true;
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