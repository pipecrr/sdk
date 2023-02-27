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
        static SDKApp()
        {
            AsembliesReg = new List<System.Reflection.Assembly>();
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new SDKContractResolver()
            };
        }
        public static void AddAssembly(System.Reflection.Assembly configuration)
        {
            AsembliesReg.Add(configuration);
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