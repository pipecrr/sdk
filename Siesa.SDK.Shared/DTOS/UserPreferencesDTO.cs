using Siesa.SDK.Entities;
using System.Collections.Generic;

namespace Siesa.SDK.Shared.DTOS
{
    public class UserPreferencesDTO
    {
        public Dictionary<string, string> ThemeColor { get; set; } //Key = ThemeName, Value = ThemeColor

        public Dictionary<string, string> IconStyle { get; set; } //Key = IconName, Value = IconType
        public List<ComplementaryColor> ComplementaryColors { get; set; }

    }

    public class ComplementaryColor
    {
        public string ColorName { get; set; }
        public string PrimaryColor { get; set; }
        public string SecundaryColor { get; set; }
    }
}