using Pokemons.Domain.Entities;

namespace Pokemons.Domain.Repositories;

public interface IMoveRepository
{
    Task<Move?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Move>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Base Pokemon that share a given move (via MyPokemon assignments in V1 data, per requirements §2).</summary>
    Task<List<BasePokemon>> GetBasePokemonSharingMoveAsync(int moveId, CancellationToken cancellationToken = default);

    void Add(Move move);
    void Update(Move move);
    void Remove(Move move);
}
