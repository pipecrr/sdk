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
	/// Enums en BD
	/// </summary>

	[Index(nameof(EnumCode), nameof(ID), nameof(Code), Name = "IX_e00024_1", IsUnique = true)]
	public class E00024_GenericEnumCode
	{
		[Required]
		public  int Rowid { get; set; }

		[Required]
		[StringLength(100)]
		public  string EnumCode { get; set; }

		[Required]
		[StringLength(100)]
		public  string ID { get; set; }

		[Required]
		public  byte Code { get; set; }


	}
}