using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Siesa.SDK.Frontend.Components.FormManager.Model
{
    public class MessageValidatorBase<TValue> : ComponentBase, IDisposable
    {
        private FieldIdentifier _fieldIdentifier;
        private EventHandler<ValidationStateChangedEventArgs> _stateChangedHandler
            => (sender, args) => StateHasChanged();

        [CascadingParameter]
        private EditContext EditContext { get; set; }
        [Parameter]
        public Expression<Func<TValue>> For { get; set; }
        [Parameter]
        public string Class { get; set; } = "validation-message";

        protected IEnumerable<string> ValidationMessages { get {
                if(EditContext == null)
                {
                    return new List<string>();
                }
                return EditContext.GetValidationMessages(_fieldIdentifier).Distinct();
            } }
        protected override void OnInitialized()
        {
            _fieldIdentifier = FieldIdentifier.Create(For);
            if(EditContext != null)
            {
                EditContext.OnValidationStateChanged += _stateChangedHandler;
            }
        }

        public void Dispose()
        {
            if(EditContext != null)
            {
                EditContext.OnValidationStateChanged -= _stateChangedHandler;
            }
        }

    }
}