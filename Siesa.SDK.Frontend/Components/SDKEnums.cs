using DevExpress.Blazor;
using Radzen;

namespace Siesa.SDK.Frontend.Components
{
    public static class SDKEnums
    {
        public static Radzen.DataGridSelectionMode Get(this SDKSelectionMode selectionMode)
        {
            return selectionMode switch
            {
                SDKSelectionMode.Single => Radzen.DataGridSelectionMode.Single,
                SDKSelectionMode.Multiple => Radzen.DataGridSelectionMode.Multiple
            };
        }

        public static NotificationSeverity GetNotification(this SDKNotificationSeverity notificationSeverity)
        {
            return notificationSeverity switch
            {
                SDKNotificationSeverity.Error => NotificationSeverity.Error,
                SDKNotificationSeverity.Info => NotificationSeverity.Info,
                SDKNotificationSeverity.Success => NotificationSeverity.Success,
                SDKNotificationSeverity.Warning => NotificationSeverity.Warning

            };
        }

        public static BadgeStyle GetStyleBadge(this SDKBadgeStyle badgeStyle)
        {

            return badgeStyle switch
            {
                SDKBadgeStyle.Primary => BadgeStyle.Primary,
                SDKBadgeStyle.Secondary => BadgeStyle.Secondary,
                SDKBadgeStyle.Success => BadgeStyle.Success,
                SDKBadgeStyle.Danger => BadgeStyle.Danger,
                SDKBadgeStyle.Warning => BadgeStyle.Warning,
                SDKBadgeStyle.Info => BadgeStyle.Info,
                SDKBadgeStyle.Light => BadgeStyle.Light
            };

        }

        public static DevExpress.Blazor.Orientation GetOrientationMenu(this SDKOrientationMenu orientationMenu)
        {
            return orientationMenu switch
            {
                SDKOrientationMenu.Horizontal => DevExpress.Blazor.Orientation.Horizontal,
                SDKOrientationMenu.Vertical => DevExpress.Blazor.Orientation.Vertical

            };
        }

        public static DataEditorClearButtonDisplayMode Get(this SDKClearButtonDisplayMode clearButtonDisplayMode)
        {
            return clearButtonDisplayMode switch
            {
                SDKClearButtonDisplayMode.Auto => DataEditorClearButtonDisplayMode.Auto,
                SDKClearButtonDisplayMode.Never => DataEditorClearButtonDisplayMode.Never,
            };
        }

    }

    public enum SDKClearButtonDisplayMode
    {
        Auto = DataEditorClearButtonDisplayMode.Auto,
        Never = DataEditorClearButtonDisplayMode.Never,
    }
    public enum SDKSelectionMode
    {
        Single = Radzen.DataGridSelectionMode.Single,
        Multiple = Radzen.DataGridSelectionMode.Multiple
    }

    public enum SDKColumnAlign {
        Left,
        Right,
        Center
    }

    public enum SDKNotificationSeverity
    {
        Error = NotificationSeverity.Error,
        Info = NotificationSeverity.Info,

        Success = NotificationSeverity.Success,
        Warning = NotificationSeverity.Warning

    }

    public enum SDKBadgeStyle{
        Primary = BadgeStyle.Primary,
        Secondary = BadgeStyle.Secondary,
        Success = BadgeStyle.Success,
        Danger = BadgeStyle.Danger,
        Warning = BadgeStyle.Warning,
        Info = BadgeStyle.Info,
        Light = BadgeStyle.Light,
    }

    public enum SDKTypeFile{
        Image = 1,
        Video = 2,
        Audio = 3,
        Pdf = 4,
        Text = 5,
        Other = 6
    }

    public enum SDKOrientationMenu{
        Horizontal = 1,
        Vertical = 2

    }
}