using Pokemons.Domain;

namespace Pokemons.Tests;

public class RandomProviderTests
{
    [Fact]
    public void NextDamageFactorPercentage_ReturnsValuesBetween85And100Inclusive()
    {
        var provider = new RandomProvider();

        var observedMin = int.MaxValue;
        var observedMax = int.MinValue;

        for (var i = 0; i < 10_000; i++)
        {
            var value = provider.NextDamageFactorPercentage();
            observedMin = Math.Min(observedMin, value);
            observedMax = Math.Max(observedMax, value);
        }

        Assert.Equal(85, observedMin);
        Assert.Equal(100, observedMax);
    }
}
