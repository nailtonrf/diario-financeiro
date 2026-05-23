using Fluxo.Lancamentos.Service.Core;
using Fluxo.Lancamentos.Service.Core.Creditar;
using Fluxo.Lancamentos.Service.Core.Debitar;
using Fluxo.Lancamentos.Service.Core.Estornar;

namespace Fluxo.Lancamentos.Service.Infra.EntityFramework.Mappings;

public sealed class LancamentosMapping : IEntityTypeConfiguration<Lancamento>
{
    public void Configure(EntityTypeBuilder<Lancamento> builder)
    {
        builder.ToTable("Lancamentos");

        builder.HasKey(x => x.IdLancamento);

        builder.Property(x => x.IdLancamento)
            .HasConversion(
                x => x.Id,
                x => new LancamentoId(x));

        builder.Property(x => x.Descricao)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(x => x.Data)
            .IsRequired();

        builder.Property(x => x.DataCompetencia)
            .IsRequired();

        builder.Property(x => x.Valor)
            .HasPrecision(18, 2);

        builder.HasDiscriminator<string>("TipoEvento")
            .HasValue<CreditoEfetuadoEvent>("Credito")
            .HasValue<DebitoEfetuadoEvent>("Debito")
            .HasValue<EstornoEfetuadoEvent>("Estorno");
    }
}

public sealed class EstornoMapping : IEntityTypeConfiguration<EstornoEfetuadoEvent>
{
    public void Configure(EntityTypeBuilder<EstornoEfetuadoEvent> builder)
    {
        builder.Property(x => x.IdEstornado)
            .HasConversion(
                x => x.Id,
                x => new LancamentoId(x));
    }
}