using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Siesa.SDK.Shared.Json;
namespace Siesa.SDK.Shared.Application
{
    public static class SDKApp
    {
        private static IServiceProvider _serviceProvider;
        //TODO: Solución temporal
        private static Type _indexComponent;
        public static List<System.Reflection.Assembly> AsembliesReg { get; set; }
        public static List<Type> Dashlets { get; set; }
        static SDKApp()
        {
            AsembliesReg = new List<System.Reflection.Assembly>();
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new SDKContractResolver()
            };
            Dashlets = new List<Type>();
        }
        public static void AddAssembly(System.Reflection.Assembly configuration)
        {
            AsembliesReg.Add(configuration);
            RegisterDashletsAssembly(configuration);
        }

        private static void RegisterDashletsAssembly(System.Reflection.Assembly frontAssembly)
        {
           //search for components with fullname match this regex "BL*.Dashlets."
            var pattern = @".*\.BL.*\.Dashlets\..*";
            var dashlets = frontAssembly.GetTypes().Where(t => System.Text.RegularExpressions.Regex.IsMatch(t.FullName, pattern));
            if (dashlets != null)
            {
                Dashlets.AddRange(dashlets);
            }
            
        }
        public static void SetServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public static IServiceProvider GetServiceProvider()
        {
            return _serviceProvider;
        }
        public static void SetIndexComponent(Type indexComponent)
        {
            _indexComponent = indexComponent;
        }

        public static Type GetIndexComponent()
        {
            return _indexComponent;
        }
    }
}