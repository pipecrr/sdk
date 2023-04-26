
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Siesa.SDK.Shared.Application;
using Siesa.SDK.Shared.Services;

namespace Siesa.SDK.Shared.Utilities
{
    public static class Utilities
    {
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

        public static bool CheckUserActionPermission(string businessName, int actionRowid, IAuthenticationService authenticationService)
        {
            try
            {
                if(authenticationService == null || authenticationService.User == null)
                {
                    return false;
                }

                if(!authenticationService.User.FeaturePermissions.ContainsKey(businessName))
                {
                    return false;
                }

                if(!authenticationService.User.FeaturePermissions[businessName].Contains(actionRowid) && !authenticationService.User.FeaturePermissions[businessName].Contains(-999))
                {
                    return false;
                }
                return true;

            }
            catch (System.Exception)
            {
                return false;
            }
            
        }

        public static Assembly SearchAssemblyByType(string type_name) {
            Type type = Type.GetType(type_name);
            if (type != null)
            {
                return Assembly.GetExecutingAssembly();
            }
            foreach (var asm in SDKApp.AsembliesReg)
            {
                type = asm.GetType(type_name);
                if (type != null)
                    return asm;
            }
            return null;
        }
        //TODO: Refactorizar esos 2 métodos
        public static Type SearchType(string name, bool fullSearch = false) {
            Type type = Type.GetType(name);
            if (type != null)
            {
                return type;
            }
            foreach (var asm in SDKApp.AsembliesReg)
            {
                type = asm.GetType(name);
                if (type != null)
                    return type;
            }

            if(fullSearch){
                foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
                {
                    
                    type = ass.GetType(name);
                    if (type != null)
                        return type;
                }
            }

            
            return null;
        }

        public static object GetInstance(string strFullyQualifiedName, bool fullSearch = false)
        {
            Type type = Utilities.SearchType(strFullyQualifiedName, fullSearch);
            if (type != null) {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        public static string ReadAssemblyResource(Assembly asm, string name)
        {
            try
            {
                string resourcePath = name;
                resourcePath = asm.GetManifestResourceNames().Single(str => str.EndsWith(name));
                using (Stream stream = asm.GetManifestResourceStream(resourcePath))
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception e) {
                return null;
            }

        }
        public static List<string> GetAssemblyResources(Assembly asm, string path)
        {
            try
            {
                string fullpath = $"{asm.GetName().Name}.{path}";
                return asm.GetManifestResourceNames().Where(str => str.StartsWith(fullpath)).ToList();
                
            }
            catch (Exception e) {
                return null;
            }

        }

        public static object CreateCurrentData(dynamic data, string[] fieldPath, Type typeBaseSDK)
        {
            object currentData = data;
            for (int i = 0; i < (fieldPath.Length - 1); i++)
            {
                var tmpType = currentData.GetType();
                var tmpProperty = tmpType.GetProperty(fieldPath[i]);
                var tmpValue = tmpProperty.GetValue(currentData, null);
                var isEntity = Utilities.IsAssignableToGenericType(tmpProperty.PropertyType, typeBaseSDK);
                if (tmpValue == null && isEntity)
                {
                    tmpValue = Activator.CreateInstance(tmpProperty.PropertyType);
                    tmpProperty.SetValue(currentData, tmpValue);
                }
                currentData = tmpValue;
            }
            return currentData;
        }
    }
}
