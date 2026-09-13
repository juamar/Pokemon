namespace Pokemons.Domain.Entities;

/// <summary>
/// A single row of the type effectiveness matrix: AttackerType vs DefenderType -> Multiplier.
/// </summary>
public class TypeEffectiveness
{
    public int Id { get; set; }
    public string AttackerType { get; set; } = string.Empty;
    public string DefenderType { get; set; } = string.Empty;
    public double Multiplier { get; set; }
}
