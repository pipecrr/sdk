using Siesa.Global.Enums;

namespace Siesa.SDK.Shared.DTOS
{
    public class SDKFlexVirtualColumnDTO
    {
        public string ColumnName { get; set; }
        public enumDynamicEntityDataType ColumnType { get; set; }
        public object ColumnValue { get; set; }
        public object DefaultValue { get; set; }
        public object RowidRecord { get; set; }
    }
}