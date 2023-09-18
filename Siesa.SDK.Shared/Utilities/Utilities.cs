
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Components.Forms;
using Siesa.SDK.Shared.Application;
using Siesa.SDK.Shared.DataAnnotations;
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

        /// <summary>
        /// Checks if the user has permission to perform a specified action on a business entity.
        /// </summary>
        /// <param name="businessName">The name of the business entity.</param>
        /// <param name="actionRowid">The identifier of the action.</param>
        /// <param name="authenticationService">The authentication service providing user information.</param>
        /// <returns>
        /// Returns true if the user has permission to perform the action; otherwise, returns false.
        /// </returns>
        public static bool CheckUserActionPermission(string businessName, int actionRowid,
            IAuthenticationService authenticationService)
        {
            try
            {
                if (authenticationService == null || authenticationService.User == null)
                {
                    return false;
                }

                if (!authenticationService.User.FeaturePermissions.ContainsKey(businessName))
                {
                    return false;
                }

                if (!authenticationService.User.FeaturePermissions[businessName].Contains(actionRowid) &&
                    !authenticationService.User.FeaturePermissions[businessName].Contains(-999))
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

        /// <summary>
        /// This method is used to get if the business object is a document or not.
        /// </summary>
        /// <param name="businessObj">Business object of which you want to know if it is a document</param>
        /// <param name="documentType">Type BLFrontendDocument</param>
        /// <returns></returns>
        public static bool CheckIsDocument(dynamic businessObj, Type documentType)
        {
            bool result = false;
            Type businessType = businessObj.GetType();

            if (businessType.BaseType != null && businessType.BaseType.IsGenericType &&
                businessType.BaseType.GetGenericTypeDefinition() == documentType)
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Searches for an assembly containing a specified type by its name.
        /// </summary>
        /// <param name="type_name">The name of the type to search for.</param>
        /// <returns>
        /// Returns an <see cref="Assembly"/> containing the specified type, if found; otherwise, returns null.
        /// </returns>
        public static Assembly SearchAssemblyByType(string type_name)
        {
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

        /// <summary>
        /// Searches for a type with the specified name within registered assemblies and optionally all loaded assemblies.
        /// </summary>
        /// <param name="name">The name of the type to search for.</param>
        /// <param name="fullSearch">Indicates whether to perform a search within all loaded assemblies.</param>
        /// <returns>
        /// Returns a <see cref="Type"/> representing the found type, if available; otherwise, returns null.
        /// </returns>
        public static Type SearchType(string name, bool fullSearch = false)
        {
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

            if (fullSearch)
            {
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
            if (type != null)
            {
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
            catch (Exception)
            {
                return null;
            }

        }

        public static byte[] ReadAssemblyResourceBytes(Assembly asm, string name)
        {
            try
            {
                string resourcePath = name;
                resourcePath = asm.GetManifestResourceNames().Single(str => str.EndsWith(name));
                using (Stream stream = asm.GetManifestResourceStream(resourcePath))
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }


            }
            catch (Exception)
            {
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
            catch (Exception)
            {
                return null;
            }

        }

        public static object CreateCurrentData(dynamic data, string[] fieldPath, Type typeBaseSDK)
        {
            object currentData = data;
            for (int i = 0; i < (fieldPath.Length - 1); i++)
            {
                var tmpType = currentData.GetType();
                var propertyName = fieldPath[i];
                var propertyIndex = -1;
                if (propertyName.Contains("["))
                {
                    var index = propertyName.IndexOf("[");
                    propertyIndex = int.Parse(propertyName.Substring(index + 1, propertyName.Length - index - 2));
                    propertyName = propertyName.Substring(0, index);
                }

                var tmpProperty = tmpType.GetProperty(propertyName);
                var tmpValue = tmpProperty.GetValue(currentData, null);
                if (propertyIndex > -1)
                {
                    tmpValue = ((System.Collections.IList)tmpValue)[propertyIndex];
                }

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

        private static string GetUName(Type EntityType)
        {
            var dataAnnotation = EntityType.GetCustomAttributes(typeof(SDKAuthorization), false);

            var TableName = string.Empty;

            if (dataAnnotation.Length > 0)
            {
                //Get the table name
                TableName = ((SDKAuthorization)dataAnnotation[0]).TableName;

                if (!string.IsNullOrEmpty(TableName))
                    return TableName;
            }

            //Get table name from the context
            TableName = EntityType.Name;

            //Replace the first character of the table name with the letter "u"
            if (TableName.Length > 0)
                TableName = $"U{TableName.Substring(1)}";

            TableName = $"{EntityType.Namespace}.{TableName}";
            return TableName;
        }

        public static Type GetVisibilityType(Type EntityType)
        {
            var TableName = GetUName(EntityType);
            var UTable = EntityType.Assembly.GetType(TableName);
            return UTable;
        }
    }
}
