namespace Pokemons.Domain.Entities;

/// <summary>
/// One executed turn/action within a Battle (the battle's persisted history).
/// Never exists independently of its parent Battle.
/// </summary>
public class BattleAction
{
    public int Id { get; set; }
    public int BattleId { get; set; }
    public Battle? Battle { get; set; }
    public int ActingPokemonId { get; set; }
    public MyPokemon? ActingPokemon { get; set; }
    public int MoveId { get; set; }
    public Move? Move { get; set; }
    public int DamageDealt { get; set; }
    public int TurnNumber { get; set; }
    public DateTime ExecutedAt { get; set; }
}
