using Microsoft.AspNetCore.Components;
using Siesa.SDK.Frontend.Components;
using Blazored.Toast.Configuration;



namespace Siesa.SDK.Frontend.Components.Visualization
{

    /// <summary>
    /// Represents a Toast component with customizable settings for displaying toast notifications.
    /// </summary>
    public partial class SDKToast : SDKComponent
    {
        /// <summary>
        /// Gets or sets the type of icon to be used in the toast notifications.
        /// </summary>
        [Parameter] public IconType IconType { get; set; } = IconType.Blazored;

        /// <summary>
        /// Gets or sets the CSS class to be applied to the info toast notifications.
        /// </summary>
        [Parameter] public string? InfoClass { get; set; }

        /// <summary>
        /// Gets or sets the icon to be used for info toast notifications.
        /// </summary>
        [Parameter] public string? InfoIcon { get; set; }

        /// <summary>
        /// Gets or sets the CSS class to be applied to the success toast notifications.
        /// </summary>
        [Parameter] public string? SuccessClass { get; set; }

        /// <summary>
        /// Gets or sets the icon to be used for success toast notifications.
        /// </summary>
        [Parameter] public string? SuccessIcon { get; set; }

        /// <summary>
        /// Gets or sets the CSS class to be applied to the warning toast notifications.
        /// </summary>
        [Parameter] public string? WarningClass { get; set; }

        /// <summary>
        /// Gets or sets the icon to be used for warning toast notifications.
        /// </summary>
        [Parameter] public string? WarningIcon { get; set; }

        /// <summary>
        /// Gets or sets the CSS class to be applied to the error toast notifications.
        /// </summary>
        [Parameter] public string? ErrorClass { get; set; }

        /// <summary>
        /// Gets or sets the icon to be used for error toast notifications.
        /// </summary>
        [Parameter] public string? ErrorIcon { get; set; }

        /// <summary>
        /// Gets or sets the position where the toast notifications will be displayed.
        /// </summary>
        [Parameter] public ToastPosition Position { get; set; } = ToastPosition.TopRight;

        /// <summary>
        /// Gets or sets the timeout duration in seconds for displaying toast notifications.
        /// </summary>
        [Parameter] public int Timeout { get; set; } = 5;

        /// <summary>
        /// Gets or sets the maximum number of toast notifications that can be displayed at a time.
        /// </summary>
        [Parameter] public int MaxToastCount { get; set; } = int.MaxValue;

        /// <summary>
        /// Gets or sets whether toast notifications should be automatically removed when navigating.
        /// </summary>
        [Parameter] public bool RemoveToastsOnNavigation { get; set; }

        /// <summary>
        /// Gets or sets whether to display a progress bar along with the toast notifications.
        /// </summary>
        [Parameter] public bool ShowProgressBar { get; set; }

        /// <summary>
        /// Gets or sets the content of the close button in the toast notifications.
        /// </summary>
        [Parameter] public RenderFragment? CloseButtonContent { get; set; }

        /// <summary>
        /// Gets or sets whether to display a close button in the toast notifications.
        /// </summary>
        [Parameter] public bool ShowCloseButton { get; set; } = true;

        /// <summary>
        /// Gets or sets whether the timeout for toast notifications is disabled.
        /// </summary>
        [Parameter] public bool DisableTimeout { get; set; }

        /// <summary>
        /// Gets or sets whether to pause the progress bar when hovering over a toast notification.
        /// </summary>
        [Parameter] public bool PauseProgressOnHover { get; set; } = false;

        /// <summary>
        /// Gets or sets the extended timeout duration in seconds for displaying toast notifications.
        /// </summary>
        [Parameter] public int ExtendedTimeout { get; set; }
    }

}