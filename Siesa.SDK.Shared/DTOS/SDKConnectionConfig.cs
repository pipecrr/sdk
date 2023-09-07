
using System.Collections.Generic;
using Siesa.SDK.Backend.Access;
using Siesa.SDK.Shared.Services;

namespace Siesa.SDK.Shared.DTOS
{
    public class SDKConnectionConfig
    {
        public bool IsRemote { get; set; }
        public string ApiUrl { get; set; }
        public string Token { get; set; }
        public JwtUserData User { get; set; }
    }
}