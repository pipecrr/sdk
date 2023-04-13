using System;
using System.Collections.Generic;
using Siesa.SDK.Frontend.Components.FormManager.Model.Fields;

namespace Siesa.SDK.Frontend.Components.FormManager.Model
{
    public class OrderBy
    {
        public string Field { get; set; }
        public string Direction { get; set; }
    }

    public class Paging
    {
        public int PageSize { get; set; }
        public List<int> AllowedPageSizes { get; set; }
    }
    public class ListViewModel
    {
        public OrderBy OrderBy { get; set; }
        public Paging Paging { get; set; }
        public bool InfiniteScroll { get; set; } = true;
        public List<List<object>> Filters { get; set; } //TODO: Filtrar desde metadata
        public List<FieldOptions> Fields { get; set; } 

        public string LinkTo { get; set; }

        public List<Button> Buttons { get; set; } = new List<Button>();

        public bool ShowActions { get; set; } = true;
        public bool UseFlex { get; set; } = true;
        public int FlexTake { get; set; } = 100;
        public bool ServerPaginationFlex { get; set; } = true;
        public bool ShowLinkTo { get; set; } = false;
        public List<Button> CustomActions { get; set; } = new List<Button>();

        public bool? AllowEdit { get; set; }
        public bool? AllowDelete { get; set; }
        public bool? AllowDetail { get; set; }
        public bool? AllowCreate { get; set; }

        public List<ExtraField> ExtraFields { get; set; } = new List<ExtraField>(); 
    }
}
