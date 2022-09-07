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

        private async Task<string> GetResourceMessage(string resourceTag, Int64 rowidCulture, object[] formatString)
        {
            string resourceMessage = "";
            string message = "";
            if (rowidCulture != 0)
            {
                resourceMessage = await UtilsManager.GetResource(resourceTag, rowidCulture);
            }
            else
            {
                resourceMessage = await UtilsManager.GetResource(resourceTag);
            }

            if (formatString != null)
            {
                message = String.Format(resourceMessage, formatString);
            }
            else
            {
                message = resourceMessage;
            }
            return message;
        }
        public async Task ShowError(string resourceMessage, object?[] variables = null, int duration = 999999, Int64 culture = 0)
        {
            var message = await GetResourceMessage(resourceMessage, culture, variables);

            base.Notify(new SDKNotificationMessage
            {
                Severity = SDKEnums.GetNotification(SDKNotificationSeverity.Error),
                Detail = message,
                Duration = duration
            });
        }
        public async Task ShowSuccess(string resourceMessage, object?[] variables = null, int duration = 5000, Int64 culture = 0)
        {
            var message = await GetResourceMessage(resourceMessage, culture, variables);

            base.Notify(new SDKNotificationMessage
            {
                Severity = SDKEnums.GetNotification(SDKNotificationSeverity.Success),
                Detail = message,
                Duration = duration
            });
        }

        public async Task ShowInfo(string resourceMessage, object?[] variables = null, int duration = 5000, Int64 culture = 0)
        {
            var message = await GetResourceMessage(resourceMessage, culture, variables);
            base.Notify(new SDKNotificationMessage
            {
                Severity = SDKEnums.GetNotification(SDKNotificationSeverity.Info),
                Detail = message,
                Duration = duration
            });
        }

        public async Task ShowWarning(string resourceMessage, object?[] variables = null, int duration = 5000, Int64 culture = 0)
        {
            var message = await GetResourceMessage(resourceMessage, culture, variables);
            base.Notify(new SDKNotificationMessage
            {
                Severity = SDKEnums.GetNotification(SDKNotificationSeverity.Warning),
                Detail = message,
                Duration = duration
            });
        }

        public async Task Notify(SDKNotificationSeverity type, string resourceMessage, object?[] variables = null, int duration = 5000, Int64 culture = 0)
        {
            var message = await GetResourceMessage(resourceMessage, culture, variables);
            base.Notify(new SDKNotificationMessage
            {
                Severity = SDKEnums.GetNotification(type),
                Detail = message,
                Duration = duration,
            });
        }

    }
}


