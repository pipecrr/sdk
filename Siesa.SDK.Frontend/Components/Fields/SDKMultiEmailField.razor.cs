using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Siesa.SDK.Frontend.Components.Fields;

/// <summary>
/// Class representing a multiple email field in the SDK.
/// </summary>
public partial class SDKMultiEmailField : SDKComponent
{
    /// <summary>
    /// Gets or sets the value of the email field.
    /// </summary>
    [Parameter]
    public string Value { get; set; } = "";

    /// <summary>
    /// Gets or sets the placeholder text for the field.
    /// </summary>
    [Parameter]
    public string Placeholder { get; set; } = "Enter the email address";

    /// <summary>
    /// Gets or sets the action to be executed when the field's value changes.
    /// </summary>
    [Parameter] public Action<string> ValueChanged { get; set; } = (value) => {};

    /// <summary>
    /// Gets or sets the expression representing the field's value.
    /// </summary>
    [Parameter]
    public Expression<Func<string>> ValueExpression { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the field is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the field is read-only.
    /// </summary>
    [Parameter]
    public bool ReadOnly { get; set; }

    /// <summary>
    /// Gets or sets the CSS class applied to the field.
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    private List<string> EmailList { get; set; } = new List<string>();

    private void AddEmail()
    {
        if (!string.IsNullOrEmpty(Value) && !EmailList.Contains(Value))
        {
            if (Value.Contains(' ', StringComparison.Ordinal) 
                || Value.Contains(',',StringComparison.Ordinal) 
                || Value.Contains(';', StringComparison.Ordinal))
            {
                var emails = Value.Split(new char[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var email in emails)
                {
                    if (!EmailList.Contains(email))
                    {
                        EmailList.Add(email);
                    }
                }
                Value = string.Empty;
                StateHasChanged();
                return;
            }
            EmailList.Add(Value);
            Value = string.Empty;
            StateHasChanged();
        }
    }

    private void RemoveEmail(string email)
    {
        EmailList.Remove(email);
        StateHasChanged();
    }

    private void OnKeyDown(KeyboardEventArgs e)
    {
        switch (e.Code)
        {
            case "Enter":
                AddEmail();
                break;
            case "Backspace":
                if (string.IsNullOrEmpty(Value) && EmailList.Count > 0)
                {
                    var last = EmailList[EmailList.Count - 1];
                    EmailList.Remove(last);
                }
                break;
            default:
                break;
        }
    }
    private void OnInput(ChangeEventArgs e)
    {
        Value = e.Value.ToString();
    }

    private async Task OnChange(ChangeEventArgs args )
    {
        Value = args?.Value?.ToString();

        if(ValueChanged != null)
            ValueChanged.Invoke(Value);
            
       _ = InvokeAsync(() => StateHasChanged());
    }
}