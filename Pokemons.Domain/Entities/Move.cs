namespace Pokemons.Domain.Entities;

/// <summary>
/// A move that can be assigned to a MyPokemon (V1: name + type + power only).
/// </summary>
public class Move
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Power { get; set; }
}
