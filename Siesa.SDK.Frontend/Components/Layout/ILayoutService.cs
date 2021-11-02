using Microsoft.AspNetCore.Components;
using Siesa.SDK.Frontend.Components.FormManager.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siesa.SDK.Frontend.Components.Layout
{
    public interface ILayoutService
    {
        RenderFragment TopBarTitle { get; }
        RenderFragment TopBarButtons { get; }
        SetTopBar TopBarSetter { get; set; }
        event PropertyChangedEventHandler PropertyChanged;
        void UpdateTopBar();
    }
}
