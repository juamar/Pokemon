using Microsoft.EntityFrameworkCore;
using Pokemons.Domain.Entities;

namespace Pokemons.Infra;

public class PokemonsDbContext(DbContextOptions<PokemonsDbContext> options) : DbContext(options)
{
    public DbSet<BasePokemon> BasePokemons => Set<BasePokemon>();
    public DbSet<Move> Moves => Set<Move>();
    public DbSet<MyPokemon> MyPokemons => Set<MyPokemon>();
    public DbSet<MyPokemonMove> MyPokemonMoves => Set<MyPokemonMove>();
    public DbSet<Battle> Battles => Set<Battle>();
    public DbSet<BattleAction> BattleActions => Set<BattleAction>();
    public DbSet<TypeEffectiveness> TypeEffectivenesses => Set<TypeEffectiveness>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PokemonsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
