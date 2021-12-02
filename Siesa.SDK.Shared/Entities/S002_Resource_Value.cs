
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siesa.SDK.Entities
{
    public class S002_Resource_Value: BaseEntity
    {
        public S001_Resource Resource { get; set; }
        public S003_Language Language { get; set; }
        public string Value { get; set; }
        public string CustomValue { get; set; }

    }
}
