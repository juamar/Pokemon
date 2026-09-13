using Pokemons.Domain.Entities;

namespace Pokemons.Domain.Repositories;

/// <summary>
/// Battle and BattleAction share one repository since a BattleAction never exists
/// independently of its parent Battle (it's the turn-by-turn history of that battle).
/// </summary>
public interface IBattleRepository
{
    Task<Battle?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Battle>> GetAllAsync(CancellationToken cancellationToken = default);
    void Add(Battle battle);
    void UpdateStatus(Battle battle);
    void AddAction(BattleAction action);
}
