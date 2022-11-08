using System.ComponentModel.DataAnnotations;

namespace Siesa.SDK.Shared.DTOS
{
    public class SDKResourceIndexedDbDTO{
        public int Rowid { get; set; }
        public int RowidResource { get; set; }
        public short RowidCulture { get; set; }
        public string IdResource { get; set; }
        public string Description { get; set; }
    }
}