using System;
using System.Threading.Tasks;
using Radzen;
using Siesa.SDK.Frontend.Components;
using Siesa.SDK.Frontend.Components.FormManager.Model;

namespace Siesa.SDK.Frontend.Services
{
    public class SDKNotificationService : NotificationService
    {
        private UtilsManager UtilsManager;
        public SDKNotificationService(UtilsManager utilsManager)
        {
            UtilsManager = utilsManager;
        }
        public async Task ShowError(string message, string format="" ,int duration =  999999, Int64 culture = 0)
        {
            string resourceMessage ="";
            if (culture != 0)
            {
                 resourceMessage = await UtilsManager.GetResource(message,culture);
            }
             else {
                resourceMessage = await UtilsManager.GetResource(message);
             }

             string nombre = String.Format(resourceMessage,format);

            base.Notify(new SDKNotificationMessage
            {
                Severity = SDKEnums.GetNotification(SDKNotificationSeverity.Error),
                Detail = nombre,
                Duration = duration
            });
        }
        public async Task ShowSuccess(string message, string titleResourceTag = "Notification.Success", int duration = 5000)
        {
            string text = "";
            base.Notify(new SDKNotificationMessage
            {
                Severity = SDKEnums.GetNotification(SDKNotificationSeverity.Success),
                Summary = text,
                Detail = message,
                Duration = duration
            });
        }

        public async Task ShowInfo(string message, string titleResourceTag = "Notification.Info", int duration = 5000)
        {
            string text = "";
            base.Notify(new SDKNotificationMessage
            {
                Severity = SDKEnums.GetNotification(SDKNotificationSeverity.Info),
                Summary = text,
                Detail = message,
                Duration = duration
            });
        }

        public async Task ShowWarning(string message, string titleResourceTag = "Notification.Warning", int duration = 5000)
        {
            string text = "";
            base.Notify(new SDKNotificationMessage
            {
                Severity = SDKEnums.GetNotification(SDKNotificationSeverity.Warning),
                Summary = text,
                Detail = message,
                Duration = duration
            });
        }

        public async Task Notify(SDKNotificationSeverity type, string message, string titleResourceTag, int duration = 5000)
        {
            string text = "";
            base.Notify(new SDKNotificationMessage
            {
                Severity = SDKEnums.GetNotification(type),
                Summary = text,
                Detail = message,
                Duration = duration,
            });
        }

    }
}


