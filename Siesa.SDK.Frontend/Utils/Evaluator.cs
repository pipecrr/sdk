using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.Scripting;
using System.Globalization;

namespace Siesa.SDK.Frontend.Utils
{
    public static class Evaluator
    {
        private static dynamic ProcessProperty(object context, string singleProperty)
        {
            //check if is a property of the context
            var contextType = context.GetType();

            string[] fieldPath = singleProperty.Split('.');
            if(fieldPath.Length > 1){
                if(double.TryParse(singleProperty, NumberStyles.Any ,CultureInfo.InvariantCulture, out double doubleNumber)){
                    return doubleNumber;
                }else if(decimal.TryParse(singleProperty, NumberStyles.Any ,CultureInfo.InvariantCulture, out decimal decimalNumber)){
                    return decimalNumber;
                }

                var field = contextType.GetProperty(fieldPath[0]);
                if(field != null){
                    var fieldValue = field.GetValue(context);
                    if(fieldValue != null){
                        return ProcessProperty(fieldValue, string.Join(".", fieldPath.Skip(1)));
                    }else{
                        throw new ArgumentException($"Field '{fieldPath[0]}' is null in type '{contextType.FullName}'.");
                    }
                }else{
                    throw new ArgumentException($"Field '{fieldPath[0]}' not found in type '{contextType.FullName}'.");
                }
            }
            // Console.WriteLine($"Evaluating property '{singleProperty}' in type '{contextType.FullName}'.");
            var property = contextType.GetProperty(singleProperty);
            if(property != null){
                return property.GetValue(context);
            }else{
                var _method = contextType.GetMethod(singleProperty);
                if(_method != null){
                    //return delegate to execute
                    var parameters = _method.GetParameters();
                    List<Type> delegateTypes = new List<Type>();
                    foreach(var parameter in parameters){
                        delegateTypes.Add(parameter.ParameterType);
                    }
                    delegateTypes.Add(_method.ReturnType);
                    return Delegate.CreateDelegate(Expression.GetDelegateType(delegateTypes.ToArray()), context, _method);
                }
                //check if is a number
                if(int.TryParse(singleProperty, out int number)){
                    return number;
                }else if(double.TryParse(singleProperty, NumberStyles.Any ,CultureInfo.InvariantCulture, out double doubleNumber)){
                    return doubleNumber;
                }else if(decimal.TryParse(singleProperty, NumberStyles.Any ,CultureInfo.InvariantCulture, out decimal decimalNumber)){
                    return decimalNumber;
                }else if(DateTime.TryParse(singleProperty, out DateTime date)){
                    return date;
                }else if(TimeSpan.TryParse(singleProperty, out TimeSpan time)){
                    return time;
                }else if(Guid.TryParse(singleProperty, out Guid guid)){
                    return guid;
                }else if(singleProperty.StartsWith("'") && singleProperty.EndsWith("'")){
                    return singleProperty.Substring(1, singleProperty.Length - 2).ToCharArray()[0];
                }else{
                    //try json parse
                    try{
                        var json = Newtonsoft.Json.JsonConvert.DeserializeObject(singleProperty);
                        return json;
                    }catch(Exception ex){
                        //ignore
                    }
                    //check if is a reserved word
                    switch(singleProperty){
                        case "true":
                            return true;
                        case "false":
                            return false;
                        case "null":
                            return null;
                        default:
                            throw new ArgumentException($"Property '{singleProperty}' not found in type '{contextType.FullName}'.");
                    }
                }
            }
        }

        public static async Task<object> EvaluateCode(string code, object globals, bool useRoslyn = false)
        {
            if(useRoslyn)
            {
                object result;
                try
                {
                    result = await CSharpScript.EvaluateAsync(code, options: ScriptOptions.Default.WithImports("System"), globals: globals);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"Error evaluating code: '{code}'.", ex);
                }finally{
                    GC.Collect(); //TODO: Check performance
                }

                return result;
            }else{
                // check if code starts with "await"
                var asyncCode = code.Trim().StartsWith("await ");
                if (asyncCode)
                {
                    code = code.Substring(6);
                }

                var contextType = globals.GetType();
                var methodPattern = @"^([\w\.]+)\((.*)\)$|^([\w\.]+)$";
                var match = Regex.Match(code.Trim(), methodPattern);

                if (!match.Success)
                {
                    throw new ArgumentException($"Invalid code format: '{code}'.");
                }
                var methodName = match.Groups[1].Value;
                var arguments = match.Groups[2].Value.Split(',').Select(arg => arg.Trim()).ToArray();
                var singleProperty = match.Groups[3].Value.Trim();
                if(string.IsNullOrEmpty(singleProperty))
                {
                    //remove empty arguments
                    arguments = arguments.Where(arg => !string.IsNullOrEmpty(arg)).ToArray();
                    var evalMethod = contextType.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                        .FirstOrDefault(m => m.Name == methodName && m.GetParameters().Length == arguments.Length);

                    if (evalMethod == null)
                    {
                        throw new ArgumentException($"Method '{methodName}' not found in type '{contextType.FullName}' or does not have {arguments.Length} parameters.");
                    }

                    var parameters = evalMethod.GetParameters();
                    var convertedArguments = new object[arguments.Length];

                    for (int i = 0; i < arguments.Length; i++)
                    {
                        var parameterType = parameters[i].ParameterType;
                        object argumentValue = null;
                        try
                        {
                            argumentValue = ProcessProperty(globals, arguments[i]);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            argumentValue = arguments[i];
                        }
                        try
                        {
                            argumentValue = Convert.ChangeType(argumentValue, parameterType);
                        }catch(Exception ex){
                            //ignore
                        }
                        
                        convertedArguments[i] = argumentValue;
                    }

                    if (asyncCode)
                    {
                        var task = (Task)evalMethod.Invoke(globals, convertedArguments);
                        await task;
                        return task.GetType().GetProperty("Result").GetValue(task);
                    }

                    var result = evalMethod.Invoke(globals, convertedArguments);
                    return result;
                }else{
                    return ProcessProperty(globals, singleProperty);
                }
            }
        }
    }
}
