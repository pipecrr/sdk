using Radzen;

namespace Siesa.SDK.Frontend.Components
{
    public static class SDKEnums
    {
        public static DataGridSelectionMode Get(this SDKSelectionMode selectionMode)
        {
            return selectionMode switch
            {
                SDKSelectionMode.Single => DataGridSelectionMode.Single,
                SDKSelectionMode.Multiple => DataGridSelectionMode.Multiple
            };
        }

        public static NotificationSeverity GetNotification(this SDKNotificationSeverity notificationSeverity )
        {
            return notificationSeverity switch
            {
                SDKNotificationSeverity.Error => NotificationSeverity.Error,
                SDKNotificationSeverity.Info => NotificationSeverity.Info,
                SDKNotificationSeverity.Success => NotificationSeverity.Success,
                SDKNotificationSeverity.Warning => NotificationSeverity.Warning

            };
        }

    }
    public enum SDKSelectionMode
    {
        Single = DataGridSelectionMode.Single,
        Multiple = DataGridSelectionMode.Multiple
    }

    public enum SDKNotificationSeverity
    {
        Error = NotificationSeverity.Error,
        Info = NotificationSeverity.Info,

        Success = NotificationSeverity.Success,
        Warning = NotificationSeverity.Warning

    }
}