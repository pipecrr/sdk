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
	/// tabla sdk recreacion
	/// </summary>

	public class E00884_SDKRecreacion : BaseSDK<int>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }


	}
}