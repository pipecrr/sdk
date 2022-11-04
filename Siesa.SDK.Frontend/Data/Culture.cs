using System.ComponentModel.DataAnnotations;

namespace Siesa.SDK.Frontend.Data
{
    public class Culture{
        [Key]
        public int Rowid { get; set; }
        public string Id { get; set; }
    }
}