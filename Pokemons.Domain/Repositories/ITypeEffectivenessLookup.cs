namespace Pokemons.Domain.Repositories;

/// <summary>
/// Domain-facing lookup for the type effectiveness matrix (attacking move type vs defending Pokemon type).
/// </summary>
public interface ITypeEffectivenessLookup
{
    Task<double> GetMultiplierAsync(string attackerType, string defenderType, CancellationToken cancellationToken = default);
}
