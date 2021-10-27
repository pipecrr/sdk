using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siesa.SDK.Frontend.Application
{
    public static class SDKApp
    {
        public static List<System.Reflection.Assembly> AsembliesReg { get; set; }

        static SDKApp()
        {
            AsembliesReg = new List<System.Reflection.Assembly>();
        }

        public static void AddAssembly(System.Reflection.Assembly configuration)
        {
            AsembliesReg.Add(configuration);
        }
    }
}
