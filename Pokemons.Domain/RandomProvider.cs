namespace Pokemons.Domain;

public class RandomProvider : IRandomProvider
{
    private readonly Random _random = new();

    public int NextDamageFactorPercentage()
    {
        //GreaterThanOrEqualTo 85 and LessThanOrEqualTo 100
        return _random.Next(85, 101);
    }
}
