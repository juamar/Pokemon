namespace Pokemons.Domain.Repositories;

/// <summary>
/// Minimal Unit-of-Work seam: a direct pass-through to the underlying persistence's
/// SaveChanges, needed for cross-aggregate atomicity (e.g. a battle turn touching
/// both MyPokemon and Battle/BattleAction). No Begin/Commit/Rollback ceremony.
/// </summary>
public interface ISaveChanges
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
