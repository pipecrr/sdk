using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

namespace Siesa.SDK.Frontend.Components.Visualization;

/// <summary>
/// Partial class representing a generic scheduler with options for displaying various time views.
/// </summary>
/// <typeparam name="TItem">The type of items to be displayed in the scheduler.</typeparam>
public partial class SDKScheduler<TItem>
{
    /// <summary>
    /// Gets or sets a value indicating whether the daily view should be shown in the scheduler. Default is true.
    /// </summary>
    [Parameter]
    public bool ShowDayView { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the weekly view should be shown in the scheduler. Default is true.
    /// </summary>
    [Parameter]
    public bool ShowWeekView { get; set; } = true;


    /// <summary>
    /// Gets or sets a value indicating whether the monthly view should be shown in the scheduler. Default is true.
    /// </summary>
    [Parameter]
    public bool ShowMonthView { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the yearly view should be shown in the scheduler. Default is true.
    /// </summary>
    [Parameter]
    public bool ShowYearView { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the year planner view should be shown in the scheduler. Default is true.
    /// </summary>
    [Parameter]
    public bool ShowYearPlannerView { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the year timeline view should be shown in the scheduler. Default is true.
    /// </summary>
    [Parameter]
    public bool ShowYearTimelineView { get; set; } = true;

    /// <summary>
    /// Gets or sets the child content of the scheduler. Use to specify what views to render.
    /// </summary>
    /// <value>The child content.</value>

    [Parameter]
    public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the template used to render appointments.
    /// </summary>
    /// <value>The template.</value>
    [Parameter]
    public RenderFragment<TItem> Template { get; set; }

    /// <summary>
    /// Gets or sets the data of RadzenScheduler. It will display an appointment for every item of the  collection which is within the current view date range.
    /// </summary>
    /// <value>The data.</value>
    [Parameter]
    public IEnumerable<TItem> Data { get; set; }

    /// <summary>
    /// Specifies the property of <typeparamref name="TItem" /> which will set 
    /// <see cref="AppointmentData.Start" />.
    /// </summary>
    /// <value>The name of the property. Must be a <c>DateTime</c> property.</value>
    [Parameter]
    public string StartProperty { get; set; }

    /// <summary>
    /// Specifies the property of <typeparamref name="TItem" /> which will set 
    /// <see cref="AppointmentData.End" />.
    /// </summary>
    /// <value>The name of the property. Must be a <c>DateTime</c> property.</value>
    [Parameter]
    public string EndProperty { get; set; }

    /// <summary>
    /// Specifies the initially selected view.
    /// </summary>
    /// <value>The index of the selected.</value>
    [Parameter]
    public int SelectedIndex { get; set; }

    /// <summary>
    /// Gets or sets the text of the today button. Set to <c>Today</c> by default.
    /// </summary>
    /// <value>The today text.</value>
    [Parameter]
    public string TodayText { get; set; } = "Today";

    /// <summary>
    /// Gets or sets the initial date displayed by the selected view. Set to <c>DateTime.Today</c> by default.
    /// </summary>
    /// <value>The date.</value>
    [Parameter]
    public DateTime Date { get; set; } = DateTime.Today;

    /// <summary>
    /// Gets or sets the current date displayed by the selected view. Initially set to 
    /// <see cref="Date" />. Changes during navigation.
    /// </summary>
    /// <value>The current date.</value>
    public DateTime CurrentDate { get; set; }

    /// <summary>
    /// Specifies the property of <typeparamref name="TItem" /> which will set 
    /// <see cref="AppointmentData.Text" />.
    /// </summary>
    /// <value>The name of the property. Must be a <c>DateTime</c> property.</value>

    [Parameter]
    public string TextProperty { get; set; }

    /// <summary>
    /// A callback that will be invoked when the user clicks a slot in the current view. Commonly used to add new appointments.
    /// </summary>

    [Parameter]
    public EventCallback<SchedulerSlotSelectEventArgs> SlotSelect { get; set; }

    /// <summary>
    /// A callback that will be invoked when the user clicks an appointment in the current view. Commonly used to edit existing appointments.
    /// </summary>

    [Parameter]
    public EventCallback<SchedulerAppointmentSelectEventArgs<TItem>> AppointmentSelect { get; set; }

    /// <summary>
    /// A callback that will be invoked when the user clicks the more text in the current view. Commonly used to view additional appointments.
    /// Invoke the <see cref="SchedulerMoreSelectEventArgs.PreventDefault"/> method to prevent the default action (showing the additional appointments).
    /// </summary>

    [Parameter]
    public EventCallback<SchedulerMoreSelectEventArgs> MoreSelect { get; set; }

    /// <summary>
    /// An action that will be invoked when the current view renders an appointment. Never call <c>StateHasChanged</c> when handling AppointmentRender.
    /// </summary>

    [Parameter]
    public Action<SchedulerAppointmentRenderEventArgs<TItem>> AppointmentRender { get; set; }

    /// <summary>
    /// An action that will be invoked when the current view renders an slot. Never call 
    /// <c>StateHasChanged</c> when handling SlotRender.
    /// </summary>

    [Parameter]
    public Action<SchedulerSlotRenderEventArgs> SlotRender { get; set; }

    /// <summary>
    /// A callback that will be invoked when the scheduler needs data for the current view. Commonly used to filter the
    /// data assigned to <see cref="Data" />.
    /// </summary>

    [Parameter]
    public EventCallback<SchedulerLoadDataEventArgs> LoadData { get; set; }

    private RadzenScheduler<TItem> scheduler;

    public async Task Reload()
    {
        if (scheduler != null)
        {
            await scheduler.Reload().ConfigureAwait(true);
        }
    }



    /*public ISchedulerView SelectedView
    {
        get
        {
            return Views.ElementAtOrDefault(SelectedIndex);
        }
    }

    

    private async Task InvokeLoadData()
    {
        if (SelectedView != null)
        {
            await LoadData.InvokeAsync(new SchedulerLoadDataEventArgs { Start = SelectedView.StartDate, End = SelectedView.EndDate }).ConfigureAwait(true);
        }
    }*/



}
