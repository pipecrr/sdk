using System.Collections.Generic;

namespace Siesa.SDK.Shared.DTOS;

public class SDKActionInteractionsDTO
{
    //Puede ser una accion que altere a varias
    public SDKActionDTO Action { get; set; }
    //Pueden ser varias que alteren a otras
    public List<SDKActionDTO> Actions { get; set; }
    public bool AnyoneActionCanAlter { get; set; }
    public List<SDKInteractionDetail> ActionsToAlter { get; set; }
}

public class SDKInteractionDetail
{
    public SDKActionDTO Action { get; set; }
    //Con qué estado va a interactuar, cuando prenda o cuando apague
    public bool StatusToInteract { get; set; }
}