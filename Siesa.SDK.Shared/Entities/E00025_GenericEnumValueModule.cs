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
	/// Módulo  enum valor genérico
	/// </summary>

	[Index(nameof(RowidGenericEnumValue), nameof(RowidModule), Name = "IX_e00025_1", IsUnique = true)]
	public class E00025_GenericEnumValueModule : BaseSDK<int>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[Required]
		public int RowidGenericEnumValue { get; set; }

		[ForeignKey("Module")]
		[Required]
		public short RowidModule { get; set; }


		[Required]
		public E00010_Module Module { get; set; }

	}
}