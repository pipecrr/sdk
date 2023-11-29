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

        public static ProgressBarStyle GetStyleProgressBar(this SDKProgressBarStyle progresBarStyle)
        {

            return progresBarStyle switch
            {
                SDKProgressBarStyle.Primary => ProgressBarStyle.Primary,
                SDKProgressBarStyle.Secondary => ProgressBarStyle.Secondary,
                SDKProgressBarStyle.Success => ProgressBarStyle.Success,
                SDKProgressBarStyle.Danger => ProgressBarStyle.Danger,
                SDKProgressBarStyle.Warning => ProgressBarStyle.Warning,
                SDKProgressBarStyle.Info => ProgressBarStyle.Info,
                SDKProgressBarStyle.Light => ProgressBarStyle.Light,
                SDKProgressBarStyle.Dark => ProgressBarStyle.Dark
            };

        }

        public static ProgressBarMode GetModeProgressBar(this SDKProgressBarType progresBarType)
        {

            return progresBarType switch
            {
                SDKProgressBarType.Determinate => ProgressBarMode.Determinate,
                SDKProgressBarType.Indeterminate => ProgressBarMode.Indeterminate
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

        public static CaptionPosition Get(this SDKCaptionPosition captionPosition)
        {
            return captionPosition switch
            {
                SDKCaptionPosition.Vertical => CaptionPosition.Vertical,
                SDKCaptionPosition.Horizontal => CaptionPosition.Horizontal,
            };
        }

        public static RelativePosition Get(this SDKChartRelativePosition relativePosition)
        {
            return relativePosition switch
            {
                SDKChartRelativePosition.Inside => RelativePosition.Inside,
                SDKChartRelativePosition.Outside => RelativePosition.Outside
            };
        }

        public static HorizontalAlignment Get(this SDKChartHorizontalAlignment horizontalAlignment)
        {
            return horizontalAlignment switch
            {
                SDKChartHorizontalAlignment.Center => HorizontalAlignment.Center,
                SDKChartHorizontalAlignment.Left => HorizontalAlignment.Left,
                SDKChartHorizontalAlignment.Right => HorizontalAlignment.Right
            };
        }

        public static VerticalEdge Get(this SDKChartVerticalEdge verticalAlignment)
        {
            return verticalAlignment switch
            {
                SDKChartVerticalEdge.Top => VerticalEdge.Top,
                SDKChartVerticalEdge.Bottom => VerticalEdge.Bottom
            };
        }

        public static DevExpress.Blazor.Orientation Get(this SDKChartOrientation orientation)
        {
            return orientation switch
            {
                SDKChartOrientation.Horizontal => DevExpress.Blazor.Orientation.Horizontal,
                SDKChartOrientation.Vertical => DevExpress.Blazor.Orientation.Vertical
            };
        }

        public static DevExpress.Blazor.ChartLegendHoverMode Get(this SDKChartLegendHoverMode hoverMode)
        {
            return hoverMode switch
            {
                SDKChartLegendHoverMode.LegendMarkerAndSeriesWithPoints => DevExpress.Blazor.ChartLegendHoverMode.LegendMarkerAndSeriesWithPoints,
                SDKChartLegendHoverMode.LegendMarkerAndSeries => DevExpress.Blazor.ChartLegendHoverMode.LegendMarkerAndSeries,
                SDKChartLegendHoverMode.None => DevExpress.Blazor.ChartLegendHoverMode.None
            };
        }

        public static FlexWrap Get(this SDKFlexWrap flexWrap)
        {
            return flexWrap switch
            {
                SDKFlexWrap.NoWrap => FlexWrap.NoWrap,
                SDKFlexWrap.Wrap => FlexWrap.Wrap,
                SDKFlexWrap.WrapReverse => FlexWrap.WrapReverse
            };
        }

        public static Radzen.Orientation Get(this SDKOrientation orientation)
        {
            return orientation switch
            {
                SDKOrientation.Vertical => Radzen.Orientation.Vertical,
                SDKOrientation.Horizontal => Radzen.Orientation.Horizontal
            };
        }

        public static Radzen.AlignItems Get(this SDKAlignItems alignItems)
        {
            return alignItems switch
            {
                SDKAlignItems.Center => Radzen.AlignItems.Center,
                SDKAlignItems.End => Radzen.AlignItems.End,
                SDKAlignItems.Normal => Radzen.AlignItems.Normal,
                SDKAlignItems.Start => Radzen.AlignItems.Start,
                SDKAlignItems.Stretch => Radzen.AlignItems.Stretch
            };
        }

        public static Radzen.JustifyContent Get(this SDKJustifyContent justifyContent)
        {
            return justifyContent switch
            {
                SDKJustifyContent.Center => Radzen.JustifyContent.Center,
                SDKJustifyContent.End => Radzen.JustifyContent.End,
                SDKJustifyContent.Left => Radzen.JustifyContent.Left,
                SDKJustifyContent.Normal => Radzen.JustifyContent.Normal,
                SDKJustifyContent.Right => Radzen.JustifyContent.Right,
                SDKJustifyContent.SpaceAround => Radzen.JustifyContent.SpaceAround,
                SDKJustifyContent.SpaceBetween => Radzen.JustifyContent.SpaceBetween,
                SDKJustifyContent.SpaceEvenly => Radzen.JustifyContent.SpaceEvenly,
                SDKJustifyContent.Start => Radzen.JustifyContent.Start,
                SDKJustifyContent.Stretch => Radzen.JustifyContent.Stretch
            };
        }
        public static Radzen.HtmlEditorMode Get(this SDKHtmlEditorMode htmlEditorMode)
        {
            return htmlEditorMode switch
            {
                SDKHtmlEditorMode.Design => Radzen.HtmlEditorMode.Design,
                SDKHtmlEditorMode.Source => Radzen.HtmlEditorMode.Source,
                _ => Radzen.HtmlEditorMode.Design
            };
        }

        public static ChartContinuousSeriesHoverMode Get(this SDKChartContinuousSeriesHoverMode hoverMode)
        {
            return hoverMode switch
            {
                SDKChartContinuousSeriesHoverMode.NearestPoint => ChartContinuousSeriesHoverMode.NearestPoint,
                SDKChartContinuousSeriesHoverMode.SeriesAndAllPoints => ChartContinuousSeriesHoverMode.SeriesAndAllPoints,
                SDKChartContinuousSeriesHoverMode.Series => ChartContinuousSeriesHoverMode.Series,
                SDKChartContinuousSeriesHoverMode.None => ChartContinuousSeriesHoverMode.None,
                _ => ChartContinuousSeriesHoverMode.NearestPoint
            };
        }

        public static ChartContinuousSeriesSelectionMode Get(this SDKChartContinuousSeriesSelectionMode selectionMode)
        {
            return selectionMode switch
            {
                SDKChartContinuousSeriesSelectionMode.Series => ChartContinuousSeriesSelectionMode.Series,
                SDKChartContinuousSeriesSelectionMode.SeriesAndAllPoints => ChartContinuousSeriesSelectionMode.SeriesAndAllPoints,
                SDKChartContinuousSeriesSelectionMode.None => ChartContinuousSeriesSelectionMode.None,
                _ => ChartContinuousSeriesSelectionMode.Series
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

    public enum SDKCaptionPosition {
        Vertical = CaptionPosition.Vertical,

        Horizontal = CaptionPosition.Horizontal
    }

    public enum SDKChartRelativePosition {
        Inside = CaptionPosition.Vertical,
        Outside = CaptionPosition.Horizontal
    }

    public enum SDKChartHorizontalAlignment {
        Center = HorizontalAlignment.Center,
        Left = HorizontalAlignment.Left,
        Right = HorizontalAlignment.Right
    }

    public enum SDKChartVerticalEdge {
        Top = VerticalEdge.Top,
        Bottom = VerticalEdge.Bottom
    }

    public enum SDKChartOrientation {
        Horizontal = DevExpress.Blazor.Orientation.Horizontal,
        Vertical = DevExpress.Blazor.Orientation.Vertical
    }

    public enum SDKChartLegendHoverMode 
    {
        LegendMarkerAndSeriesWithPoints = DevExpress.Blazor.ChartLegendHoverMode.LegendMarkerAndSeriesWithPoints,
        LegendMarkerAndSeries = DevExpress.Blazor.ChartLegendHoverMode.LegendMarkerAndSeries,
        None = DevExpress.Blazor.ChartLegendHoverMode.None
    }

    public enum SDKModalWidth {
        Undefined = 0,
        Small = 1,
        Medium = 2,
        Large = 3
    }

    public enum SDKProgressBarStyle 
    {
        Primary = ProgressBarStyle.Primary,
        Secondary = ProgressBarStyle.Secondary,
        Success = ProgressBarStyle.Success,
        Danger = ProgressBarStyle.Danger,
        Warning = ProgressBarStyle.Warning,
        Info = ProgressBarStyle.Info,
        Light = ProgressBarStyle.Light,  

        Dark = ProgressBarStyle.Dark
    }

    public enum SDKProgressBarType
    {
        Determinate = ProgressBarMode.Determinate,
        Indeterminate = ProgressBarMode.Indeterminate
    }
    public enum SDKChartContinuousSeriesHoverMode 
    {
        NearestPoint = 0,
        SeriesAndAllPoints = 1,
        Series = 2,
        None = 3

    }

    public enum SDKChartContinuousSeriesSelectionMode
    {
        Series = 0,
        SeriesAndAllPoints = 1,
        None = 2
    }

    public enum SDKFlexWrap
    {
        NoWrap = FlexWrap.NoWrap,
        Wrap = FlexWrap.Wrap,
        WrapReverse = FlexWrap.WrapReverse
    }

    public enum SDKOrientation
    {
        Vertical = Radzen.Orientation.Vertical,
        Horizontal = Radzen.Orientation.Horizontal
    }

    public enum SDKAlignItems
    {
        Center = Radzen.AlignItems.Center,
        End = Radzen.AlignItems.End,
        Normal = Radzen.AlignItems.Normal,
        Start = Radzen.AlignItems.Start,
        Stretch = Radzen.AlignItems.Stretch
    }

    public enum SDKJustifyContent
    {
        Center = Radzen.JustifyContent.Center,
        End = Radzen.JustifyContent.End,
        Left = Radzen.JustifyContent.Left,
        Normal = Radzen.JustifyContent.Normal,
        Right = Radzen.JustifyContent.Right,
        SpaceAround = Radzen.JustifyContent.SpaceAround,
        SpaceBetween = Radzen.JustifyContent.SpaceBetween,
        SpaceEvenly = Radzen.JustifyContent.SpaceEvenly,
        Start = Radzen.JustifyContent.Start,
        Stretch = Radzen.JustifyContent.Stretch
    }

    public enum SDKHtmlEditorMode
    {
        Design = HtmlEditorMode.Design,
        Source = HtmlEditorMode.Source  
    }
}