using System.Collections.Generic;

namespace Siesa.SDK.Shared.DTOS;

//Una vez se crea, no cambia
public record SDKActionInteractionsDTO
{
    //Puede ser una accion que altere a varias
    public SDKActionDTO Action { get; set; }
    //Pueden ser varias que alteren a otras
    public List<SDKActionDTO> Actions { get; set; }
    public bool AnyActionCanAlter { get; set; }
    public List<SDKInteractionDetail> ActionsToAlter { get; set; }
}

//Una vez se crea, no cambia
public record SDKInteractionDetail
{
    public SDKActionDTO Action { get; set; }
    //Con qué estado va a interactuar, cuando prenda o cuando apague
    public bool StatusToInteract { get; set; }
    public bool ApplyToAll {get; set;}
}