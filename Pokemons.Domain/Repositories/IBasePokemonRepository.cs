using Pokemons.Domain.Entities;

namespace Pokemons.Domain.Repositories;

public interface IBasePokemonRepository
{
    Task<BasePokemon?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<BasePokemon>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<Move>> GetPossibleMovesAsync(int basePokemonId, CancellationToken cancellationToken = default);
    Task<List<BasePokemon>> GetByMoveAsync(int moveId, CancellationToken cancellationToken = default);
    void Add(BasePokemon basePokemon);
    void Update(BasePokemon basePokemon);
    void Remove(BasePokemon basePokemon);
}
