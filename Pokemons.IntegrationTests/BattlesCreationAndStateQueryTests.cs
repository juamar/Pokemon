extern alias BattlesApi;
extern alias PokedexApi;

using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Pokemons.Infra.DependencyInjection;

namespace Pokemons.IntegrationTests;

public class BattlesCreationAndStateQueryTests(BattleApisFixture fixture) : IClassFixture<BattleApisFixture>
{
    private readonly HttpClient _pokedexClient = fixture.PokedexClient;
    private readonly HttpClient _battlesClient = fixture.BattlesClient;

    [Fact]
    public async Task CreateBattle_WithValidParticipants_ReturnsStarted()
    {
        var pokemon1 = await CreateMyPokemonWithAssignedMoveAsync("battle-trainer-1", "Alpha");
        var pokemon2 = await CreateMyPokemonWithAssignedMoveAsync("battle-trainer-2", "Bravo");

        var request = new CreateBattleRequest
        {
            Pokemon1Id = pokemon1.Id,
            Pokemon2Id = pokemon2.Id
        };

        var response = await _battlesClient.PostAsJsonAsync("/api/battles", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<BattleCreatedDto>();
        Assert.NotNull(created);
        Assert.Equal(BattleStatusDto.Started, created!.Status);
        Assert.Equal(pokemon1.Id, created.Pokemon1Id);
        Assert.Equal(pokemon2.Id, created.Pokemon2Id);
        Assert.Equal(pokemon1.Id, created.CurrentTurnPokemonId);
    }

    [Fact]
    public async Task GetBattleState_ById_ReturnsParticipantsAndStartedState()
    {
        var pokemon1 = await CreateMyPokemonWithAssignedMoveAsync("battle-trainer-3", "Gamma");
        var pokemon2 = await CreateMyPokemonWithAssignedMoveAsync("battle-trainer-4", "Delta");

        var createRequest = new CreateBattleRequest
        {
            Pokemon1Id = pokemon1.Id,
            Pokemon2Id = pokemon2.Id
        };

        var createResponse = await _battlesClient.PostAsJsonAsync("/api/battles", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<BattleCreatedDto>();
        Assert.NotNull(created);

        var getResponse = await _battlesClient.GetAsync($"/api/battles/{created!.Id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var state = await getResponse.Content.ReadFromJsonAsync<BattleStateDto>();
        Assert.NotNull(state);
        Assert.Equal(BattleStatusDto.Started, state!.Status);
        Assert.Equal(pokemon1.Id, state.Pokemon1Id);
        Assert.Equal(pokemon2.Id, state.Pokemon2Id);
        Assert.Equal(pokemon1.Id, state.CurrentTurnPokemonId);
        Assert.NotNull(state.Pokemon1);
        Assert.NotNull(state.Pokemon2);
        Assert.Empty(state.Actions);
    }

    private async Task<MyPokemonDto> CreateMyPokemonWithAssignedMoveAsync(string ownerId, string name)
    {
        var basePokemonsResponse = await _pokedexClient.GetAsync("/api/base-pokemons");
        var basePokemons = await basePokemonsResponse.Content.ReadFromJsonAsync<List<BasePokemonDto>>();
        Assert.NotNull(basePokemons);
        Assert.NotEmpty(basePokemons);

        var createMyPokemonRequest = new CreateMyPokemonRequest
        {
            OwnerId = ownerId,
            BasePokemonId = basePokemons![0].Id,
            Name = name
        };

        var createMyPokemonResponse = await _pokedexClient.PostAsJsonAsync("/api/my-pokemons", createMyPokemonRequest);
        Assert.Equal(HttpStatusCode.Created, createMyPokemonResponse.StatusCode);

        var myPokemon = await createMyPokemonResponse.Content.ReadFromJsonAsync<MyPokemonDto>();
        Assert.NotNull(myPokemon);

        var movesResponse = await _pokedexClient.GetAsync("/api/moves");
        var moves = await movesResponse.Content.ReadFromJsonAsync<List<MoveDto>>();
        Assert.NotNull(moves);
        Assert.NotEmpty(moves);

        var assignResponse = await _pokedexClient.PostAsJsonAsync(
            $"/api/my-pokemons/{myPokemon!.Id}/moves",
            new AssignMoveRequest { MoveId = moves![0].Id });

        Assert.Equal(HttpStatusCode.Created, assignResponse.StatusCode);

        return myPokemon;
    }

    public sealed class CreateBattleRequest
    {
        public int Pokemon1Id { get; set; }
        public int Pokemon2Id { get; set; }
    }

    public sealed class BattleCreatedDto
    {
        public int Id { get; set; }
        public BattleStatusDto Status { get; set; }
        public int Pokemon1Id { get; set; }
        public int Pokemon2Id { get; set; }
        public int CurrentTurnPokemonId { get; set; }
        public DateTime StartedAt { get; set; }
    }

    public sealed class BattleStateDto
    {
        public int Id { get; set; }
        public BattleStatusDto Status { get; set; }
        public int Pokemon1Id { get; set; }
        public int Pokemon2Id { get; set; }
        public int CurrentTurnPokemonId { get; set; }
        public int? WinnerPokemonId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public BattlePokemonDto? Pokemon1 { get; set; }
        public BattlePokemonDto? Pokemon2 { get; set; }
        public List<BattleActionDto> Actions { get; set; } = [];
    }

    public sealed class BattlePokemonDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CurrentHP { get; set; }
        public int TotalHP { get; set; }
    }

    public sealed class BattleActionDto
    {
        public int Id { get; set; }
        public int TurnNumber { get; set; }
        public int ActingPokemonId { get; set; }
        public int MoveId { get; set; }
        public int DamageDealt { get; set; }
        public DateTime ExecutedAt { get; set; }
    }

    public enum BattleStatusDto
    {
        Started,
        Finished
    }

    public sealed class CreateMyPokemonRequest
    {
        public string OwnerId { get; set; } = string.Empty;
        public int BasePokemonId { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public sealed class AssignMoveRequest
    {
        public int MoveId { get; set; }
    }

    public sealed class MyPokemonDto
    {
        public int Id { get; set; }
        public string OwnerId { get; set; } = string.Empty;
        public int BasePokemonId { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public sealed class BasePokemonDto
    {
        public int Id { get; set; }
    }

    public sealed class MoveDto
    {
        public int Id { get; set; }
    }
}

public sealed class BattleApisFixture : IAsyncLifetime
{
    private readonly string _databasePath = Path.Combine(Path.GetTempPath(), $"battle-integration-{Guid.NewGuid():N}.db");
    private SharedPokedexApiFactory? _pokedexFactory;
    private SharedBattlesApiFactory? _battlesFactory;

    public HttpClient PokedexClient { get; private set; } = default!;
    public HttpClient BattlesClient { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        _pokedexFactory = new SharedPokedexApiFactory(_databasePath);
        _battlesFactory = new SharedBattlesApiFactory(_databasePath);

        await _pokedexFactory.Services.ApplyPokemonsMigrationsAsync();

        PokedexClient = _pokedexFactory.CreateClient();
        BattlesClient = _battlesFactory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        PokedexClient.Dispose();
        BattlesClient.Dispose();

        if (_pokedexFactory is not null)
        {
            await _pokedexFactory.DisposeAsync();
        }

        if (_battlesFactory is not null)
        {
            await _battlesFactory.DisposeAsync();
        }

        if (File.Exists(_databasePath))
        {
            File.Delete(_databasePath);
        }
    }

    private sealed class SharedPokedexApiFactory(string databasePath) : WebApplicationFactory<PokedexApi::Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((_, configBuilder) =>
            {
                configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:PokemonsDb"] = $"Data Source={databasePath}"
                });
            });
        }
    }

    private sealed class SharedBattlesApiFactory(string databasePath) : WebApplicationFactory<BattlesApi::Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((_, configBuilder) =>
            {
                configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:PokemonsDb"] = $"Data Source={databasePath}"
                });
            });
        }
    }
}
