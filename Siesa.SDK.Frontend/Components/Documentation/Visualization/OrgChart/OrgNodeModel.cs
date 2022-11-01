using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;

public class OrgNodeModel : NodeModel
{
    public OrgNodeModel(Point position = null) : base(position) { }

    public string Name { get; set; }
    public string Photo { get; set; }
    public string WorkPosition { get; set; }

    // Here, you can put whatever you want, such as a method that does the addition
}