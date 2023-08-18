using Microsoft.AspNetCore.Components;
using Siesa.SDK.Frontend.Components;
using System.Threading;
using System.Threading.Tasks;

namespace Siesa.SDK.Frontend.Components.Fields
{
    public partial class SDKSearchInput : SDKComponent
    {

        [Parameter]
        public string Value { get; set; }

        [Parameter]
        public string Placeholder { get; set; } = "Search...";

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        [Parameter]
        public int MinMillisecondsBetweenSearch { get; set; } = 250;

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public int MinToFilter { get; set; } = 1;

        [Parameter]
        public string CssClass { get; set; }
        private CancellationTokenSource _cancellationToken {get; set;}

        private async Task HandleInput(ChangeEventArgs e)
        {
            Value = e.Value.ToString().Trim();

            if(_cancellationToken is not null)
            {
                _cancellationToken.Cancel();
            }

            _cancellationToken = new();

            var token = _cancellationToken.Token;

            await Task.Delay(MinMillisecondsBetweenSearch, token).ConfigureAwait(true);
            
            if (token.IsCancellationRequested)
                return;

            if(!string.IsNullOrEmpty(Value) && Value.Length < MinToFilter)
                return;
            
            await ValueChanged.InvokeAsync(Value).ConfigureAwait(true);
        }

    }
}