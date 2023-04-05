using Newtonsoft.Json;
using Siesa.SDK.Entities;
using Siesa.SDK.Shared.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Siesa.SDK.Shared.Json;
using Microsoft.EntityFrameworkCore;
using Siesa.Global.Enums;



namespace Siesa.SDK.Entities
{
	/// <summary>
	/// Acceso diario programado
	/// </summary>
	[SDKLogEntity]
	[Index(nameof(RowidUser), nameof(DayWeek), nameof(StartTime), Name = "IX_e00222_1", IsUnique = true)]
	public partial class E00222_UserAccessSchedulingJournal : BaseAudit<int>
	{
		[Key]
		[SDKRequired]
		public override int Rowid { get; set; }

		[ForeignKey("User")]
		[SDKRequired]
		public int RowidUser { get; set; }

		[SDKRequired]
		public byte DayWeek { get; set; }

		[SDKRequired]
		public TimeSpan StartTime { get; set; }

		[SDKRequired]
		public TimeSpan EndTime { get; set; }


		[SDKCheckRelationship]
		[SDKRequired]
		public virtual E00220_User User { get; set; }

	}
}