using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using Siesa.SDK.Frontend.Services;
using Siesa.SDK.Shared.DTOS;

namespace Siesa.SDK.Frontend.Components.Fields;
/// <summary>
/// A reusable Blazor component for displaying a grid of items using Radzen DataGrid.
/// </summary>
/// <typeparam name="TItem">The type of items in the grid.</typeparam>
public partial class SDKGrid<TItem> : SDKComponent
{
    /// <summary>
    /// Gets or sets a value indicating whether row selection is allowed when clicking on a row.
    /// </summary>
    [Parameter] public bool AllowRowSelectOnRowClick { get; set; } = false;
    /// <summary>
    /// Gets or sets a value indicating whether radio buttons should be displayed for row selection.
    /// </summary>
    [Parameter] public bool ShowRadioButtons { get; set; }
    /// <summary>
    /// Gets or sets the template to be displayed when the grid is empty.
    /// </summary>
    [Parameter] public RenderFragment? EmptyTemplate { get; set; }
    /// <summary>
    /// Gets or sets the template for defining columns in the grid.
    /// </summary>
    [Parameter] public RenderFragment? Columns { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether filtering is allowed in the grid.
    /// </summary>
    [Parameter] public bool AllowFiltering { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the grid is in a loading state.
    /// </summary>
    [Parameter] public bool Loading { get; set; }

    /// <summary>
    /// Gets or sets the number of items to display per page in the grid.
    /// </summary>
    [Parameter] public int PageSize { get; set; } = 10;

    /// <summary>
    /// Gets or sets a value indicating whether sorting is allowed in the grid.
    /// </summary>
    [Parameter] public bool AllowSorting { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether paging is allowed in the grid.
    /// </summary>
    [Parameter] public bool AllowPaging { get; set; }

    /// <summary>
    /// Gets or sets the data source for the grid.
    /// </summary>
    [Parameter] public IEnumerable<TItem> Data { get; set; }

    /// <summary>
    /// Gets or sets the total count of items in the grid.
    /// </summary>
    [Parameter] public int Count { get; set; }

    /// <summary>
    /// Gets or sets an event callback for loading data into the grid.
    /// </summary>
    [Parameter] public EventCallback<Radzen.LoadDataArgs> LoadData { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the paging summary should be displayed.
    /// </summary>
    [Parameter] public bool ShowPagingSummary { get; set; }

    /// <summary>
    /// Gets or sets the list of allowed page sizes for the grid.
    /// </summary>
    [Parameter] public IEnumerable<int> AllowedPageSizes { get; set; } = new int[] { 10, 20, 50, 100 };

    /// <summary>
    /// Gets or sets the selection mode for the grid.
    /// </summary>
    [Parameter] public SDKSelectionMode SelectionMode { get; set; }

    /// <summary>
    /// Gets or sets the list of selected items in the grid.
    /// </summary>
    [Parameter] public IList<TItem> Value { get; set; } = new List<TItem>();

    /// <summary>
    /// Gets or sets an event callback that is invoked when the selection changes.
    /// </summary>
    [Parameter] public EventCallback<IList<TItem>> ValueChanged { get; set; }

    /// <summary>
    /// Gets or sets a callback for handling selection change events.
    /// </summary>
    [Parameter] public Action<TItem> OnChangeSelect { get; set; }

    // Otros parámetros de configuración

    /// <summary>
    /// Gets or sets a value indicating whether the grid should be displayed in card view.
    /// </summary>
    [Parameter] public bool CardView { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether virtualization is allowed in the grid.
    /// </summary>
    [Parameter] public bool AllowVirtualization { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether column resizing is allowed in the grid.
    /// </summary>
    [Parameter] public bool AllowColumnResize { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the "Create" button should be displayed.
    /// </summary>
    [Parameter] public bool ShowButtonCreate { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the "Delete" button should be displayed.
    /// </summary>
    [Parameter] public bool ShowButtonDelete { get; set; }

    /// <summary>
    /// Gets or sets a callback for adding a new row to the grid return true if row will be creted.
    /// </summary>
    [Parameter] public Func<TItem, bool> OnAddRow { get; set; }

    /// <summary>
    /// Gets or sets a callback for deleting a row from the grid return true if row will be deleted.
    /// </summary>
    [Parameter] public Func<TItem, bool> OnDeleteRow { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the "Create" button is enabled.
    /// </summary>
    [Parameter] public bool EnabledCreate { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the "Delete" button is enabled.
    /// </summary>
    [Parameter] public bool EnabledDelete { get; set; } = true;

    /// <summary>
    /// Gets or sets the CSS class for the "Create" button.
    /// </summary>
    [Parameter] public string CssClassButtonCreate { get; set; } = "";
    private RadzenDataGrid<TItem> _grid;
    /// <summary>
    /// Get or sets that allow merge cells in the grid.
    /// </summary>
    [Parameter] public bool AllowMerge { get; set; }
    private Dictionary<string, List<SDKColumnMergeDTO>> _dicRowSpans = new Dictionary<string, List<SDKColumnMergeDTO>>();
    /// <summary>
    /// Gets or sets the event callback for changing data.
    /// </summary>
    /// <value></value>
    [Parameter] public EventCallback<IEnumerable<TItem>> OnChangeData { get; set; }
    /// <summary>
    /// Gets or sets the SDKNotificationService dependency.
    /// </summary>
    [Inject] public SDKNotificationService NotificationService {get; set;}

    /// <summary>
    /// This method is called when parameter values are set or changed.
    /// </summary>    
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if(ShowRadioButtons){
            AllowRowSelectOnRowClick = true;
        }
        if (Data != null && !LoadData.HasDelegate)
        {
            Count = Data.Count();
        }
    }

    // Helper method to get the value of a property from an item
    private static object GetValue(TItem item, string fieldName)
    {
       return item.GetType().GetProperty(fieldName)?.GetValue(item);
    }
    // Helper method to retrieve the RenderFragment for the action column
    private RenderFragment<TItem> GetActionColumn()
    {
       var query = _grid.ColumnsCollection.SingleOrDefault(x=> x.CssClass == "internal-sdk-gridcolumn-action");

       if (query != null)
       {
        var childContent = query.Template;
        return childContent;
       }
       return null;
    }
    /// <summary>
        /// Reloads the grid's data.
        /// </summary>
    public void Reload()
    {
        _grid.Reload();
    }
    // Helper method to check if an item is selected and invoke the OnChangeSelect callback
    private bool Check(TItem item){
        if(Value.Count == 0){
            //Custom.SDKGrid.ErrorImplementOnChangeData
            return false;
        }        
        if(Value[0].Equals(item)){
            OnChangeSelect?.Invoke(item);
            return true;
        }else{
            return false;
        }
    }
    /// <summary>
    /// Handles the click event for adding a new row to the grid.
    /// </summary>
    public async Task ClickAddRow()
    {
        if(!OnChangeData.HasDelegate){
            _ = NotificationService.ShowError("Custom.SDKGrid.ErrorImplementOnChangeData").ConfigureAwait(true);
            return;
        }
        bool result = true;
        Type dataType = typeof(TItem);
        dynamic obj = Activator.CreateInstance(dataType);
        if(OnAddRow != null){
              result = OnAddRow(obj);
        }
        if (result)
        {
            Data = Data.Append((TItem)obj);
            await _grid.Reload().ConfigureAwait(true);
            await InvokeAsync(StateHasChanged).ConfigureAwait(true);
            await OnChangeData.InvokeAsync(Data).ConfigureAwait(true);
        }
    }
    /// <summary>
    /// Handles the click event for deleting a row from the grid.
    /// </summary>
    /// <param name="item">The item to be deleted.</param>
    public async Task ClickDeleteRow(TItem item)
    {
        if(!OnChangeData.HasDelegate){
            _ = NotificationService.ShowError("Custom.SDKGrid.ErrorImplementOnChangeData").ConfigureAwait(true);
            return;
        }
        bool result = true;
        int index = Data.ToList().IndexOf(item);
        if(OnDeleteRow != null){
            result = OnDeleteRow(item);
        }
        if (result)
        {
            List<TItem> list = Data.ToList();
            list.RemoveAt(index);
            Data = list; 
            await _grid.Reload().ConfigureAwait(true);
            await InvokeAsync(StateHasChanged).ConfigureAwait(true);
            await OnChangeData.InvokeAsync(Data).ConfigureAwait(true);
        }
    }

    private void CellRender(DataGridCellRenderEventArgs<TItem> obj)
    {
        var property = obj.Column.Property;
        bool isActionColumn = (bool)(obj.Column.Template.Target?.GetType().GetProperty("IsActionColumn")?
                .GetValue(obj.Column.Template.Target) ?? false);
        if(property == null && !isActionColumn )
        {
            return;
        }
        bool mergeable = false;
        if(AllowMerge)
        {
            mergeable = (bool)(obj.Column.Template.Target?.GetType().GetProperty("Mergeable")?
                .GetValue(obj.Column.Template.Target) ?? false);
        }
        if(mergeable){
            if (isActionColumn)
            {
                var nextColumn = _grid.ColumnsCollection.SkipWhile(x => x.Property != property).Skip(1).FirstOrDefault(x => (bool)(x.Template.Target?.GetType().GetProperty("Mergeable")?.GetValue(x.Template.Target) ?? false));
                property = nextColumn?.Property;
            }

            if (property == null)
            {
                return;
            }
            CalculateRowspans(property);
            int rowspan = 1;
            string value = GetValue(obj.Data, property)?.ToString() ?? "";
            var list = _dicRowSpans[property];
            var item = list.SingleOrDefault(x => x.Value == value);
            if(item != null){
                rowspan = item.RowSpan;
            }
            obj.Attributes.Add("rowspan", rowspan);
        }
    }

    private void CalculateRowspans(string columnName)
    {
        if(_dicRowSpans.ContainsKey(columnName)){
            return;
        }
        var property = typeof(TItem).GetProperty(columnName);
        if(property == null)
        {
            return;
        }
        List<SDKColumnMergeDTO> list = new List<SDKColumnMergeDTO>();
        list = Data.GroupBy(x => property.GetValue(x)).Select(x => new SDKColumnMergeDTO { Value = x.Key.ToString(), RowSpan = x.Count() }).ToList();
        _dicRowSpans.Add(columnName, list);
    }
}