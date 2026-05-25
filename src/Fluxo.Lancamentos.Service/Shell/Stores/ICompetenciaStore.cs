namespace Fluxo.Lancamentos.Service.Shell.Stores;

using Core;

public interface ICompetenciaStore
{
    Task<Option<Competencia>> GetAsync(CancellationToken cancellationToken);
    Result<Competencia> Save(Competencia competencia);
}