using Siesa.SDK.Entities;
using System.Collections.Generic;
using Siesa.Global.Enums;

namespace Siesa.SDK.Shared.DTOS
{
    public class UserPreferencesDTO
    {
        public Dictionary<string, string> ThemeColor { get; set; } //Key = ThemeName, Value = ThemeColor

        public List<enumThemeIconStyle> IconsStyles { get; set; } //Enum of Icons Styles enumSDKIconStyle

        public List<ComplementaryColor> ComplementaryColors { get; set; }

        //public List<enumThemeTopbarStyle> TopbarStyles { get; set; } //Enum of Topbar Styles enumSDKTopbarStyle

    }

    public class ComplementaryColor
    {
        public string ColorName { get; set; }
        public string PrimaryColor { get; set; }
        public string SecundaryColor { get; set; }
    }
}