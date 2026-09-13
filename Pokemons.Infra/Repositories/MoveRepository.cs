using Microsoft.EntityFrameworkCore;
using Pokemons.Domain.Entities;
using Pokemons.Domain.Repositories;

namespace Pokemons.Infra.Repositories;

public class MoveRepository(PokemonsDbContext dbContext) : IMoveRepository
{
    public Task<Move?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        dbContext.Moves.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

    public Task<List<Move>> GetAllAsync(CancellationToken cancellationToken = default) =>
        dbContext.Moves.ToListAsync(cancellationToken);

    public Task<List<BasePokemon>> GetBasePokemonSharingMoveAsync(int moveId, CancellationToken cancellationToken = default) =>
        dbContext.MyPokemonMoves
            .Where(mpm => mpm.MoveId == moveId)
            .Select(mpm => mpm.MyPokemon!.BasePokemonId)
            .Distinct()
            .Join(dbContext.BasePokemons, id => id, bp => bp.Id, (id, bp) => bp)
            .ToListAsync(cancellationToken);

    public void Add(Move move) => dbContext.Moves.Add(move);

    public void Update(Move move) => dbContext.Moves.Update(move);

    public void Remove(Move move) => dbContext.Moves.Remove(move);
}
