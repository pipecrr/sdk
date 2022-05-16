using System.Threading.Tasks;

namespace Siesa.SDK.Shared.Services
{
    public interface IFeaturePermissionService
    {
        public bool CheckUserActionPermission(int rowidFeature, int actionRowid, IAuthenticationService authenticationService);
        public Task<bool> CheckUserActionPermission(string FeatureBLName, int actionRowid, IAuthenticationService authenticationService);
    }
}