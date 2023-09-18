using System;
using System.Collections.Generic;

namespace Siesa.SDK.Shared.DTOS 
{
     /// <summary>
    /// Represents a row configuration for a portal dashboard, including height, zoom behavior, and dashlets.
    /// </summary>
    public class PortalDashboardRow
    {
        /// <summary>
        /// Gets or sets the height of the dashboard row.
        /// </summary>
        public string Height { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the dashlets in this row should keep up in zoom mode.
        /// </summary>
        public bool KeepInZoom { get; set; }

        /// <summary>
        /// Gets or sets the list of dashlets present in this dashboard row.
        /// </summary>
        public List<PortalDashlet> Dashlets { get; set; }
    }

    /// <summary>
    /// Represents a configuration for a single dashlet within a portal dashboard.
    /// </summary>
    public class PortalDashlet
    {
         /// <summary>
        /// Gets or sets the resource tag associated with the dashlet.
        /// </summary>
        public string ResourceTag { get; set; }

        /// <summary>
        /// Gets or sets the type of component to be displayed in the dashlet.
        /// </summary>
        public Type ComponentType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the dashlet title should be shown. Default value is true.
        /// </summary>
        public bool ShowTitle { get; set; } = true;

        /// <summary>
        /// Gets or sets the CSS class for styling the dashlet.
        /// </summary>
        public string CssClass { get; set; }

        /// <summary>
        /// Gets or sets the number of columns the dashlet occupies in the dashboard grid. Default value is 1.
        /// </summary>
        public int Columns { get; set; } = 1;

        /// <summary>
        /// Gets or sets the number of rows the dashlet occupies in the dashboard grid. Default value is 1.
        /// </summary>
        public int Rows { get; set; } = 1;

        /// <summary>
        /// Gets or sets a value indicating whether the dashlet is expanded.
        /// </summary>
        public bool Expanded { get; set; }
    }

    /// <summary>
    /// Represents the configuration for a portal, including dashboard rows and the main dashlet.
    /// </summary>
    public class SDKPortalConfiguration
    {
        /// <summary>
        /// Gets or sets the list of dashboard rows within the portal configuration.
        /// </summary>
        public List<PortalDashboardRow> DashboardRows { get; set; }
        /// <summary>
        /// Gets or sets the main dashlet configuration for the portal.
        /// </summary>
        public PortalDashlet MainDashlet { get; set; }
        /// <summary>
        /// Gets or sets a value indicating when a dashlet is expanded.
        /// </summary>
        public bool HasExpanded { get; set; }
    }
}