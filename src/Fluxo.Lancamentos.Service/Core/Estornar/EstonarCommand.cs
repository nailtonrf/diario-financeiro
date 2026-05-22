namespace Fluxo.Lancamentos.Service.Core.Estornar;

/// <summary>
///     Estornar Lançamento.
/// </summary>
/// <param name="LancamentoId"></param>
public sealed record EstornarCommand(
    [Required(AllowEmptyStrings = false)] string LancamentoId) : IMessage;