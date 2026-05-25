namespace Fluxo.Lancamentos.Service.Core.Consolidar;

public sealed record ConsolidarDiaCommand(
    [Required] DateOnly ProximaData) : IMessage;