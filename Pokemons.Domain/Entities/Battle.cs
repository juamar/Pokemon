namespace Pokemons.Domain.Entities;

public enum BattleStatus
{
    Started,
    Finished
}

/// <summary>
/// A battle between two MyPokemon. Starts immediately in Started status (no "not started" state).
/// </summary>
public class Battle
{
    public int Id { get; set; }
    public int Pokemon1Id { get; set; }
    public MyPokemon? Pokemon1 { get; set; }
    public int Pokemon2Id { get; set; }
    public MyPokemon? Pokemon2 { get; set; }
    public BattleStatus Status { get; set; } = BattleStatus.Started;
    public int CurrentTurnPokemonId { get; set; }
    public int? WinnerPokemonId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public List<BattleAction> Actions { get; set; } = [];
}
