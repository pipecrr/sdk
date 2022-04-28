using Newtonsoft.Json;
using Siesa.SDK.Entities;
using Siesa.SDK.Shared.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Siesa.SDK.Shared.Json;
using Microsoft.EntityFrameworkCore;


namespace Siesa.SDK.Entities
{
	/// <summary>
	/// Acceso diario programado
	/// </summary>

	[Index(nameof(RowidUser), nameof(DayWeek), nameof(StartTime), Name = "IX_e00222_1", IsUnique = true)]
	public class E00222_UserAccessSchedulingJournal : BaseAudit<int>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[ForeignKey("User")]
		[Required]
		public int RowidUser { get; set; }

		[Required]
		public byte DayWeek { get; set; }

		[Required]
		public TimeSpan StartTime { get; set; }

		[Required]
		public TimeSpan EndTime { get; set; }


		[Required]
		public E00220_User User { get; set; }

	}
}