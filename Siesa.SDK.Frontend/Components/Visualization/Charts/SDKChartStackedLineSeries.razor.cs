using System;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Shared.DTOS;
using System.Threading.Tasks;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Frontend.Services;
using Siesa.SDK.Entities;
using System.Collections.Generic;
using System.Linq;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using DevExpress.Blazor;
using System.Drawing;
using System.Linq.Expressions;

namespace Siesa.SDK.Frontend.Components.Visualization.Charts;

public partial class SDKChartStackedLineSeries<TData, TArgument, TValue> : SDKComponent
{
    // Colección de datos para la serie del gráfico.
    [Parameter]
    public IEnumerable<TData> Data { get; set; }

    // Expresión que representa el campo de argumentos de la serie del gráfico.
    [Parameter]
    public Expression<Func<TData, TArgument>> ArgumentField { get; set; }

    // Configuración específica de la serie del gráfico.
    [Parameter]
    public ChartSeriesSettings<TData, TValue, TArgument> Settings { get; set; }

    // Expresión que representa el campo de valores de la serie del gráfico.
    [Parameter]
    public Expression<Func<TData, TValue>> ValueField { get; set; }

    // Define el eje asociado a la serie del gráfico.
    [Parameter]
    public string Axis { get; set; }

    // Modo de visualización al pasar el mouse sobre la serie del gráfico.
    [Parameter]
    public ChartContinuousSeriesHoverMode HoverMode { get; set; }

    // Modo de selección de la serie del gráfico.
    [Parameter]
    public ChartContinuousSeriesSelectionMode SelectionMode { get; set; }

    // Determina si se debe interrumpir en puntos vacíos.
    [Parameter]
    public bool BreakOnEmptyPoints { get; set; }

    // Color de la serie del gráfico.
    [Parameter]
    public Color Color { get; set; }

    // Expresión para filtrar los datos de la serie del gráfico.
    [Parameter]
    public Expression<Func<TData, bool>> Filter { get; set; }

    // Define el panel asociado a la serie del gráfico.
    [Parameter]
    public string Pane { get; set; }

    // Nombre de la serie del gráfico.
    [Parameter]
    public string Name { get; set; }

    // Contenido adicional que se puede incrustar en el componente.
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}