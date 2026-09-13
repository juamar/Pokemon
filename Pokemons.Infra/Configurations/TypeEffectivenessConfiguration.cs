using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pokemons.Domain.Entities;

namespace Pokemons.Infra.Configurations;

public class TypeEffectivenessConfiguration : IEntityTypeConfiguration<TypeEffectiveness>
{
    public void Configure(EntityTypeBuilder<TypeEffectiveness> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.AttackerType).IsRequired().HasMaxLength(50);
        builder.Property(t => t.DefenderType).IsRequired().HasMaxLength(50);
        builder.HasIndex(t => new { t.AttackerType, t.DefenderType }).IsUnique();
        builder.HasData(SeedData.TypeEffectivenesses);
    }
}
