
using System.Collections.Generic;

namespace Siesa.SDK.Shared.DTOS
{
    public class MapFkRelationsImportDTO

    {
        public string IdFieldFkEntity { get; set; }
        public string DependencyIdFieldFKEntity{ set;get;}
        public string NameInternalIndex { get; set; }
        public List<string> ListIndexForeingFields { get; set; }  = new();
    }
}