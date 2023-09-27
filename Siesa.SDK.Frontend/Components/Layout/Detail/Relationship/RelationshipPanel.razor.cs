using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Frontend.Components.FormManager.Model;
using Siesa.SDK.Frontend.Components;
using Siesa.SDK.Frontend.Components.Visualization;
using Siesa.SDK.Frontend.Components.Layout;


namespace Siesa.SDK.Frontend.Components.Layout.Detail.Relationship
{
    /// <summary>
    /// Represents a panel to manage CRUD operations on related BL's
    /// </summary>
    public partial class RelationshipPanel : ComponentBase
    {
        /// <summary>
        /// Gets or sets the name of the parent attachment.
        /// </summary>
        [Parameter] public string BLNameParentAttatchment { get; set; }

        /// <summary>
        /// Gets or sets the relationship information. <see cref="Relationship"/>
        /// </summary>
        [Parameter] public  Siesa.SDK.Frontend.Components.FormManager.Model.Relationship Relationship { get; set; }

        /// <summary>
        /// Gets or sets the parent dynamic object.
        /// </summary>
        [Parameter] public dynamic Parent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether creation of new items is allowed.
        /// </summary>
        [Parameter] public bool AllowCreate { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether editing of existing items is allowed.
        /// </summary>
        [Parameter] public bool AllowEdit { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether deletion of items is allowed.
        /// </summary>
        [Parameter] public bool AllowDelete { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether viewing details of items is allowed.
        /// </summary>
        [Parameter] public bool AllowDetail { get; set; } = true;

        /// <summary>
        /// Gets or sets the action to execute when clicking the edit button.
        /// </summary>
        [Parameter] public Action<string> OnClickEdit { get; set; } = null;

        /// <summary>
        /// Gets or sets the action to execute when clicking the detail button.
        /// </summary>
        [Parameter] public Action<string> OnClickDetail { get; set; } = null;

        /// <summary>
        /// Gets or sets the action to execute when clicking the delete button.
        /// </summary>
        [Parameter] public Action<string, string> OnClickDelete { get; set; } = null;

        /// <summary>
        /// Gets or sets the action to execute when clicking the new button.
        /// </summary>
        [Parameter] public Action OnClickNew { get; set; } = null;

        /// <summary>
        /// Gets or sets a value indicating whether the header is shown.
        /// </summary>
        [Parameter] public bool ShowHeader { get; set; } = true;

        /// <summary>
        /// Gets or sets the width of the panel.
        /// </summary>
        [Parameter] public string Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the panel.
        /// </summary>
        [Parameter] public string Height { get; set; }

        /// <summary>
        /// Gets or sets the width of the modal if used. <see cref="SDKModalWidth"/>
        /// </summary>
        [Parameter] public SDKModalWidth ModalWidth { get; set; } = SDKModalWidth.Undefined;

    }
}