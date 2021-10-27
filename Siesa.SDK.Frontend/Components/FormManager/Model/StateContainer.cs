using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siesa.SDK.Frontend.Components.FormManager.Model
{
    public class StateContainer
    {
        private List<List<string>> _filters = new List<List<string>>();

        public event EventHandler StateChanged;

        public List<List<string>> getFilters()
        {
            return _filters;
        }

        public void SetFilters (List<List<string>> paramFilters)
        {
            _filters = paramFilters;
            StateHasChanged();
        }

        public void ResetFilters()
        {
            _filters.Clear();
            StateHasChanged();
        }

        private void StateHasChanged()
        {
            StateChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}