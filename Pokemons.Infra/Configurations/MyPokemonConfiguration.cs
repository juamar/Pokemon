using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pokemons.Domain.Entities;

namespace Pokemons.Infra.Configurations;

public class MyPokemonConfiguration : IEntityTypeConfiguration<MyPokemon>
{
    public void Configure(EntityTypeBuilder<MyPokemon> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.OwnerId).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Type).IsRequired().HasMaxLength(50);

        builder.HasOne(p => p.BasePokemon)
            .WithMany()
            .HasForeignKey(p => p.BasePokemonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Moves)
            .WithOne(m => m.MyPokemon)
            .HasForeignKey(m => m.MyPokemonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
