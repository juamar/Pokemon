using Pokemons.Domain.Entities;

namespace Pokemons.Domain.Repositories;

public interface IMyPokemonRepository
{
    Task<MyPokemon?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<MyPokemon>> GetAllAsync(CancellationToken cancellationToken = default);
    void Add(MyPokemon myPokemon);
    void Update(MyPokemon myPokemon);
    void Remove(MyPokemon myPokemon);

    // Move management
    Task<bool> HasMoveAsync(int myPokemonId, int moveId, CancellationToken cancellationToken = default);
    Task<int> GetMoveCountAsync(int myPokemonId, CancellationToken cancellationToken = default);
    void AddMove(int myPokemonId, int moveId, string moveName, int movePower, string moveType);
    void RemoveMove(int myPokemonId, int moveId);
    Task<List<Move>> GetMovesAsync(int myPokemonId, CancellationToken cancellationToken = default);
}
