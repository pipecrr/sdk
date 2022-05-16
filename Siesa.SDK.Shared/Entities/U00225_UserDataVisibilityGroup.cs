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
	public class U00225_UserDataVisibilityGroup : BaseUserPermission<E00225_UserDataVisibilityGroup, int>
	{

	}
}