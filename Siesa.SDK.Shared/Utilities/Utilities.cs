
using System;
using System.Collections.Generic;
using Siesa.SDK.Shared.Services;

namespace Siesa.SDK.Shared.Utilities
{
    public class Utilities
    {
        public Utilities()
        {

        }

        public static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            Type baseType = givenType.BaseType;
            if (baseType == null) return false;

            return IsAssignableToGenericType(baseType, genericType);
        }

        public static bool CheckUserActionPermission(int rowidFeature, int actionRowid, IAuthenticationService authenticationService)
        {
            try
            {
                if(authenticationService == null || authenticationService.User == null)
                {
                    return false;
                }

                if(!authenticationService.User.FeaturePermissions.ContainsKey(rowidFeature))
                {
                    return false;
                }

                if(!authenticationService.User.FeaturePermissions[rowidFeature].ContainsKey(actionRowid))
                {
                    return false;
                }else{
                    return true;
                }
                
            }
            catch (System.Exception)
            {
                return false;
            }
            
        }

    }
}
