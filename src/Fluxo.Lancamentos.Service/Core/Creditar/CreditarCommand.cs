namespace Fluxo.Lancamentos.Service.Core.Creditar;

/// <summary>
///     Creditar valor no fluxo da empresa.
/// </summary>
/// <param name="Descricao"></param>
/// <param name="Valor"></param>
public sealed record CreditarCommand(
    [Required(AllowEmptyStrings = false)]
    [MaxLength(100)]
    string Descricao,
    [Required] [Range(1, 999_999_999)] decimal Valor) : IMessage;