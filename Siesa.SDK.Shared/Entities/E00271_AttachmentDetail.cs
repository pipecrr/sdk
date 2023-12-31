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
		[SDKRequired]
		public override int Rowid { get; set; }

		[ForeignKey("Attachment")]
		public int? RowidAttachment { get; set; }

		[SDKRequired]
		public string Url { get; set; }

		[SDKRequired]
		[SDKStringLength(100)]
		public string FileType { get; set; }

		[SDKStringLength(250)]
		public string? FileName { get; set; }

		public byte[]? FileInternalAttached { get; set; }


		[SDKCheckRelationship]
		public virtual E00270_Attachment Attachment { get; set; }

	}
}