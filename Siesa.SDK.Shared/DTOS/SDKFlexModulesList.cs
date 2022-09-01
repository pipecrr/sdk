using System.Collections.Generic;

namespace Siesa.SDK.Shared.DTOS
{
   public class SDKFlexModulesList
{
    public string Label { get; set; }
    public List<SDKFlexModuleListOption> options { get; set; }
}
}