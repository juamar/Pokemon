using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Pokemons.IntegrationTests;

public class MyPokemonsCrudTests(PokedexApiFactory factory) : IClassFixture<PokedexApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetAll_ReturnsOkWithList()
    {
        var response = await _client.GetAsync("/api/my-pokemons");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var myPokemons = await response.Content.ReadFromJsonAsync<List<MyPokemonDto>>();
        Assert.NotNull(myPokemons);
        Assert.IsType<List<MyPokemonDto>>(myPokemons);
    }

    [Fact]
    public async Task Create_WithValidBasePokemonId_ReturnsCreated()
    {
        // First, get a valid base pokemon ID from seeded data
        var basePokemonsResponse = await _client.GetAsync("/api/base-pokemons");
        var basePokemons = await basePokemonsResponse.Content.ReadFromJsonAsync<List<BasePokemonDto>>();
        Assert.NotNull(basePokemons);
        Assert.NotEmpty(basePokemons);

        var validBasePokemonId = basePokemons![0].Id;

        var createRequest = new CreateMyPokemonRequest
        {
            OwnerId = "trainer-001",
            BasePokemonId = validBasePokemonId,
            Name = "Flamey"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/my-pokemons", createRequest);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<MyPokemonDto>();
        Assert.NotNull(created);
        Assert.Equal(createRequest.Name, created!.Name);
        Assert.Equal(createRequest.OwnerId, created.OwnerId);
        Assert.Equal(validBasePokemonId, created.BasePokemonId);
    }

    [Fact]
    public async Task Create_WithInvalidBasePokemonId_ReturnsBadRequest()
    {
        var createRequest = new CreateMyPokemonRequest
        {
            OwnerId = "trainer-001",
            BasePokemonId = 99999,
            Name = "BadMon"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/my-pokemons", createRequest);

        Assert.Equal(HttpStatusCode.BadRequest, createResponse.StatusCode);
    }

    [Fact]
    public async Task GetById_WithValidId_ReturnsMyPokemon()
    {
        var created = await CreateMyPokemonAsync("trainer-002", "MyMon");
        Assert.NotNull(created);

        var getResponse = await _client.GetAsync($"/api/my-pokemons/{created!.Id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var fetched = await getResponse.Content.ReadFromJsonAsync<MyPokemonDto>();
        Assert.NotNull(fetched);
        Assert.Equal(created.Id, fetched!.Id);
        Assert.Equal(created.Name, fetched.Name);
    }

    [Fact]
    public async Task GetById_WithInvalidId_ReturnsNotFound()
    {
        var getResponse = await _client.GetAsync("/api/my-pokemons/99999");

        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Update_ExistingMyPokemon_ReturnsUpdated()
    {
        var created = await CreateMyPokemonAsync("trainer-003", "MonToUpdate");
        Assert.NotNull(created);

        var updateRequest = new UpdateMyPokemonRequest
        {
            Name = "UpdatedMon",
            Level = 25,
            CurrentHP = 45,
            TotalHP = 50,
            BaseAttack = 55,
            BaseDefense = 50,
            BaseSpecialAttack = 60,
            BaseSpecialDefense = 55,
            BaseSpeed = 70
        };

        var updateResponse = await _client.PutAsJsonAsync($"/api/my-pokemons/{created!.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updated = await updateResponse.Content.ReadFromJsonAsync<MyPokemonDto>();
        Assert.NotNull(updated);
        Assert.Equal(updateRequest.Name, updated!.Name);
        Assert.Equal(updateRequest.Level, updated.Level);
    }

    [Fact]
    public async Task Delete_ExistingMyPokemon_ReturnsNoContent()
    {
        var created = await CreateMyPokemonAsync("trainer-004", "MonToDelete");
        Assert.NotNull(created);

        var deleteResponse = await _client.DeleteAsync($"/api/my-pokemons/{created!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getDeletedResponse = await _client.GetAsync($"/api/my-pokemons/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getDeletedResponse.StatusCode);
    }

    [Fact]
    public async Task AssignMove_WithValidMoveId_ReturnsCreated()
    {
        var myPokemon = await CreateMyPokemonAsync("trainer-005", "MoveLearner");
        Assert.NotNull(myPokemon);

        // Get a valid move ID from seeded data
        var movesResponse = await _client.GetAsync("/api/moves");
        var moves = await movesResponse.Content.ReadFromJsonAsync<List<MoveDto>>();
        Assert.NotNull(moves);
        Assert.NotEmpty(moves);

        var moveId = moves![0].Id;

        var assignRequest = new AssignMoveRequest { MoveId = moveId };
        var assignResponse = await _client.PostAsJsonAsync($"/api/my-pokemons/{myPokemon!.Id}/moves", assignRequest);

        Assert.Equal(HttpStatusCode.Created, assignResponse.StatusCode);
    }

    [Fact]
    public async Task AssignMove_WithInvalidMoveId_ReturnsBadRequest()
    {
        var myPokemon = await CreateMyPokemonAsync("trainer-006", "FailLearner");
        Assert.NotNull(myPokemon);

        var assignRequest = new AssignMoveRequest { MoveId = 99999 };
        var assignResponse = await _client.PostAsJsonAsync($"/api/my-pokemons/{myPokemon!.Id}/moves", assignRequest);

        Assert.Equal(HttpStatusCode.BadRequest, assignResponse.StatusCode);
    }

    [Fact]
    public async Task AssignMove_DuplicateMove_ReturnsBadRequest()
    {
        var myPokemon = await CreateMyPokemonAsync("trainer-007", "DupeLearner");
        Assert.NotNull(myPokemon);

        var movesResponse = await _client.GetAsync("/api/moves");
        var moves = await movesResponse.Content.ReadFromJsonAsync<List<MoveDto>>();
        Assert.NotNull(moves);
        Assert.NotEmpty(moves);

        var moveId = moves![0].Id;

        // First assignment should succeed
        var assignRequest1 = new AssignMoveRequest { MoveId = moveId };
        var assignResponse1 = await _client.PostAsJsonAsync($"/api/my-pokemons/{myPokemon!.Id}/moves", assignRequest1);
        Assert.Equal(HttpStatusCode.Created, assignResponse1.StatusCode);

        // Second assignment of same move should fail
        var assignRequest2 = new AssignMoveRequest { MoveId = moveId };
        var assignResponse2 = await _client.PostAsJsonAsync($"/api/my-pokemons/{myPokemon.Id}/moves", assignRequest2);
        Assert.Equal(HttpStatusCode.BadRequest, assignResponse2.StatusCode);
    }

    [Fact]
    public async Task AssignMove_ExceedingMax4Moves_ReturnsBadRequest()
    {
        var myPokemon = await CreateMyPokemonAsync("trainer-008", "OverloadLearner");
        Assert.NotNull(myPokemon);

        var movesResponse = await _client.GetAsync("/api/moves");
        var moves = await movesResponse.Content.ReadFromJsonAsync<List<MoveDto>>();
        Assert.NotNull(moves);
        Assert.True(moves!.Count >= 5, "Need at least 5 moves for this test");

        // Assign 4 moves successfully
        for (int i = 0; i < 4; i++)
        {
            var assignRequest = new AssignMoveRequest { MoveId = moves[i].Id };
            var assignResponse = await _client.PostAsJsonAsync($"/api/my-pokemons/{myPokemon!.Id}/moves", assignRequest);
            Assert.Equal(HttpStatusCode.Created, assignResponse.StatusCode);
        }

        // 5th move should fail
        var failRequest = new AssignMoveRequest { MoveId = moves[4].Id };
        var failResponse = await _client.PostAsJsonAsync($"/api/my-pokemons/{myPokemon.Id}/moves", failRequest);
        Assert.Equal(HttpStatusCode.BadRequest, failResponse.StatusCode);
    }

    [Fact]
    public async Task GetMovesByMyPokemonId_ReturnsAssignedMoves()
    {
        var myPokemon = await CreateMyPokemonAsync("trainer-009", "MoveQueryTest");
        Assert.NotNull(myPokemon);

        var movesResponse = await _client.GetAsync("/api/moves");
        var moves = await movesResponse.Content.ReadFromJsonAsync<List<MoveDto>>();
        Assert.NotNull(moves);
        Assert.NotEmpty(moves);

        // Assign 2 moves
        var moveIds = new[] { moves![0].Id, moves[1].Id };
        foreach (var moveId in moveIds)
        {
            var assignRequest = new AssignMoveRequest { MoveId = moveId };
            await _client.PostAsJsonAsync($"/api/my-pokemons/{myPokemon!.Id}/moves", assignRequest);
        }

        // Query assigned moves
        var queryResponse = await _client.GetAsync($"/api/my-pokemons/{myPokemon.Id}/moves");
        Assert.Equal(HttpStatusCode.OK, queryResponse.StatusCode);

        var assignedMoves = await queryResponse.Content.ReadFromJsonAsync<List<MyPokemonMoveDto>>();
        Assert.NotNull(assignedMoves);
        Assert.Equal(2, assignedMoves!.Count);
    }

    [Fact]
    public async Task RemoveMove_FromMyPokemon_ReturnsNoContent()
    {
        var myPokemon = await CreateMyPokemonAsync("trainer-010", "MoveRemovalTest");
        Assert.NotNull(myPokemon);

        var movesResponse = await _client.GetAsync("/api/moves");
        var moves = await movesResponse.Content.ReadFromJsonAsync<List<MoveDto>>();
        Assert.NotNull(moves);
        Assert.NotEmpty(moves);

        var moveId = moves![0].Id;

        // Assign move
        var assignRequest = new AssignMoveRequest { MoveId = moveId };
        await _client.PostAsJsonAsync($"/api/my-pokemons/{myPokemon!.Id}/moves", assignRequest);

        // Remove move
        var removeResponse = await _client.DeleteAsync($"/api/my-pokemons/{myPokemon.Id}/moves/{moveId}");
        Assert.Equal(HttpStatusCode.NoContent, removeResponse.StatusCode);

        // Verify it's gone
        var queryResponse = await _client.GetAsync($"/api/my-pokemons/{myPokemon.Id}/moves");
        var assignedMoves = await queryResponse.Content.ReadFromJsonAsync<List<MyPokemonMoveDto>>();
        Assert.NotNull(assignedMoves);
        Assert.Empty(assignedMoves!);
    }

    [Fact]
    public async Task RemoveMove_NotAssigned_ReturnsNotFound()
    {
        var myPokemon = await CreateMyPokemonAsync("trainer-011", "MoveNotFoundTest");
        Assert.NotNull(myPokemon);

        var removeResponse = await _client.DeleteAsync($"/api/my-pokemons/{myPokemon!.Id}/moves/99999");
        Assert.Equal(HttpStatusCode.NotFound, removeResponse.StatusCode);
    }

    private async Task<MyPokemonDto?> CreateMyPokemonAsync(string trainerId, string name)
    {
        var basePokemonsResponse = await _client.GetAsync("/api/base-pokemons");
        var basePokemons = await basePokemonsResponse.Content.ReadFromJsonAsync<List<BasePokemonDto>>();
        if (basePokemons == null || basePokemons.Count == 0)
            return null;

        var createRequest = new CreateMyPokemonRequest
        {
            OwnerId = trainerId,
            BasePokemonId = basePokemons[0].Id,
            Name = name
        };

        var createResponse = await _client.PostAsJsonAsync("/api/my-pokemons", createRequest);
        if (createResponse.StatusCode != HttpStatusCode.Created)
            return null;

        return await createResponse.Content.ReadFromJsonAsync<MyPokemonDto>();
    }

    public sealed class CreateMyPokemonRequest
    {
        public string OwnerId { get; set; } = string.Empty;
        public int BasePokemonId { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public sealed class UpdateMyPokemonRequest
    {
        public string Name { get; set; } = string.Empty;
        public int Level { get; set; }
        public int CurrentHP { get; set; }
        public int TotalHP { get; set; }
        public int BaseAttack { get; set; }
        public int BaseDefense { get; set; }
        public int BaseSpecialAttack { get; set; }
        public int BaseSpecialDefense { get; set; }
        public int BaseSpeed { get; set; }
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
        public string Type { get; set; } = string.Empty;
        public int Level { get; set; }
        public int CurrentHP { get; set; }
        public int TotalHP { get; set; }
        public int BaseAttack { get; set; }
        public int BaseDefense { get; set; }
        public int BaseSpecialAttack { get; set; }
        public int BaseSpecialDefense { get; set; }
        public int BaseSpeed { get; set; }
        public List<MyPokemonMoveDto> Moves { get; set; } = [];
    }

    public sealed class MyPokemonMoveDto
    {
        public int Id { get; set; }
        public int MyPokemonId { get; set; }
        public int MoveId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Power { get; set; }
    }

    public sealed class MoveDt
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Power { get; set; }
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

    public sealed class MoveDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Power { get; set; }
    }
}
