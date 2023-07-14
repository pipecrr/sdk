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
	/// tabla interna para saber los posibles portales
	/// </summary>

	public partial class E00515_PortalType : BaseSDK<int>
	{
		[Key]
		[SDKRequired]
		public override int Rowid { get; set; }

		[ForeignKey("Feature")]
		[SDKRequired]
		public int RowidFeature { get; set; } = 2;


		[SDKCheckRelationship]
		[SDKRequired]
		public virtual E00040_Feature Feature { get; set; }

	}
}