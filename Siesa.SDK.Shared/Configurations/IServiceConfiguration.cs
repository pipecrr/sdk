using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siesa.SDK.Shared.Configurations
{
    public interface IServiceConfiguration
    {
        string MasterBackendUrl { get; set; }
        string AuditServerUrl { get; set; }
        string CurrentUrl { get; set; }

        string GetCurrentUrl();
    }
}
