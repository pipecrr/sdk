using System.Collections.Generic;

namespace Siesa.SDK.Shared.DTOS
{
    public class SDKFlexRelationships
    {
        public List<object> one_to_many { get; set; }
        public List<SDKFlexManyToMany> many_to_many { get; set; }
        public List<SDKFlexManyToOne> many_to_one { get; set; }
    }
}