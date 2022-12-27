﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SDK.Report.Services
{
	/// <summary>
	/// Allows to load data definition from custom storage.
	/// </summary>
	public interface IDataSetsService
	{
		object[] GetDataSetsList();
		object GetDataSet(string id);
	}
}

