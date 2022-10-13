using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Siesa.SDK.Backend.Extensions
{
    public static class ReflectionExtensions
	{   
		public static IEnumerable<MethodInfo> GetExtensionMethods(this Type type, Assembly extensionsAssembly)
		{
			var query = from t in extensionsAssembly.GetTypes()
						where !t.IsGenericType && !t.IsNested
						from m in t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
						where m.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false)
						where m.GetParameters()[0].ParameterType.Name == type.Name
						select m;

			return query;
		}

		public static MethodInfo GetExtensionMethod(this Type type, Assembly extensionsAssembly, string name)
		{
			return type.GetExtensionMethods(extensionsAssembly).FirstOrDefault(m => m.Name == name);
		}

		public static MethodInfo GetExtensionMethod(this Type type, Assembly extensionsAssembly, string name, Type[] types, bool IsGeneric = false)
		{
			var methods = (from m in type.GetExtensionMethods(extensionsAssembly)
						where m.Name == name
						&& m.GetParameters().Count() == types.Length
						&& (!IsGeneric || m.ContainsGenericParameters)
						select m).ToList();

			if (!methods.Any())
			{
				return default(MethodInfo);
			}

			if (methods.Count() == 1)
			{
				return methods.First();
			}

			foreach (var methodInfo in methods)
			{
				var parameters = methodInfo.GetParameters();

				bool found = false;
				for (byte b = 0; b < types.Length; b++)
				{
					if (parameters[b].ParameterType == types[b] || (types[b].IsGenericType && types[b].Name == parameters[b].ParameterType.Name))
					{
						found = true;
					}else{
						found = false;
						break;
					}
				}

				if (found)
				{
					return methodInfo;
				}
			}

			return default(MethodInfo);
		}
	} 
}