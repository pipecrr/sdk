using System;
using System.Collections.Generic;
using Siesa.Global.Enums;

namespace Siesa.SDK.Shared.DTOS;

public class ModelMessagesDTO
{
   public string Message { get; set; }
   public string PropertyName { get; set; }
   public string StackTrace { get; set; } 
   public EnumModelMessageType TypeMessange { get; set; } = EnumModelMessageType.Error;
   public Dictionary<string, List<string>> MessageFormat { get; set; }
}
