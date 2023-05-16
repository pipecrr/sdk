using System;

namespace Siesa.SDK.Shared.DTOS
{
    public class DynamicEntityFieldsDTO<U>
    {
        public int Rowid { get; set; }
        public byte[] RowVersion { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdateDate { get; set; } = DateTime.UtcNow;
        public string? Source { get; set; }
        public int RowidUserCreates { get; set; }
        public int RowidUserLastUpdate { get; set; }
        public int? RowidSession { get; set; }
        public U RowidRecord { get; set; }
        public int RowidEntityColumn { get; set; }
        public short RowData { get; set; }
        public string TextData { get; set; }
        public decimal? NumericData { get; set; }
        public DateTime? DateData { get; set; }
        public int? RowidInternalEntityData { get; set; }
        public int? RowidGenericEntityData { get; set; }
    }
}