using System;
using DevExpress.Blazor;

namespace Siesa.SDK.Components.Visualization
{
    public static class SDKButtonExtensions {
        public static ButtonRenderStyle Get(this SDKButtonRenderStyle renderStyle)
        {
            return renderStyle switch
            {
                SDKButtonRenderStyle.None => ButtonRenderStyle.None,
                SDKButtonRenderStyle.Primary => ButtonRenderStyle.Primary,
                SDKButtonRenderStyle.Secondary => ButtonRenderStyle.Secondary,
                SDKButtonRenderStyle.Info => ButtonRenderStyle.Info,
                SDKButtonRenderStyle.Link => ButtonRenderStyle.Link,
                SDKButtonRenderStyle.Success => ButtonRenderStyle.Success,
                SDKButtonRenderStyle.Warning => ButtonRenderStyle.Warning,
                SDKButtonRenderStyle.Danger => ButtonRenderStyle.Danger,
                SDKButtonRenderStyle.Cancel => ButtonRenderStyle.Dark,
                SDKButtonRenderStyle.Light => ButtonRenderStyle.Light,
                _ =>  ButtonRenderStyle.Primary
            };
        }

        public static ButtonRenderStyleMode Get(this SDKButtonRenderStyleMode renderStyle)
        {
            return renderStyle switch
            {
                SDKButtonRenderStyleMode.Contained => ButtonRenderStyleMode.Contained,
                SDKButtonRenderStyleMode.Outline => ButtonRenderStyleMode.Outline,
                SDKButtonRenderStyleMode.Text => ButtonRenderStyleMode.Text
            };
        }


    }
    public enum SDKButtonRenderStyle
    {
        None = ButtonRenderStyle.None,
        Primary = ButtonRenderStyle.Primary,
        Secondary = ButtonRenderStyle.Secondary,
        Info = ButtonRenderStyle.Info,
        Link = ButtonRenderStyle.Link,
        Success = ButtonRenderStyle.Success,
        Warning = ButtonRenderStyle.Warning,
        Danger = ButtonRenderStyle.Danger,
        Cancel = ButtonRenderStyle.Dark,
        Light = ButtonRenderStyle.Light
    } 
    
    public enum SDKButtonRenderStyleMode
    {
        Contained = ButtonRenderStyleMode.Contained,
        Outline = ButtonRenderStyleMode.Outline,
        Text = ButtonRenderStyleMode.Text
    }
}

