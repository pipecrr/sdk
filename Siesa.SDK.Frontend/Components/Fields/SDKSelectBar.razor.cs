using Microsoft.AspNetCore.Components;
using System.Collections.Generic;


namespace Siesa.SDK.Frontend.Components.Fields
{
    public class SelectBarItemWrap
    {
        public int Key { get; set; }
        public string Name { get; set; }
    }
    public partial class SDKSelectBar : ComponentBase
    {

        [Parameter]
        public IEnumerable<SelectBarItemWrap> Data { get; set; }
        //bool singleValue = false;
        int selectedValue = 0;
    }
}
