using System.Collections.Generic;

namespace Siesa.SDK.Shared.DTOS
{
    public class EnumSearchDTO
    {
        public string EnumName { get; set; }
        public string PropertyName { get; set; }
        public Dictionary<string, object> EnumValues { get; set; }
    }
}