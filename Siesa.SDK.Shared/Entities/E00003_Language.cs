
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siesa.SDK.Entities
{
    public class E00003_Language: BaseEntity
    {
        public string LanguageCode { get; set; }
        public string CountryCode { get; set; }
        public string Descrption { get; set; }
    }
}
