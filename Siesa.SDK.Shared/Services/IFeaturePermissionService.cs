using System.Collections.Generic;
using System.Threading.Tasks;

namespace Siesa.SDK.Shared.Services
{
    public interface IFeaturePermissionService
    {
        public bool CheckUserActionPermission(string businessName, int actionRowid, IAuthenticationService authenticationService);
        public bool CheckUserActionPermissions(string businessName, List<int> permissions, IAuthenticationService authenticationService);
    }
}