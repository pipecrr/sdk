using Newtonsoft.Json;
using Siesa.SDK.Entities;
using Siesa.SDK.Shared.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Siesa.SDK.Shared.Json;
using Microsoft.EntityFrameworkCore;
using Siesa.SDK.Entities.Enums;


namespace Siesa.SDK.Entities
{
	/// <summary>
	/// Sesión
	/// </summary>

	[Index(nameof(Id), Name = "IX_e00210_1", IsUnique = true)]
	public partial class E00210_Session : BaseSDK<int>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[Required]
		public DateTime StartDate { get; set; }

		[Required]
		[StringLength(200)]
		public string Service { get; set; }

		[ForeignKey("User")]
		[Required]
		public int RowidUser { get; set; }

		[StringLength(50)]
		public string? IpAddress { get; set; }

		[StringLength(200)]
		public string? MachineName { get; set; }

		[Required]
		[StringLength(200)]
		public string UserDataBase { get; set; }

		[StringLength(2000)]
		public string? AditionalInformation { get; set; }

		[Required]
		[StringLength(64)]
		public string Id { get; set; }

		[Required]
		public string Token { get; set; }


		[Required]
		public virtual E00220_User User { get; set; }

	}
}