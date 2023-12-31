using System;
using System.Collections.Generic;
using System.Linq;
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

            if (resourceTag.StartsWith("Exception: ", StringComparison.OrdinalIgnoreCase))
                resourceTag = "Custom.Generic.Message.Error";

            if (rowidCulture != 0)
            {
                resourceMessage = await UtilsManager.GetResource(resourceTag, rowidCulture).ConfigureAwait(true);
            }
            else
            {
                resourceMessage = await UtilsManager.GetResource(resourceTag).ConfigureAwait(true);
            }

            if(resourceMessage == resourceTag)
            {
                resourceMessage = $"Resource not found: {resourceTag}";
            }

            if (formatString != null)
            {
                var formats = new object[formatString.Length];

                for (int i = 0; i < formats.Length; i++)
                {
                    var format = formatString[i].ToString();

                    if (format.Contains(',', StringComparison.Ordinal))
                    {
                        var formatSplits = format.Split(',').Where(item => !string.IsNullOrEmpty(item)).ToArray();

                        if (formatSplits.Length > 0)
                        {
                            var messageFragments = new List<string>();

                            foreach (var item in formatSplits)
                            {
                                var resourceFormat = await UtilsManager.GetResource(item, rowidCulture).ConfigureAwait(true);
                                messageFragments.Add(resourceFormat);
                            }

                            formats[i] = string.Join("-", messageFragments);
                        }
                    }
                    else
                    {
                        var resourceFormat = await UtilsManager.GetResource(format, rowidCulture).ConfigureAwait(true);
                        formats[i] = resourceFormat;
                    }
                }
                return String.Format(resourceMessage, formats);
            }
            else
            {
                return resourceMessage;
            }
        }
        public async Task ShowError(string resourceMessage, object?[] variables = null, int duration = 7000, Int64 culture = 0)
        {
            var message = await GetResourceMessage(resourceMessage, culture, variables);

            base.Notify(new SDKNotificationMessage
            {
                Summary = resourceMessage,
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
                Summary = resourceMessage,
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
                Summary = resourceMessage,
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
                Summary = resourceMessage,
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
                Summary = resourceMessage,
                Severity = SDKEnums.GetNotification(type),
                Detail = message,
                Duration = duration,
            });
        }

    }
}


