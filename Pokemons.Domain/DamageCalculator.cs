using Pokemons.Domain.Entities;
using Pokemons.Domain.Repositories;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Timers;

namespace Pokemons.Domain;

public class DamageCalculator(ITypeEffectivenessLookup typeEffectivenessLookup, IRandomProvider randomProvider)
{
    public async Task<int> CalculateAsync(
        BasePokemon attacker,
        Move selectedMove,
        BasePokemon defender,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(attacker);
        ArgumentNullException.ThrowIfNull(selectedMove);
        ArgumentNullException.ThrowIfNull(defender);

        // Defense is used as a divisor in the damage formula, so values less than or equal to zero
        // must be rejected to avoid division-by-zero and nonsensical damage results.
        if (defender.BaseDefense <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(defender.BaseDefense), "Defender base defense must be greater than zero.");
        }

        // Since the type matrix is static, this is a good candidate for caching or loading into
        // memory once instead of querying every time.
        var effectiveness = await typeEffectivenessLookup.GetMultiplierAsync(
            selectedMove.Type,
            defender.Type,
            cancellationToken);

        var randomFactor = randomProvider.NextDamageFactorPercentage() / 100.0;

        var baseDamage = (((2 * attacker.Level / 5.0) + 2) * attacker.BaseAttack * selectedMove.Power / defender.BaseDefense) / 50.0;
        var totalDamage = baseDamage * effectiveness * randomFactor;

        return (int)totalDamage;
    }
}
