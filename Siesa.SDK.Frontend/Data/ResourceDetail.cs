using System.ComponentModel.DataAnnotations;

namespace Siesa.SDK.Frontend.Data
{
    public class ResourceDetail{
        [Key]
        public int Rowid { get; set; }
        public string IdCulture { get; set; }
        public string IdResource { get; set; }
        public string Description { get; set; }
    }
}