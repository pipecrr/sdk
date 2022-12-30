using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Siesa.SDK.Frontend.Extension
{
    public static class ParameterViewExtensions
    {
        public static bool DidParameterChange<T>(this ParameterView parameters, string parameterName, T parameterValue)
        {
            if (parameters.TryGetValue(parameterName, out T value))
            {
                return !EqualityComparer<T>.Default.Equals(value, parameterValue);
            }

            return false;
        }
    }
}