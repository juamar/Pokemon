using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pokemons.Domain.Entities;

namespace Pokemons.Infra.Configurations;

public class BasePokemonConfiguration : IEntityTypeConfiguration<BasePokemon>
{
    public void Configure(EntityTypeBuilder<BasePokemon> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Type).IsRequired().HasMaxLength(50);
        builder.HasData(SeedData.BasePokemons);
    }
}
