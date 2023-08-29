using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Siesa.SDK.Shared.Json;
using Microsoft.Extensions.DependencyInjection;
using Siesa.SDK.Shared.Backend;
using Siesa.SDK.Shared.Services;



namespace Siesa.SDK.Shared.Application
{
    /// <summary>
    /// Provides a set of static methods and properties to manage the SDK application.
    /// </summary>
    public static class SDKApp
    {
        private static IServiceProvider _serviceProvider;
        //TODO: Solución temporal
        private static Type _indexComponent;

        /// <summary>
        /// Gets or sets the list of assemblies registered with the SDK application.
        /// </summary>
        public static List<System.Reflection.Assembly> AsembliesReg { get; set; }

        /// <summary>
        /// Gets or sets the list of dashlets registered with the SDK application.
        /// </summary>
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

        /// <summary>
        /// Adds an assembly to the list of registered assemblies.
        /// </summary>
        /// <param name="configuration">The assembly to add.</param>
        public static void AddAssembly(System.Reflection.Assembly configuration)
        {
            AsembliesReg.Add(configuration);
            RegisterDashletsAssembly(configuration);
        }

        private static void RegisterDashletsAssembly(System.Reflection.Assembly frontAssembly)
        {
            //search for components with fullname match this regex "BL*.Dashlets."
            var pattern = @".*\.Dashlets\.BL.*\..*";
            var dashlets = frontAssembly.GetTypes()
            .Where(t => System.Text.RegularExpressions.Regex.IsMatch(t.FullName, pattern)
            && t.GetCustomAttributes(typeof(DataAnnotations.SDKDashlet), false).Length > 0);
            if (dashlets != null)
            {
                Dashlets.AddRange(dashlets);
            }

        }

        /// <summary>
        /// Sets the service provider for the SDK application.
        /// </summary>
        /// <param name="serviceProvider">The service provider to set.</param>
        public static void SetServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Gets the service provider for the SDK application.
        /// </summary>
        /// <returns>The service provider for the SDK application.</returns>
        public static IServiceProvider GetServiceProvider()
        {
            return _serviceProvider;
        }

        /// <summary>
        /// Sets the index component for the SDK application.
        /// </summary>
        /// <param name="indexComponent">The index component to set.</param>
        public static void SetIndexComponent(Type indexComponent)
        {
            _indexComponent = indexComponent;
        }

        /// <summary>
        /// Gets the index component for the SDK application.
        /// </summary>
        /// <returns>The index component for the SDK application.</returns>
        public static Type GetIndexComponent()
        {
            return _indexComponent;
        }

        /// <summary>
        /// Ejecuta la suscripción a colas para una lista de tipos de negocio.
        /// </summary>
        /// <param name="BusinessList">Lista de tipos de negocio para los cuales se realizará la suscripción.</param>
        public static void ExecuteSubscribeToQueues(IEnumerable<Type> BusinessList)
        {
            try
            {
                foreach (var bl in BusinessList)
                {
                    var BLMethod = Siesa.SDK.Shared.Utilities.Utilities.SearchType($"{bl.Namespace}.{bl.Name}", true).GetMethod("SubscribeToQueues");

                    if (BLMethod != null && _serviceProvider != null && BLMethod.GetBaseDefinition().DeclaringType != BLMethod.DeclaringType)
                    {
                        Type blType = Siesa.SDK.Shared.Utilities.Utilities.SearchType($"{bl.Namespace}.{bl.Name}", true);
                        var blInstance = ActivatorUtilities.CreateInstance(_serviceProvider, blType);
                        BLMethod.Invoke(blInstance, null);
                    }
                }
            }
            catch ()
            {
                Console.WriteLine("Error SDKAPP");
            }
        }
    }
}
