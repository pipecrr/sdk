using Microsoft.CodeAnalysis.CSharp.Scripting;
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
                result = await CSharpScript.EvaluateAsync(code, globals: globals);
            }
            catch (Exception)
            {
                Console.WriteLine("error, eval");
                return null;
            }

            return result;
        }
    }
}
