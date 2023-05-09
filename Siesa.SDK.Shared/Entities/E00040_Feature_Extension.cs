using Newtonsoft.Json;
using Siesa.SDK.Entities;
using Siesa.SDK.Shared.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Siesa.SDK.Shared.Json;
using Microsoft.EntityFrameworkCore;
using Siesa.SDK.Shared.DTOS;

namespace Siesa.SDK.Entities
{
	public partial class E00040_Feature
	{
		[NotMapped]
		[JsonIgnore]
		public FeactureOperationsDTO OperationsInfo {get; set;}
		public virtual ICollection<E00042_Operation> Operations {get; set;}
		[NotMapped]
		public List<E00040_Feature> Childs {get; set;}
		[NotMapped]
		public bool IsChildOfAnyFeature {get; set;}

	}
}