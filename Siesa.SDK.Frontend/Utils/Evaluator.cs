using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siesa.SDK.Frontend.Utils
{
    public static class Evaluator
    {
        public static async Task<object> EvaluateCode(string code, object globals)
        {
            object result;
            try
            {
                result = await CSharpScript.EvaluateAsync(code, options: ScriptOptions.Default.WithImports("System"), globals: globals);
            }
            catch (Exception ex)
            {
                Console.WriteLine("error, eval");
                return null;
            }

            return result;
        }
    }
}
