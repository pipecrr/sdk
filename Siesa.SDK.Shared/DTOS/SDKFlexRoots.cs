using System;
using System.Collections.Generic;

namespace Siesa.SDK.Shared.DTOS
{
    public class SDKFlexRootClassInfo
    {
        public SDKFlexRelationships relationships { get; set; }
        public List<SDKFlexField> fields { get; set; }
    }

    public class SDKFlexRootPreviewData
    {
        public string id { get; set; }
        public int company { get; set; }
        public DateTime creation_date { get; set; }
    }

    public class SDKFlexRootMock
    {
        public List<SDKFlexModulesList> modules_list { get; set; }
        public SDKFlexRequestSaveData report_header_data { get; set; }
    }


}