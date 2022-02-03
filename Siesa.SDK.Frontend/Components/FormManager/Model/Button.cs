using System;
using System.Collections.Generic;
using DevExpress.Blazor;
using Siesa.SDK.Frontend.Components.FormManager.Model.Fields;

namespace Siesa.SDK.Frontend.Components.FormManager.Model
{
    public class Button
    {
        public string Label { get; set; }

        public ButtonRenderStyle RenderStyle { get{
                return Style switch
                {
                    "primary" => ButtonRenderStyle.Primary,
                    "secondary" => ButtonRenderStyle.Secondary,
                    "success" => ButtonRenderStyle.Success,
                    "danger" => ButtonRenderStyle.Danger,
                    "warning" => ButtonRenderStyle.Warning,
                    "info" => ButtonRenderStyle.Info,
                    "light" => ButtonRenderStyle.Light,
                    "dark" => ButtonRenderStyle.Dark,
                    _ => ButtonRenderStyle.Secondary,
                };
            }
        } //TODO: Encapsular SDKButtonRenderStyle
        public string Style { get; set; } 
        public string Action { get; set; }

        public string Target { get; set; } = "_self";
        public string Href { get; set; }
    }
}
