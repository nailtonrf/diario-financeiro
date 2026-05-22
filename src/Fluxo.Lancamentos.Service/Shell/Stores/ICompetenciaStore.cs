using Fluxo.Lancamentos.Service.Core;

namespace Fluxo.Lancamentos.Service.Shell.Stores;

public interface ICompetenciaStore
{
    Task<Option<Competencia>> GetAsync(CancellationToken cancellationToken);
}