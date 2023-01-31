
using System.Collections.Generic;
using Siesa.SDK.Entities;
using Siesa.SDK.Entities.Enums;

namespace Siesa.SDK.Shared.DTOS
{
    public class FeactureOperationsDTO
    {
        public int RowidFeature {get; set;}
        public int RowidRole {get; set;}
        public ICollection<E00042_Operation> Operations {get; set;}
        public List<ActionDescription> Actions {get; set;}
    }

    public class ActionDescription
    {
        public int RowidAction {get; set;}
        public int RowidResource {get; set;}
        public string ResourceTag {get; set;}
        public bool Status {get; set;}
        public enumGeneralAction GeneralAction {get; set;} = enumGeneralAction.None;
    }
}