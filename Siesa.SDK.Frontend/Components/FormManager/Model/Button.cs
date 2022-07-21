using System;
using System.Collections.Generic;
using DevExpress.Blazor;
using Siesa.SDK.Components.Visualization;
using Siesa.SDK.Frontend.Components.FormManager.Model.Fields;

namespace Siesa.SDK.Frontend.Components.FormManager.Model
{
    public class Button
    {
        public string ResourceTag { get; set; }

        public SDKButtonRenderStyle RenderStyle { get{
                return Style switch
                {
                    "primary" => SDKButtonRenderStyle.Primary,
                    "secondary" => SDKButtonRenderStyle.Secondary,
                    "success" => SDKButtonRenderStyle.Success,
                    "danger" => SDKButtonRenderStyle.Danger,
                    "warning" => SDKButtonRenderStyle.Warning,
                    "info" => SDKButtonRenderStyle.Info,
                    "light" => SDKButtonRenderStyle.Light,
                    "dark" => SDKButtonRenderStyle.Dark,
                    _ => SDKButtonRenderStyle.Secondary,
                };
            }
        } //TODO: Encapsular SDKButtonRenderStyle
        public string Style { get; set; } 
        public string Action { get; set; }

        public string Target { get; set; } = "_self";
        public string Href { get; set; }

        public string IconClass { get; set; }
    }
}
