using Pokemons.Domain.Entities;

namespace Pokemons.Domain.Repositories;

public interface IMyPokemonRepository
{
    Task<MyPokemon?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<MyPokemon>> GetAllAsync(CancellationToken cancellationToken = default);
    void Add(MyPokemon myPokemon);
    void Update(MyPokemon myPokemon);
    void Remove(MyPokemon myPokemon);
}
