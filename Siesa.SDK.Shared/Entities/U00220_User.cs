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
	/// Permisos por registro para la tabla de usuarios
	/// </summary>
	[SDKLogEntity]
	[Index(nameof(RowidUser), nameof(RowidRecord), nameof(RowidDataVisibilityGroup), Name = "IX_u00220_1", IsUnique = true)]
	public partial class U00220_User : BaseUserPermission<E00220_User, int>
	{

	}
}