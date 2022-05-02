
using System;
using System.Collections.Generic;


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

        public static string GetDinamycWhere(Dictionary<string, object> inDictionary, Dictionary<string, object> PrimaryKey)
        {

            string filter = string.Empty;

            foreach (var field in inDictionary)
            {
                if (!String.IsNullOrEmpty(filter)) filter += " AND ";

                filter += $"{field.Key}==\"{field.Value}\"";

            }

            foreach (var keyPropertie in PrimaryKey)
            {
                if (keyPropertie.Value != null)
                {

                    if (!String.IsNullOrEmpty(filter)) filter += " AND ";

                    filter += $"{keyPropertie.Key}!=\"{keyPropertie.Value}\"";
                }

            }

            return filter;

        }

    }
}
