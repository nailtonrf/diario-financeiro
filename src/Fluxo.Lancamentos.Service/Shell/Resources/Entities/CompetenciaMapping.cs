using Fluxo.Lancamentos.Service.Core;

namespace Fluxo.Lancamentos.Service.Shell.Resources.Entities;

public class CompetenciaMapping : IEntityTypeConfiguration<Competencia>
{
    public void Configure(
        EntityTypeBuilder<Competencia> builder)
    {
        builder.ToTable("Competencia");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.DataCompetencia)
            .IsRequired();
    }
}