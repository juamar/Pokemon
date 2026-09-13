using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pokemons.Domain.Repositories;
using Pokemons.Infra.Repositories;

namespace Pokemons.Infra.DependencyInjection;

public static class PersistenceServiceCollectionExtensions
{
    /// <summary>
    /// Single DI seam for persistence: registers PokemonsDbContext (SQLite today, swappable
    /// to another EF Core provider later) and all repository implementations against their
    /// Pokemons.Domain interfaces.
    /// </summary>
    public static IServiceCollection AddPokemonsPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PokemonsDb") ?? "Data Source=pokemons.db";

        services.AddDbContext<PokemonsDbContext>(options => options.UseSqlite(connectionString));

        services.AddScoped<IBasePokemonRepository, BasePokemonRepository>();
        services.AddScoped<IMoveRepository, MoveRepository>();
        services.AddScoped<IMyPokemonRepository, MyPokemonRepository>();
        services.AddScoped<IBattleRepository, BattleRepository>();
        services.AddScoped<ITypeEffectivenessLookup, TypeEffectivenessLookup>();
        services.AddScoped<ISaveChanges, SaveChanges>();

        return services;
    }
}
