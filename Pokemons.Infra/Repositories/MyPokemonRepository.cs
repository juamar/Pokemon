using Microsoft.EntityFrameworkCore;
using Pokemons.Domain.Entities;
using Pokemons.Domain.Repositories;

namespace Pokemons.Infra.Repositories;

internal class MyPokemonRepository(PokemonsDbContext dbContext) : IMyPokemonRepository
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

    public async Task<bool> HasMoveAsync(int myPokemonId, int moveId, CancellationToken cancellationToken = default) =>
        await dbContext.MyPokemonMoves
            .AnyAsync(m => m.MyPokemonId == myPokemonId && m.MoveId == moveId, cancellationToken);

    public async Task<int> GetMoveCountAsync(int myPokemonId, CancellationToken cancellationToken = default) =>
        await dbContext.MyPokemonMoves
            .CountAsync(m => m.MyPokemonId == myPokemonId, cancellationToken);

    public void AddMove(int myPokemonId, int moveId, string moveName, int movePower, string moveType)
    {
        var myPokemonMove = new MyPokemonMove
        {
            MyPokemonId = myPokemonId,
            MoveId = moveId,
            Name = moveName,
            Power = movePower,
            Type = moveType
        };
        dbContext.MyPokemonMoves.Add(myPokemonMove);
    }

    public void RemoveMove(int myPokemonId, int moveId)
    {
        var myPokemonMove = dbContext.MyPokemonMoves
            .FirstOrDefault(m => m.MyPokemonId == myPokemonId && m.MoveId == moveId);

        if (myPokemonMove is not null)
        {
            dbContext.MyPokemonMoves.Remove(myPokemonMove);
        }
    }

    public async Task<List<Move>> GetMovesAsync(int myPokemonId, CancellationToken cancellationToken = default) =>
        await dbContext.MyPokemonMoves
            .Where(m => m.MyPokemonId == myPokemonId)
            .Include(m => m.Move)
            .Select(m => m.Move)
            .ToListAsync(cancellationToken);
}

