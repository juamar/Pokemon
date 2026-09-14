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

    public async Task<List<Move>> GetPossibleMovesAsync(int basePokemonId, CancellationToken cancellationToken = default)
    {
        var basePokemon = await dbContext.BasePokemons.FirstOrDefaultAsync(p => p.Id == basePokemonId, cancellationToken);
        if (basePokemon is null)
        {
            return [];
        }

        // V1 simplification: A move is "possible" for a BasePokemon if it shares the same type.
        // In a real Pokemon game, there would be an explicit move-learning table (level-up, TM, tutor, etc.).
        return await dbContext.Moves
            .Where(m => m.Type == basePokemon.Type)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<BasePokemon>> GetByMoveAsync(int moveId, CancellationToken cancellationToken = default)
    {
        var move = await dbContext.Moves.FirstOrDefaultAsync(m => m.Id == moveId, cancellationToken);
        if (move is null)
        {
            return [];
        }

        // V1 simplification: Return all BasePokemon that share the move's type (i.e., could theoretically learn this move).
        // In a real Pokemon game, you'd join against an explicit move-learning table.
        return await dbContext.BasePokemons
            .Where(p => p.Type == move.Type)
            .ToListAsync(cancellationToken);
    }

    public void Add(BasePokemon basePokemon) => dbContext.BasePokemons.Add(basePokemon);

    public void Update(BasePokemon basePokemon) => dbContext.BasePokemons.Update(basePokemon);

    public void Remove(BasePokemon basePokemon) => dbContext.BasePokemons.Remove(basePokemon);
}
