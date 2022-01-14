
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siesa.SDK.Entities
{
    public enum PermissionUserTypes
    {
        User = 1,
        Team = 2,
    }

    public enum PermissionAuthTypes
    {
        Query = 1,
        Transaction = 2,
        Query_Tx = 3,
    }

    public enum PermissionRestrictionType
    {
        Not_Applicable = 0,
        Enabled = 1,
        Disabled = 2,
    }
    public abstract class BaseUserPermissionEntity<T>: BaseEntity
    {
        public PermissionUserTypes UserType { get; set; }
        public PermissionAuthTypes AuthorizationType { get; set; }
        public PermissionRestrictionType RestrictionType { get; set; }

        public virtual E00110_Team Team { get; set; }
        public int? RowidRelUser { get; set; }
        [ForeignKey(nameof(RowidRelUser))]
        public virtual E00102_User User { get; set; }

        public int? RowidRecord { get; set; }
        [ForeignKey(nameof(RowidRecord))]
        public virtual T Record { get; set; }
    }

}
