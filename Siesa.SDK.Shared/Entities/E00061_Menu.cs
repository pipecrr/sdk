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
	/// Menú
	/// </summary>
	public enum MenuType
    {
        Separator = 1,
        Menu = 2,
        Feature = 3
    }


	[Index(nameof(RowidMenuParent), nameof(RowidResource), nameof(RowidFeature), nameof(Order), Name = "IX_e00061_1", IsUnique = true)]
	public class E00061_Menu : BaseSDK<int>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[ForeignKey("MenuParent")]
		public int? RowidMenuParent { get; set; }

		[ForeignKey("Resource")]
		public int? RowidResource { get; set; }

		[ForeignKey("Feature")]
		public int? RowidFeature { get; set; }

		[Required]
		public byte Order { get; set; }

		[Required]
		public MenuType Type { get; set; }

		[Required]
		public byte Level { get; set; }

		public int? RowidImage { get; set; }

		[StringLength(250)]
		public string? Url { get; set; }

		[StringLength(250)]
		public string? IconClass { get; set; }


		public E00020_Resource Resource { get; set; }

		public E00040_Feature Feature { get; set; }

		public E00061_Menu MenuParent { get; set; }

		public virtual ICollection<E00061_Menu> SubMenus { get; set; }

	}
}