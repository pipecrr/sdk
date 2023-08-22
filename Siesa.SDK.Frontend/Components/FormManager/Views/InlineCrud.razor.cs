
using System;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Frontend.Components.Visualization;
using Siesa.SDK.Components.Visualization;
using Siesa.SDK.Frontend.Services;
using Siesa.Global.Enums;
using System.Reflection;
using Siesa.SDK.Frontend.Components;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;


namespace Siesa.SDK.Frontend.Components.FormManager.Views
{
    /// <summary>
    /// Represents a component for CRUD operations.
    /// </summary>
    public partial class InlineCrud : ComponentBase
    {
        /// <summary>
        /// Gets or sets the BL name parent attachment.
        /// </summary>
        [Parameter]
        public string BLNameParentAttatchment { get; set; }

        /// <summary>
        /// Gets or sets the business name.
        /// </summary>
        [Parameter]
        public string BusinessName { get; set; }

        /// <summary>
        /// Gets or sets the default fields for creating new items.
        /// </summary>
        [Parameter]
        public Dictionary<string, object> DefaultFieldsCreate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the search form.
        /// </summary>
        [Parameter]
        public bool ShowSearchForm { get; set; } = true;

        /// <summary>
        /// Gets or sets the filter expression.
        /// </summary>
        [Parameter]
        public string Filter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow item creation.
        /// </summary>
        [Parameter]
        public bool AllowCreate { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to allow item editing.
        /// </summary>
        [Parameter]
        public bool AllowEdit { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to allow item deletion.
        /// </summary>
        [Parameter]
        public bool AllowDelete { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to allow item details view.
        /// </summary>
        [Parameter]
        public bool AllowDetail { get; set; } = true;

        /// <summary>
        /// Gets or sets the action to execute when one or more rows are selected.
        /// </summary>
        [Parameter]
        public Action<IList<dynamic>> OnSelectedRow { get; set; } = null;

        /// <summary>
        /// Gets or sets the data source for the items.
        /// </summary>
        [Parameter]
        public IEnumerable<object> Data { get; set; } = null;

        /// <summary>
        /// Gets or sets the action to execute when creating a new item.
        /// </summary>
        [Parameter]
        public Action<dynamic> OnCreate { get; set; }

        /// <summary>
        /// Gets or sets the action to execute when editing an item.
        /// </summary>
        [Parameter]
        public Action<dynamic> OnEdit { get; set; }

        /// <summary>
        /// Gets or sets the action to execute when deleting an item.
        /// </summary>
        [Parameter]
        public Action OnDelete { get; set; }

        /// <summary>
        /// Gets or sets the modal width. <see cref="SDKModalWidth"/>
        /// </summary>
        [Parameter]
        public SDKModalWidth ModalWidth { get; set; } = SDKModalWidth.Undefined;

        /// <summary>
        /// Gets or sets the width of the modal.
        /// </summary>
        [Parameter]
        public string Width { get; set; } = "600px";

        /// <summary>
        /// Gets or sets the height of the modal.
        /// </summary>
        [Parameter]
        public string Height { get; set; }
    }
}