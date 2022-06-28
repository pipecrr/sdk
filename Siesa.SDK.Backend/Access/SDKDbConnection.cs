
namespace Siesa.SDK.Backend.Access
{
    public class SDKDbConnection
    {
        public short Rowid { get; set; }
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public EnumDBType ProviderName { get; set; }
    }
}