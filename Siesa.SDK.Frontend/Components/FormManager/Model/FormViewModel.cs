using System;
using System.Collections.Generic;
using Siesa.SDK.Frontend.Components.FormManager.Model.Fields;

namespace Siesa.SDK.Frontend.Components.FormManager.Model
{
    /// <summary>
    /// Represents a view model for a form - viewdef, containing various form-related properties.
    /// </summary>
    public class FormViewModel
    {
        /// <summary>
        /// Gets or sets the list of buttons associated with the form.
        /// </summary>
        public List<Button> Buttons { get; set; } = new List<Button>();
        /// <summary>
        /// Gets or sets the list of panels within the form.
        /// </summary>
        public List<Panel> Panels { get; set; } = new List<Panel>();
        /// <summary>
        /// Gets or sets the list of relationships used specifically for the detail view of the form.
        /// </summary>
        public List<Relationship> Relationships { get; set; } = new List<Relationship>(); //Used for relationships in detailview
        /// <summary>
        /// Gets or sets the list of additional fields to be included in the form.
        /// </summary>
        public List<string> ExtraFields { get; set; } = new List<string>();
        /// <summary>
        /// Gets or sets the list of field options for the detail view of the form.
        /// </summary>
        public List<FieldOptions> DetailFields { get; set; } = new ();
        /// <summary>
        /// Gets or sets the list of additional detail fields for the BLDocument.
        /// </summary>
        public List<string> ExtraDetailFields { get; set; } = new List<string>();
        /// <summary>
        /// Gets or sets of actions to Add a row to the form.
        /// </summary>
        public string ActionAddRow { get; set; }
        /// <summary>
        /// Gets or sets of actions to Delete a row from the form.
        /// </summary>
        public string ActionDeleteRow { get; set; }
    }
}
