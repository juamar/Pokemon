using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pokemons.Domain.Entities;

namespace Pokemons.Infra.Configurations;

public class BattleConfiguration : IEntityTypeConfiguration<Battle>
{
    public void Configure(EntityTypeBuilder<Battle> builder)
    {
        builder.HasKey(b => b.Id);

        builder.HasOne(b => b.Pokemon1)
            .WithMany()
            .HasForeignKey(b => b.Pokemon1Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Pokemon2)
            .WithMany()
            .HasForeignKey(b => b.Pokemon2Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(b => b.Actions)
            .WithOne(a => a.Battle)
            .HasForeignKey(a => a.BattleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
