using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pokemons.Domain.Entities;

namespace Pokemons.Infra.Configurations;

public class MyPokemonMoveConfiguration : IEntityTypeConfiguration<MyPokemonMove>
{
    public void Configure(EntityTypeBuilder<MyPokemonMove> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Name).IsRequired().HasMaxLength(100);
        builder.Property(m => m.Type).IsRequired().HasMaxLength(50);

        builder.HasOne(m => m.Move)
            .WithMany()
            .HasForeignKey(m => m.MoveId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
