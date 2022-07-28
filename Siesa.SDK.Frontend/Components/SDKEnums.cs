using Radzen;

namespace Siesa.SDK.Frontend.Components
{
    public static class SDKEnums {
        public static DataGridSelectionMode Get(this SDKSelectionMode selectionMode){
        return selectionMode switch
        {
            SDKSelectionMode.Single => DataGridSelectionMode.Single,
            SDKSelectionMode.Multiple => DataGridSelectionMode.Multiple
        };
    }

    }
    public enum SDKSelectionMode
    {
        Single = DataGridSelectionMode.Single,
        Multiple = DataGridSelectionMode.Multiple
    }
}