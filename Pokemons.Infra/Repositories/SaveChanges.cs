using Pokemons.Domain.Repositories;

namespace Pokemons.Infra.Repositories;

public class SaveChanges(PokemonsDbContext dbContext) : ISaveChanges
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
