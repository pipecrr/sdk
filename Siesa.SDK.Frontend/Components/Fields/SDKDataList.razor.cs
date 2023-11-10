using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Siesa.SDK.Frontend.Components;
using Siesa.SDK.Frontend.Components.Visualization;
using Siesa.SDK.Shared.Application;
using Microsoft.Extensions.DependencyInjection;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Shared.Utilities;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading;
using Siesa.SDK.Shared.DTOS;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using DevExpress.Data.Mask.Internal;
using Siesa.SDK.Frontend.Components.Layout;
using Siesa.Global.Enums;
using Siesa.SDK.Frontend.Services;
namespace Siesa.SDK.Frontend.Components.Fields;
public partial class SDKDataList : SDKComponent
{
    [Parameter]
    public bool WrapItems { get; set; }
    [Parameter]
    public bool AllowVirtualization { get; set; }
    [Parameter]
    public bool IsLoading { get; set; }

}