
using Siesa.SDK.Backend.Access;

namespace Siesa.SDK.Shared.DTOS
{
    public class SDKDbConnection
    {
        public short Rowid { get; set; }
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public EnumDBType ProviderName { get; set; }
        public string LogoUrl {get; set;}
        public string StyleUrl {get; set;}
        public bool IsTest {get ; set;} = false;
    }
}