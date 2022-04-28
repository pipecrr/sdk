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
	/// Sesión
	/// </summary>

	public class E00210_Session : BaseSDK<int>
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
		public E00220_User User { get; set; }

	}
}