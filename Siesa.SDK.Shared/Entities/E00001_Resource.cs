﻿
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siesa.SDK.Entities
{
    public class E00001_Resource: BaseSDK<int>
    {
        public string ID { get; set; }
    }
}
