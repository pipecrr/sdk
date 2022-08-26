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
	public partial class E00021_Culture
	{
		public override string ToString()
		{
			return $"({LanguageCode}{(CountryCode != null ? '_'+CountryCode : CountryCode)}) - {Description}";
		}



	}
}