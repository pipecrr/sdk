namespace Siesa.Global.Enums;

    /// <summary>
    /// Class containing constants to represent different actions related to the SDK.
    /// </summary>
public static class enumSDKActions
{
    /// <summary>
    /// Represents the create action.
    /// </summary>
    public static readonly string Create = "Action.Create";

    /// <summary>
    /// Represents the delete action.
    /// </summary>
    public static readonly string Delete = "Action.Delete";

    /// <summary>
    /// Represents the edit action.
    /// </summary>
    public static readonly string Edit = "Action.Edit";

    /// <summary>
    /// Represents the detail view action.
    /// </summary>
    public static readonly string Detail = "Action.Detail";

    /// <summary>
    /// Represents the access action.
    /// </summary>
    public static readonly string Access = "Action.Access";

    /// <summary>
    /// Represents the import action.
    /// </summary>
    public static readonly string Import = "Action.Import";

    /// <summary>
    /// Represents the export action.
    /// </summary>
    public static readonly string Export = "Action.Export";

    /// <summary>
    /// Represents the action to access attachments.
    /// </summary>
    public static readonly string AccessAttachment = "Action.AccessAttachment";

    /// <summary>
    /// Represents the action to upload attachments.
    /// </summary>
    public static readonly string UploadAttachment = "Action.UploadAttachment";

    /// <summary>
    /// Represents the action to delete attachments.
    /// </summary>
    public static readonly string DeleteAttachment = "Action.DeleteAttachment";

    /// <summary>
    /// Represents the action to download attachments.
    /// </summary>
    public static readonly string DownloadAttachment = "Action.DownloadAttachment";
}


/// <summary>
/// Enum type that defines the types of messages that can be displayed in the application.
/// </summary>
public static class enumMessageCategory
{
    /// <summary>
    /// Message of CRUD type.
    /// </summary>
    public static readonly string CRUD = "CRUD";

    /// <summary>
    /// Custom message type.
    /// </summary>
    public static readonly string Custom = "Custom";
}

public enum EnumModelMessageType
{
    Error = 1,
    Info = 2,
    Warning = 3,
}
