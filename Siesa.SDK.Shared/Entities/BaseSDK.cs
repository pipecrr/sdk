
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siesa.SDK.Entities
{
    public interface IBaseSDK {
        bool CheckRowid(Int64 rowid);
        Int64 GetRowid();

        void SetRowid(Int64 rowid);

        object GetRowidObject();

        Type GetRowidType();

    }
    public abstract class BaseSDK<T>: IBaseSDK
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual T Rowid { get; set; }

        [Timestamp]
        public virtual byte[] RowVersion { get; set; }

        public uint xmin { get; set; }
        public virtual bool CheckRowid(Int64 rowid)
        {
            return Rowid.Equals(rowid);
        }

        public virtual Int64 GetRowid()
        {
            return Convert.ToInt64(Rowid);
        }

        public object GetRowidObject()
        {
            return Rowid;
        }

        public Type GetRowidType()
        {
            return typeof(T);
        }

        public virtual void SetRowid(Int64 rowid)
        {
            Rowid = (T)Convert.ChangeType(rowid, typeof(T));
        }
    }
}
