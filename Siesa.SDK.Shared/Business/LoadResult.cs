using Siesa.SDK.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siesa.SDK.Shared.Business
{
    public class LoadResult
    {
        public IEnumerable<dynamic> Data { get; set; }

        public int TotalCount { get; set; }
        public int GroupCount { get; set; }
        public object[] Summary { get; set; }
        public List<string> Errors { get; set; }
    }
}
