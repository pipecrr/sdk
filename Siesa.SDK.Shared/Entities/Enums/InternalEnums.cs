namespace Siesa.Global.Enums
{
/// <summary>
/// Clase que contiene constantes para representar diferentes acciones relacionadas con el SDK.
/// </summary>
public static class enumSDKActions
{
    /// <summary>
    /// Representa la acción de crear.
    /// </summary>
    public static readonly string Create = "Action.Create";

    /// <summary>
    /// Representa la acción de eliminar.
    /// </summary>
    public static readonly string Delete = "Action.Delete";

    /// <summary>
    /// Representa la acción de editar.
    /// </summary>
    public static readonly string Edit = "Action.Edit";

    /// <summary>
    /// Representa la acción de ver detalles.
    /// </summary>
    public static readonly string Detail = "Action.Detail";

    /// <summary>
    /// Representa la acción de acceso.
    /// </summary>
    public static readonly string Access = "Action.Access";

    /// <summary>
    /// Representa la acción de importar.
    /// </summary>
    public static readonly string Import = "Action.Import";

    /// <summary>
    /// Representa la acción de exportar.
    /// </summary>
    public static readonly string Export = "Action.Export";

    /// <summary>
    /// Representa la acción de acceder a archivos adjuntos.
    /// </summary>
    public static readonly string AccessAttachment = "Action.AccessAttachment";

    /// <summary>
    /// Representa la acción de subir archivos adjuntos.
    /// </summary>
    public static readonly string UploadAttachment = "Action.UploadAttachment";

    /// <summary>
    /// Representa la acción de eliminar archivos adjuntos.
    /// </summary>
    public static readonly string DeleteAttachment = "Action.DeleteAttachment";

    /// <summary>
    /// Representa la acción de descargar archivos adjuntos.
    /// </summary>
    public static readonly string DownloadAttachment = "Action.DownloadAttachment";
}

    /// <summary>
    /// Enumerado que define los tipos de mensajes que se pueden mostrar en la aplicación
    /// </summary>

    public static class enumMessageCategory
    {
        /// <summary>
        /// Mensaje tipo Crud
        /// </summary>
        public static readonly string CRUD = "CRUD";

        /// <summary>
        /// Mensaje tipo Custom
        /// </summary>
        public static readonly string Custom = "Custom";
    }
}