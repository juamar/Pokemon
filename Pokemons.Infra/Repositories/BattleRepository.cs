using Microsoft.EntityFrameworkCore;
using Pokemons.Domain.Entities;
using Pokemons.Domain.Repositories;

namespace Pokemons.Infra.Repositories;

internal class BattleRepository(PokemonsDbContext dbContext) : IBattleRepository
{
    public Task<Battle?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        dbContext.Battles
            .Include(b => b.Pokemon1)
                .ThenInclude(p => p!.Moves)
            .Include(b => b.Pokemon2)
                .ThenInclude(p => p!.Moves)
            .Include(b => b.Actions)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

    public Task<List<Battle>> GetAllAsync(CancellationToken cancellationToken = default) =>
        dbContext.Battles
            .Include(b => b.Pokemon1)
                .ThenInclude(p => p!.Moves)
            .Include(b => b.Pokemon2)
                .ThenInclude(p => p!.Moves)
            .Include(b => b.Actions)
            .ToListAsync(cancellationToken);

    public void Add(Battle battle) => dbContext.Battles.Add(battle);

    public void UpdateStatus(Battle battle) => dbContext.Battles.Update(battle);

    public void AddAction(BattleAction action) => dbContext.BattleActions.Add(action);
}
