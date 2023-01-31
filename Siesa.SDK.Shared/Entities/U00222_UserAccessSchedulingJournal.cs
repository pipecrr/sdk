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
	/// Permisos de usuario por fila
	/// </summary>
	[SDKLogEntity]
	[Index(nameof(RowidUser), nameof(RowidRecord), nameof(RowidDataVisibilityGroup), Name = "IX_u00222_1", IsUnique = true)]
	public partial class U00222_UserAccessSchedulingJournal : BaseUserPermission<E00222_UserAccessSchedulingJournal, int>
	{

	}
}