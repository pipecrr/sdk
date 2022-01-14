
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siesa.SDK.Entities
{
    public class E00002_ResourceValue: BaseEntity
    {
        public virtual E00001_Resource Resource { get; set; }
        public virtual E00003_Language Language { get; set; }
        public string Value { get; set; }
        public string CustomValue { get; set; }

    }
}
