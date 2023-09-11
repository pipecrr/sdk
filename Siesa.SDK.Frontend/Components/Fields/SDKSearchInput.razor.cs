using System;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Frontend.Components;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;

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
        /// Gets or sets the callback that is invoked when the search input is focused.
        /// </summary>
        [Parameter]
        public EventCallback OnFocus { get; set; }
        
        /// <summary>
        /// Gets or sets the callback that is invoked when the search input is focusout.
        /// </summary>
        [Parameter]
        public EventCallback OnFocusOut { get; set; }
        
        /// <summary>
        /// Gets or sets the callback that is invoked when the search input is clicked.
        /// </summary>
        [Parameter]
        public EventCallback OnClick { get; set; }

        ///<summary>
        /// Gets or sets the callback that is invoked when the search input is keydown.
        /// </summary>
        /// <param name="e">The <see cref="KeyboardEventArgs"/> instance containing the event data.</param>
        [Parameter]
        public EventCallback<KeyboardEventArgs> OnEnter { get; set; }
        
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
        private bool _loading { get; set; }
        private bool _queued { get; set; }

        private async Task HandleInput(ChangeEventArgs e)
        {
            Value = e.Value.ToString();

            if (_cancellationToken is not null)
            {
                try
                {
                    _cancellationToken.Cancel();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    if (_cancellationToken != null)
                    {
                        Console.WriteLine($"ARME: {Value} TOKEN: {_cancellationToken.Token.IsCancellationRequested.ToString()}");
                        _cancellationToken.Dispose();
                        _cancellationToken = null;
                    }
                }
            }

            _cancellationToken = new();

            var token = _cancellationToken.Token;
            Console.WriteLine($"ARME0: {Value} TOKEN: {token.IsCancellationRequested.ToString()}");
            try{
                await Task.Delay(MinMillisecondsBetweenSearch, token).ContinueWith(t => {}).ConfigureAwait(true);
            }catch (TaskCanceledException ex){
                Console.WriteLine($"ARME1: {Value} TOKEN: {ex.Message}");                
                return;
            }

            Console.WriteLine($"ARME2: {Value} TOKEN: {token.IsCancellationRequested.ToString()}");
            if (token.IsCancellationRequested){
                Console.WriteLine($"ARME3: {Value} TOKEN: {token.IsCancellationRequested.ToString()}");
                return;
            }

            if (!string.IsNullOrEmpty(Value) && Value.Length < MinToFilter){
                Console.WriteLine($"ARME4: {Value} TOKEN: {token.IsCancellationRequested.ToString()}");
                return;
            }
            Console.WriteLine($"ARME5: {Value} TOKEN: {token.IsCancellationRequested.ToString()}");
            await ValueChanged.InvokeAsync(Value).ConfigureAwait(true);
            
        }
        
        private async Task HandleFocus()
        {
            await OnFocus.InvokeAsync().ConfigureAwait(true);
        }
        
        private async Task HandleFocusOut()
        {
            await OnFocusOut.InvokeAsync().ConfigureAwait(true);
        }
        
        private async Task HandleClick()
        {
            await OnClick.InvokeAsync().ConfigureAwait(true);
        }

        private async Task HandleKeyDown(KeyboardEventArgs e)
        { 
            if(e.Key == "Delete" || e.Key == "Backspace"){
                if (_cancellationToken is not null)
                    _cancellationToken.Cancel();
            }                
            if (e.Key != "Enter")
                return;
            await OnEnter.InvokeAsync(e).ConfigureAwait(true);
        }

    }
}