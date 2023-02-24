using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Siesa.SDK.Frontend.Services;
using Siesa.SDK.Shared.Json;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Shared.Utilities;

namespace Siesa.SDK.Frontend.Application
{
    public interface IViewdefManager {
        public string GetViewdef(string businessName, string viewName);
    }
    public class ViewdefManager: IViewdefManager
    {
        private Dictionary<string, string> _viewdefs = new();
        public ViewdefManager(IConfiguration configuration)
        {
        }

        private string _GetViewdef(string businessName, string viewName)
        {
            var business = BackendRouterService.Instance.GetBackend(businessName);
            if (business == null) {
                return null;
            }
            var asm = Utilities.SearchAssemblyByType(business.Namespace + "." + business.Name);
            if (asm == null)
            {
                return null;
            }
            return Utilities.ReadAssemblyResource(asm, business.Name + ".Viewdefs."+ viewName + ".json");
        }

        public string GetViewdef(string businessName, string viewName)
        {
            var key = businessName + "." + viewName;
            if (!_viewdefs.ContainsKey(key))
            {
                _viewdefs[key] = _GetViewdef(businessName, viewName);
            }
            return _viewdefs[key];
        }
    }
}
