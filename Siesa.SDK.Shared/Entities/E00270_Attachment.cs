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
	/// Adjuntos
	/// </summary>

	public partial class E00270_Attachment : BaseSDK<int>
	{
		[Key]
		[SDKRequired]
		public override int Rowid { get; set; }


	}
}