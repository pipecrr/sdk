using Microsoft.AspNetCore.Components;
using Siesa.SDK.Frontend.Components;
using System.Threading;
using System.Threading.Tasks;

namespace Siesa.SDK.Frontend.Components.Fields
{
    /// <summary>
    /// Represents  an component with input field for searching 
    /// </summary>
    public partial class SDKSearchInput : SDKComponent
    {

        /// <summary>
        /// Gets or sets the current value of the search input.
        /// </summary>

        [Parameter]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the placeholder text displayed inside the search input when it's empty.
        /// </summary>
        [Parameter]
        public string Placeholder { get; set; } = "Search...";

        /// <summary>
        /// Gets or sets the callback that is invoked when the value of the search input changes.
        /// </summary>
        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        /// <summary>
        /// Gets or sets the minimum time in milliseconds between consecutive search requests.
        /// </summary>
        [Parameter]
        public int MinMillisecondsBetweenSearch { get; set; } = 250;

        /// <summary>
        /// Gets or sets a value indicating whether the search input is disabled.
        /// </summary>
        [Parameter]
        public bool Disabled { get; set; }

        /// <summary>
        /// Gets or sets the minimum number of characters required to trigger a search.
        /// </summary>
        [Parameter]
        public int MinToFilter { get; set; } = 1;

        /// <summary>
        /// Gets or sets the CSS class to apply to the search input.
        /// </summary>
        [Parameter]
        public string CssClass { get; set; }

        private CancellationTokenSource _cancellationToken { get; set; }

        private async Task HandleInput(ChangeEventArgs e)
        {
            Value = e.Value.ToString().Trim();

            if (_cancellationToken is not null)
            {
                _cancellationToken.Cancel();
            }

            _cancellationToken = new();

            var token = _cancellationToken.Token;

            await Task.Delay(MinMillisecondsBetweenSearch, token).ConfigureAwait(true);

            if (token.IsCancellationRequested)
                return;

            if (!string.IsNullOrEmpty(Value) && Value.Length < MinToFilter)
                return;

            await ValueChanged.InvokeAsync(Value).ConfigureAwait(true);
        }

    }
}