
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siesa.SDK.Entities
{
    public class S001_Resource_Value: BaseEntity
    {
        public string ID { get; set; }
        public S001_Resource Resource { get; set; }
        public S003_Language Language { get; set; }
        public string Value { get; set; }
        public string CustomValue { get; set; }


    }
}
