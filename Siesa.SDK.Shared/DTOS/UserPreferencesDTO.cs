using Siesa.SDK.Entities;
using System.Collections.Generic;
using Siesa.Global.Enums;

namespace Siesa.SDK.Shared.DTOS
{
    public class UserPreferencesDTO
    {
        public string ThemeColor { get; set; } //Key = ThemeName, Value = ThemeColor

        public enumThemeIconStyle IconsStyles { get; set; } //Enum of Icons Styles enumSDKIconStyle

        public string ComplementaryColors { get; set; }

        public enumThemeTopbarStyle TopbarStyles { get; set; } //Enum of Topbar Styles enumSDKTopbarStyle

    }

    
}