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
	/// Permisos de usuario por fila
	/// </summary>
	[SDKLogEntity]
	[Index(nameof(RowidUser), nameof(RowidRecord), nameof(RowidDataVisibilityGroup), Name = "IX_u00223_1", IsUnique = true)]
	public partial class U00223_UserAccountPolicy : BaseUserPermission<E00223_UserAccountPolicy, int>
	{

	}
}