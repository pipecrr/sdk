using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Frontend.Components.Fields;
using Siesa.SDK.Frontend.Components.FormManager.Model.Fields;

namespace Siesa.SDK.Frontend.Components.FormManager.Fields
{
    public partial class CustomField : ComponentBase
    {
        [Parameter] public CustomComponent Definition { get; set; }
        [Parameter] public FieldOptions FieldOpt { get; set; }
        [Parameter] public object BindModel { get; set; }
        [Parameter] public string FieldName { get; set; }
        [Parameter] public object BaseModelObj { get; set; }
        [Parameter] public object BaseObj { get; set; }

        private Type componentType;
        private IDictionary<string, object> parameters = new Dictionary<string, object>();
        private RenderFragment RenderGemericComponent()
        {
            if (componentType != null)
            {
                return (builder) =>
                {
                    Type genericField = componentType.MakeGenericType(Definition.Generics.First());
                    builder.OpenComponent(0, genericField);
                    for (int i = 0; i < parameters.Count; i++)
                    {
                        var key = parameters.ElementAt(i).Key;
                        var value = parameters.ElementAt(i).Value;
                        builder.AddAttribute(i+1, key, value);
                    }
                    builder.CloseComponent();
                };
            }
            else
            {
                return null;
            }
        }
    }


}