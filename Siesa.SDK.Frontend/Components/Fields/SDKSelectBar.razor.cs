using Microsoft.AspNetCore.Components;
using System.Collections.Generic;


namespace Siesa.SDK.Frontend.Components.Fields
{
    public partial class SDKSelectBar : ComponentBase
    {

        [Parameter] public IEnumerable<SelectBarItemWrap> Data { get; set; }

        //bool singleValue = false;

        [Parameter] public int Value { get; set; }

        [Parameter] public EventCallback<int> ValueChanged {get; set;}
    }
}
