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
	/// Detalle adjuntos
	/// </summary>

	public partial class E00271_AttachmentDetail : BaseAudit<int>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[ForeignKey("Attachment")]
		public int? RowidAttachment { get; set; }

		[Required]
		public string Url { get; set; }

		[Required]
		[StringLength(100)]
		public string FileType { get; set; }

		[StringLength(250)]
		public string? FileName { get; set; }


		[SDKCheckRelationship]
		public virtual E00270_Attachment Attachment { get; set; }

	}
}