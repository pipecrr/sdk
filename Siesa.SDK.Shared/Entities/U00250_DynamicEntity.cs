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
	/// Permisos por usuario
	/// </summary>

	[Index(nameof(RowidUser), nameof(RowidRecord), nameof(RowidDataVisibilityGroup), Name = "IX_u00250_1", IsUnique = true)]
	public partial class U00250_DynamicEntity : BaseUserPermission<E00250_DynamicEntity, int>
	{

	}
}