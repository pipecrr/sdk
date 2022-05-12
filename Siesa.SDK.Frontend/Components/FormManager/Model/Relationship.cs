using System;
using System.Collections.Generic;
using Siesa.SDK.Frontend.Components.FormManager.Model.Fields;

namespace Siesa.SDK.Frontend.Components.FormManager.Model
{
    public enum RelationshipType
    {
        OneToMany,
        ManyToMany
    }

    public class Relationship
    {
        public string Name { get; set; }
        public string ResourceTag { get; set; }
        public RelationshipType Type { get; set; }
        public string RelatedBusiness { get; set; }
        public string RelatedField { get; set; }
    }
}
