namespace Pokemons.Domain.Entities;

/// <summary>
/// A join entity representing one of up to 4 moves assigned to a MyPokemon.
/// Name/Type/Power are snapshotted from Move at assignment time (same rationale as
/// MyPokemon's stat snapshot from BasePokemon) so editing the Move catalog later
/// doesn't retroactively change the behavior of moves already assigned to owned
/// Pokemon, including ones mid-battle.
/// </summary>
public class MyPokemonMove
{
    public int Id { get; set; }
    public int MyPokemonId { get; set; }
    public MyPokemon? MyPokemon { get; set; }
    public int MoveId { get; set; }
    public Move? Move { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Power { get; set; }
}
