namespace Fluxo.Lancamentos.Service.Core;

public static class ValorPositivoModule
{
    public static Result<ValorPositivo> ValidarValorPositivo(this decimal valor)
        => valor > 0
            ? Ok(new ValorPositivo(valor))
            : ErrorResult.Validation("Valor deve ser maior que 0 (zero).");
}