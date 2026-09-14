extern alias PokedexApi;

using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Pokemons.Infra.DependencyInjection;

namespace Pokemons.IntegrationTests;

public class BasePokemonsCrudTests(PokedexApiFactory factory) : IClassFixture<PokedexApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetAll_ReturnsOkWithSeededData()
    {
        var response = await _client.GetAsync("/api/base-pokemons");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var basePokemons = await response.Content.ReadFromJsonAsync<List<BasePokemonDto>>();
        Assert.NotNull(basePokemons);
        Assert.NotEmpty(basePokemons);
    }

    [Fact]
    public async Task Create_ThenGetById_ReturnsCreatedPokemon()
    {
        var createRequest = CreateRequest($"CreatedMon-{Guid.NewGuid():N}");

        var createResponse = await _client.PostAsJsonAsync("/api/base-pokemons", createRequest);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<BasePokemonDto>();
        Assert.NotNull(created);

        var getResponse = await _client.GetAsync($"/api/base-pokemons/{created!.Id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var fetched = await getResponse.Content.ReadFromJsonAsync<BasePokemonDto>();
        Assert.NotNull(fetched);
        Assert.Equal(createRequest.Name, fetched!.Name);
        Assert.Equal(createRequest.Type, fetched.Type);
    }

    [Fact]
    public async Task Update_ExistingPokemon_ReturnsUpdatedEntity()
    {
        var createRequest = CreateRequest($"UpdateMon-{Guid.NewGuid():N}");
        var createResponse = await _client.PostAsJsonAsync("/api/base-pokemons", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<BasePokemonDto>();
        Assert.NotNull(created);

        var updateRequest = CreateRequest($"Updated-{Guid.NewGuid():N}", type: "Fuego", level: 12);

        var updateResponse = await _client.PutAsJsonAsync($"/api/base-pokemons/{created!.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updated = await updateResponse.Content.ReadFromJsonAsync<BasePokemonDto>();
        Assert.NotNull(updated);
        Assert.Equal(updateRequest.Name, updated!.Name);
        Assert.Equal(updateRequest.Type, updated.Type);
        Assert.Equal(updateRequest.Level, updated.Level);
    }

    [Fact]
    public async Task Delete_ExistingPokemon_ReturnsNoContent_AndThenNotFound()
    {
        var createRequest = CreateRequest($"DeleteMon-{Guid.NewGuid():N}");
        var createResponse = await _client.PostAsJsonAsync("/api/base-pokemons", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<BasePokemonDto>();
        Assert.NotNull(created);

        var deleteResponse = await _client.DeleteAsync($"/api/base-pokemons/{created!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getDeletedResponse = await _client.GetAsync($"/api/base-pokemons/{created.Id}");

        Assert.Equal(HttpStatusCode.NotFound, getDeletedResponse.StatusCode);
    }

    private static BasePokemonRequest CreateRequest(string name, string type = "Agua", int level = 8)
    {
        return new BasePokemonRequest
        {
            Name = name,
            Type = type,
            Level = level,
            TotalHP = 50,
            BaseAttack = 45,
            BaseDefense = 40,
            BaseSpecialAttack = 50,
            BaseSpecialDefense = 50,
            BaseSpeed = 45
        };
    }

    public sealed class BasePokemonRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Level { get; set; }
        public int TotalHP { get; set; }
        public int BaseAttack { get; set; }
        public int BaseDefense { get; set; }
        public int BaseSpecialAttack { get; set; }
        public int BaseSpecialDefense { get; set; }
        public int BaseSpeed { get; set; }
    }

    public sealed class BasePokemonDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Level { get; set; }
        public int TotalHP { get; set; }
        public int BaseAttack { get; set; }
        public int BaseDefense { get; set; }
        public int BaseSpecialAttack { get; set; }
        public int BaseSpecialDefense { get; set; }
        public int BaseSpeed { get; set; }
    }
}

public sealed class PokedexApiFactory : WebApplicationFactory<PokedexApi::Program>, IAsyncLifetime
{
    private readonly string _databasePath = Path.Combine(Path.GetTempPath(), $"pokedex-integration-{Guid.NewGuid():N}.db");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:PokemonsDb"] = $"Data Source={_databasePath}"
            });
        });
    }

    public Task InitializeAsync()
    {
        return Services.ApplyPokemonsMigrationsAsync();
    }

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();

        if (File.Exists(_databasePath))
        {
            File.Delete(_databasePath);
        }
    }
}
