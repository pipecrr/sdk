using System.Collections.Generic;

namespace Siesa.SDK.Shared.DTOS;

public abstract record SDKInteractionBase
{
    //Interaccion basada por accion
    public SDKActionDTO Action { get; set; }
    //Interaccion basada para acciones
    public List<SDKActionDTO> Actions { get; set; }
}

//Una vez se crea, no cambia
public record SDKActionInteractionsDTO : SDKInteractionBase
{
    public bool AnyActionCanAlter { get; set; }
    public List<SDKInteractionDetail> ActionsToAlter { get; set; }
}

//Una vez se crea, no cambia
public record SDKInteractionDetail : SDKInteractionBase
{
    //Con qué estado va a interactuar, cuando prenda o cuando apague
    public bool StatusToInteract { get; set; }
    public bool ApplyToAll {get; set;}
}