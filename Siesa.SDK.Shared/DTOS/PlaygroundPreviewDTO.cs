using System.Collections.Generic;
namespace Siesa.SDK.Shared.DTOS;

/// <summary>
/// Represents a playground preview data transfer object.
/// </summary>
public class PlaygroundPreviewDTO
{
    public string root { get; set; }
    public Dictionary<string, string> files { get; set; }
}