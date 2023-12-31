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
	public partial class E00271_AttachmentDetail
	{
		public override string ToString()
		{
			if(!string.IsNullOrEmpty(FileName))
			{
				return FileName;
			}
			else
			{
				return $"{Rowid}";
			}
		}
	}
}