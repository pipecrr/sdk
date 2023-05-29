using Siesa.SDK.Entities;
using System.Collections.Generic;

namespace Siesa.SDK.Frontend.Components.Documentation.DTOs
{
    public class UserPreferencesDTO
    {
        public string NameUser { get; set; }
        public string PasswordUser { get; set; }
        public string EmailUser { get; set; }
        public int RowidCultureUser { get; set; }
        public string ThemeColor { get; set; }
        public string ThemeName { get; set; }
    }
}