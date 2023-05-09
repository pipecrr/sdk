
using System;
using System.Collections.Generic;

namespace Siesa.SDK.Shared.DTOS
{
    public class DynamicSelectBarDetailDTO
    {
        public string Icon {get; set;}
        public string ResourceTag {get{
            Type ValueType = Value.GetType();
            var EnumValue = ValueType.GetEnumName(Value);
            var Id = $"Enum.{ValueType.Name}.{EnumValue}";
            return Id;
        }}
        public string IconColor {get; set;}
        public bool On {get; set;}
        public Enum Value {get; set;}
    }
}