using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;

namespace Siesa.SDK.Frontend.Components.Documentation.Playground;

public class Entry
{
    public string Name { get; set; }
    public bool IsDirectory { get; set; }
    public List<Entry> Children { get; set; }
    public string Code { get; set; }
}

public partial class FileTreeItem: ComponentBase
{
    [Parameter]
    public Entry Entry { get; set; }

    private string GetFileIcon()
    {
        bool isDirectory = Entry.IsDirectory;
        var fileIcon = isDirectory ? "fa-folder" : "fa-file";
        if (!isDirectory)
        {
            var extension = Entry.Name.Split('.').LastOrDefault();
            switch (extension.ToLower())
            {
                case "cs":
                    fileIcon = "fa-code";
                    break;
                case "razor":
                    fileIcon = "fa-code";
                    break;
                case "css":
                    fileIcon = "fa-css3-alt";
                    break;
            }
        }

        return fileIcon;
    }
}