using Microsoft.EntityFrameworkCore;
using Pokemons.Domain.Repositories;

namespace Pokemons.Infra.Repositories;

internal class TypeEffectivenessLookup(PokemonsDbContext dbContext) : ITypeEffectivenessLookup
{
    public async Task<double> GetMultiplierAsync(string attackerType, string defenderType, CancellationToken cancellationToken = default)
    {
        var entry = await dbContext.TypeEffectivenesses
            .FirstOrDefaultAsync(t => t.AttackerType == attackerType && t.DefenderType == defenderType, cancellationToken);

        return entry?.Multiplier ?? 1;
    }
}
