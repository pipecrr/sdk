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
	/// Roles
	/// </summary>

	[Index(nameof(Id), nameof(Description), Name = "IX_e00221_1", IsUnique = true)]
	public class E00221_Rol : BaseSDK<int>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[ForeignKey("GroupCompany")]
		[Required]
		public int RowidGroupCompany { get; set; }

		[Required]
		[StringLength(500)]
		public string Id { get; set; }

		[Required]
		[StringLength(500)]
		public string Description { get; set; }


		public E00220_User UserCreate { get; set; }

		public E00220_User UserLastUpdate { get; set; }

		[Required]
		public E00200_GroupCompany GroupCompany { get; set; }

	}
}