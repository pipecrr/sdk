using Siesa.SDK.Entities;
using System.Collections.Generic;
using Siesa.Global.Enums;

namespace Siesa.SDK.Shared.DTOS
{
    public class UserPreferencesDTO
    {
        public string ThemeColor { get; set; }
        public enumThemeIconStyle IconsStyles { get; set; } = enumThemeIconStyle.Solid; 

        public string ComplementaryColors { get; set; }

        public enumThemeTopbarStyle TopbarStyles { get; set; } 

    }

    
}