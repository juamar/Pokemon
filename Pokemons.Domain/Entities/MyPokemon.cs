namespace Pokemons.Domain.Entities;

/// <summary>
/// An owned instance of a BasePokemon (V1: no trainers/auth, OwnerId is a plain string).
/// </summary>
public class MyPokemon
{
    public int Id { get; set; }
    public string OwnerId { get; set; } = string.Empty;
    public int BasePokemonId { get; set; }
    public BasePokemon? BasePokemon { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Level { get; set; }
    public int CurrentHP { get; set; }
    public int TotalHP { get; set; }
    public int BaseAttack { get; set; }
    public int BaseDefense { get; set; }
    public int BaseSpecialAttack { get; set; }
    public int BaseSpecialDefense { get; set; }
    public int BaseSpeed { get; set; }
    public List<MyPokemonMove> Moves { get; set; } = [];
}
