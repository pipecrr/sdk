using System.Collections.Generic;
using Siesa.SDK.Shared.Services;

namespace Siesa.SDK.Shared.Criptography
{
    public interface ISDKJWT
    {
        public string Generate<T>(T obj, long min, Dictionary<string, List<int>>? featurePermissions = null, List<SessionRol> roles = null,  short rowIdDBConnection = 0, short rowidcompanygroup =0);

        public T Validate<T>(string token);
    }
}