namespace Pokemons.Domain;

public class RandomProvider : IRandomProvider
{
    private readonly Random _random = new();

    public int NextDamageFactorPercentage()
    {
        return _random.Next(85, 101);
    }
}
