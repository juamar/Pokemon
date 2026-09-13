using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pokemons.Domain.Entities;

namespace Pokemons.Infra.Configurations;

public class BattleActionConfiguration : IEntityTypeConfiguration<BattleAction>
{
    public void Configure(EntityTypeBuilder<BattleAction> builder)
    {
        builder.HasKey(a => a.Id);

        builder.HasOne(a => a.ActingPokemon)
            .WithMany()
            .HasForeignKey(a => a.ActingPokemonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Move)
            .WithMany()
            .HasForeignKey(a => a.MoveId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
