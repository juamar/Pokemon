using System.Net;
using System.Net.Http.Json;

namespace Pokemons.IntegrationTests;

public class MovesCrudTests(PokedexApiFactory factory) : IClassFixture<PokedexApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetAll_ReturnsOkWithSeededData()
    {
        var response = await _client.GetAsync("/api/moves");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var moves = await response.Content.ReadFromJsonAsync<List<MoveDto>>();
        Assert.NotNull(moves);
        Assert.NotEmpty(moves);
    }

    [Fact]
    public async Task Create_ThenGetById_ReturnsCreatedMove()
    {
        var createRequest = CreateRequest($"CreatedMove-{Guid.NewGuid():N}");

        var createResponse = await _client.PostAsJsonAsync("/api/moves", createRequest);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<MoveDto>();
        Assert.NotNull(created);

        var getResponse = await _client.GetAsync($"/api/moves/{created!.Id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var fetched = await getResponse.Content.ReadFromJsonAsync<MoveDto>();
        Assert.NotNull(fetched);
        Assert.Equal(createRequest.Name, fetched!.Name);
        Assert.Equal(createRequest.Type, fetched.Type);
        Assert.Equal(createRequest.Power, fetched.Power);
    }

    [Fact]
    public async Task Update_ExistingMove_ReturnsUpdatedEntity()
    {
        var createRequest = CreateRequest($"UpdateMove-{Guid.NewGuid():N}");
        var createResponse = await _client.PostAsJsonAsync("/api/moves", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<MoveDto>();
        Assert.NotNull(created);

        var updateRequest = CreateRequest($"UpdatedMove-{Guid.NewGuid():N}", type: "Fuego", power: 95);

        var updateResponse = await _client.PutAsJsonAsync($"/api/moves/{created!.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updated = await updateResponse.Content.ReadFromJsonAsync<MoveDto>();
        Assert.NotNull(updated);
        Assert.Equal(updateRequest.Name, updated!.Name);
        Assert.Equal(updateRequest.Type, updated.Type);
        Assert.Equal(updateRequest.Power, updated.Power);
    }

    [Fact]
    public async Task Delete_ExistingMove_ReturnsNoContent_AndThenNotFound()
    {
        var createRequest = CreateRequest($"DeleteMove-{Guid.NewGuid():N}");
        var createResponse = await _client.PostAsJsonAsync("/api/moves", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<MoveDto>();
        Assert.NotNull(created);

        var deleteResponse = await _client.DeleteAsync($"/api/moves/{created!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getDeletedResponse = await _client.GetAsync($"/api/moves/{created.Id}");

        Assert.Equal(HttpStatusCode.NotFound, getDeletedResponse.StatusCode);
    }

    private static MoveRequest CreateRequest(string name, string type = "Agua", int power = 40)
    {
        return new MoveRequest
        {
            Name = name,
            Type = type,
            Power = power
        };
    }

    public sealed class MoveRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Power { get; set; }
    }

    public sealed class MoveDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Power { get; set; }
    }
}
