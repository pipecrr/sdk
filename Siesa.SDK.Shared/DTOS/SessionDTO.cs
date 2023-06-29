using Siesa.SDK.Shared.DTOS;
namespace Siesa.SDK.Backend.Access
{
    public class SessionDTO
    {
        public string Token { get; set; }
        public string IdSession { get; set; }
        public string UserPhoto { get; set; }
        public UserPreferencesDTO UserPreferences { get; set; } = new UserPreferencesDTO();
    }
}