using Pokemons.Domain;
using Pokemons.Domain.Entities;
using Pokemons.Domain.Repositories;

namespace Pokemons.Tests;

public class DamageCalculatorTests
{
    [Fact]
    public async Task CalculateAsync_WeaknessCase_FromSmokeTestValues_ReturnsExpectedDamageAtRandom100()
    {
        var calculator = CreateCalculator(effectiveness: 2, randomValue: 100);
        var attacker = new BasePokemon { Level = 5, BaseAttack = 48, Type = "Agua" };
        var move = new Move { Type = "Agua", Power = 40 };
        var defender = new BasePokemon { Type = "Fuego", BaseDefense = 43 };

        var damage = await calculator.CalculateAsync(attacker, move, defender);

        Assert.Equal(7, damage);
    }

    [Fact]
    public async Task CalculateAsync_WeaknessCase_FromSmokeTestValues_ReturnsExpectedDamageAtRandom85()
    {
        var calculator = CreateCalculator(effectiveness: 2, randomValue: 85);
        var attacker = new BasePokemon { Level = 5, BaseAttack = 48, Type = "Agua" };
        var move = new Move { Type = "Agua", Power = 40 };
        var defender = new BasePokemon { Type = "Fuego", BaseDefense = 43 };

        var damage = await calculator.CalculateAsync(attacker, move, defender);

        Assert.Equal(6, damage);
    }

    [Fact]
    public async Task CalculateAsync_ResistanceMultiplier_ReducesDamage()
    {
        var calculator = CreateCalculator(effectiveness: 0.5, randomValue: 100);
        var attacker = new BasePokemon { Level = 5, BaseAttack = 48, Type = "Agua" };
        var move = new Move { Type = "Agua", Power = 40 };
        var defender = new BasePokemon { Type = "Planta", BaseDefense = 43 };

        var damage = await calculator.CalculateAsync(attacker, move, defender);

        Assert.Equal(1, damage);
    }

    [Fact]
    public async Task CalculateAsync_NeutralMultiplier_UsesBaseDamage()
    {
        var calculator = CreateCalculator(effectiveness: 1, randomValue: 100);
        var attacker = new BasePokemon { Level = 5, BaseAttack = 48, Type = "Agua" };
        var move = new Move { Type = "Normal", Power = 40 };
        var defender = new BasePokemon { Type = "Normal", BaseDefense = 43 };

        var damage = await calculator.CalculateAsync(attacker, move, defender);

        Assert.Equal(3, damage);
    }

    [Fact]
    public async Task CalculateAsync_ImmunityMultiplier_ReturnsZero()
    {
        var calculator = CreateCalculator(effectiveness: 0, randomValue: 100);
        var attacker = new BasePokemon { Level = 5, BaseAttack = 48, Type = "Normal" };
        var move = new Move { Type = "Normal", Power = 40 };
        var defender = new BasePokemon { Type = "Fantasma", BaseDefense = 43 };

        var damage = await calculator.CalculateAsync(attacker, move, defender);

        Assert.Equal(0, damage);
    }

    [Fact]
    public async Task CalculateAsync_UsesRandomFactorFromProvider()
    {
        var lookup = new FixedTypeEffectivenessLookup(1);
        var random = new AssertingRandomProvider(returnValue: 100);
        var calculator = new DamageCalculator(lookup, random);

        var attacker = new BasePokemon { Level = 5, BaseAttack = 48, Type = "Normal" };
        var move = new Move { Type = "Normal", Power = 40 };
        var defender = new BasePokemon { Type = "Normal", BaseDefense = 43 };

        _ = await calculator.CalculateAsync(attacker, move, defender);

        Assert.Equal(1, random.CallCount);
    }

    [Fact]
    public async Task CalculateAsync_WithZeroDefense_ThrowsArgumentOutOfRangeException()
    {
        var calculator = CreateCalculator(effectiveness: 1, randomValue: 100);
        var attacker = new BasePokemon { Level = 5, BaseAttack = 48, Type = "Normal" };
        var move = new Move { Type = "Normal", Power = 40 };
        var defender = new BasePokemon { Type = "Normal", BaseDefense = 0 };

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => calculator.CalculateAsync(attacker, move, defender));
    }

    private static DamageCalculator CreateCalculator(double effectiveness, int randomValue)
    {
        var lookup = new FixedTypeEffectivenessLookup(effectiveness);
        var random = new AssertingRandomProvider(randomValue);
        return new DamageCalculator(lookup, random);
    }

    private sealed class FixedTypeEffectivenessLookup(double multiplier) : ITypeEffectivenessLookup
    {
        public Task<double> GetMultiplierAsync(string attackerType, string defenderType, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(multiplier);
        }
    }

    private sealed class AssertingRandomProvider(int returnValue) : IRandomProvider
    {
        public int CallCount { get; private set; }

        public int NextDamageFactorPercentage()
        {
            CallCount++;

            if (returnValue < 85 || returnValue > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(returnValue));
            }

            return returnValue;
        }
    }
}
