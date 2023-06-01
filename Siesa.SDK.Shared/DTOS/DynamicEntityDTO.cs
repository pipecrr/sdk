using System.Collections.Generic;

namespace Siesa.SDK.Shared.DTOS
{
    public class DynamicEntityDTO<U>
    {
        public int Rowid { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public dynamic DynamicObject { get; set; }
        public Dictionary<string, DynamicEntityFieldsDTO<U>> Fields { get; set; }
    }
}