using Radzen;

namespace Siesa.SDK.Frontend.Services
{
    public class SDKNotificationService : NotificationService
    {
        public void ShowError(string title, string message, int duration = 2000)
        {
            this.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = title,
                Detail = message,
                Duration = duration});
            }
        }
    }
