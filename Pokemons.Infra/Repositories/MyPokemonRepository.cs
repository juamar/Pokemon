using Microsoft.EntityFrameworkCore;
using Pokemons.Domain.Entities;
using Pokemons.Domain.Repositories;

namespace Pokemons.Infra.Repositories;

public class MyPokemonRepository(PokemonsDbContext dbContext) : IMyPokemonRepository
{
    public Task<MyPokemon?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        dbContext.MyPokemons
            .Include(p => p.BasePokemon)
            .Include(p => p.Moves).ThenInclude(m => m.Move)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public Task<List<MyPokemon>> GetAllAsync(CancellationToken cancellationToken = default) =>
        dbContext.MyPokemons
            .Include(p => p.BasePokemon)
            .Include(p => p.Moves).ThenInclude(m => m.Move)
            .ToListAsync(cancellationToken);

    public void Add(MyPokemon myPokemon) => dbContext.MyPokemons.Add(myPokemon);

    public void Update(MyPokemon myPokemon) => dbContext.MyPokemons.Update(myPokemon);

    public void Remove(MyPokemon myPokemon) => dbContext.MyPokemons.Remove(myPokemon);
}
