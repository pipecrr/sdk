using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Siesa.SDK.Frontend.Application;
namespace Siesa.SDK.Frontend.Utils
{
    public static class Utils
    {

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

        public static object GetInstance(string strFullyQualifiedName)
        {
            Type type = Utils.SearchType(strFullyQualifiedName);
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
    }
}
