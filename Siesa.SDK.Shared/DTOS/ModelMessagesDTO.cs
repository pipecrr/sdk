using System;
using System.Collections.Generic;
using Siesa.Global.Enums;

namespace Siesa.SDK.Shared.DTOS;


/// <summary>
/// Represents model messages for the ErrorsWindow, encapsulating information about errors and messages.
/// </summary>
public class ModelMessagesDTO
{
    /// <summary>
    /// Gets or sets the main message describing the error or informational message.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets the name of the property associated with the error, if applicable.
    /// </summary>
    public string PropertyName { get; set; }

    /// <summary>
    /// Gets or sets the stack trace information related to the error, if available.
    /// </summary>
    public string StackTrace { get; set; }

    /// <summary>
    /// Gets or sets the type of the model message (e.g., Error, Warning, Information).
    /// Defaults to EnumModelMessageType.Error.
    /// </summary>
    public EnumModelMessageType TypeMessange { get; set; } = EnumModelMessageType.Error;

    /// <summary>
    /// Gets or sets a dictionary containing formatted messages, if applicable.
    /// The dictionary keys represent placeholders in the main message, and the values
    /// contain lists of strings to be used as replacements.
    /// </summary>
    public Dictionary<string, List<string>> MessageFormat { get; set; }
}
