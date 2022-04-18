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
	/// Compañias
	/// </summary>

	[Index(nameof(RowidGroupCompany), Name = "IX_e00201_1")]
	public class E00201_Company
	{
		[Key]
		[Required]
		public int Rowid { get; set; }

		[ForeignKey("GroupCompany")]
		[Required]
		public int RowidGroupCompany { get; set; }


		[Required]
		public E00200_GroupCompany GroupCompany { get; set; }

	}
}