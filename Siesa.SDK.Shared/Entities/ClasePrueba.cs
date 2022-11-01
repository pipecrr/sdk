using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Siesa.SDK.Entities
{
    [Keyless]
    public class ClasePrueba : BaseSDK<int>
    {
        [Key]
        [Required]
        public override int Rowid { get; set; }
        public string Nombre { get; set; }
    }
}