using Microsoft.EntityFrameworkCore;
using Pokemons.Domain.Entities;
using Pokemons.Domain.Repositories;

namespace Pokemons.Infra.Repositories;

internal class BasePokemonRepository(PokemonsDbContext dbContext) : IBasePokemonRepository
{
    public Task<BasePokemon?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        dbContext.BasePokemons.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public Task<List<BasePokemon>> GetAllAsync(CancellationToken cancellationToken = default) =>
        dbContext.BasePokemons.ToListAsync(cancellationToken);

    public void Add(BasePokemon basePokemon) => dbContext.BasePokemons.Add(basePokemon);

    public void Update(BasePokemon basePokemon) => dbContext.BasePokemons.Update(basePokemon);

    public void Remove(BasePokemon basePokemon) => dbContext.BasePokemons.Remove(basePokemon);
}
