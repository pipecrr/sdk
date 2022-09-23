

using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Siesa.SDK.Frontend.Components.Documentation
{

public class ComponentCategory{
     
    public string Name {get; set;}
    public List<ComponentDemo> Components {get; set;} = new List<ComponentDemo>();

    public string Icon {get; set;}

}
public class ComponentDemo {

    public string ComponentName {get; set;}

    public RenderFragment ComponentFragment {get;set;}
}
    
}