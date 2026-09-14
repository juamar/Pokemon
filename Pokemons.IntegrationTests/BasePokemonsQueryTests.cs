using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Pokemons.IntegrationTests;

public class BasePokemonsQueryTests(PokedexApiFactory factory) : IClassFixture<PokedexApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetPossibleMoves_WithValidBasePokemonId_ReturnsMovesList()
    {
        // Get a base pokemon from seeded data
        var basePokemonsResponse = await _client.GetAsync("/api/base-pokemons");
        var basePokemons = await basePokemonsResponse.Content.ReadFromJsonAsync<List<BasePokemonDto>>();
        Assert.NotNull(basePokemons);
        Assert.NotEmpty(basePokemons);

        var basePokemon = basePokemons![0];

        var possibleMovesResponse = await _client.GetAsync($"/api/base-pokemons/{basePokemon.Id}/possible-moves");

        Assert.Equal(HttpStatusCode.OK, possibleMovesResponse.StatusCode);

        var possibleMoves = await possibleMovesResponse.Content.ReadFromJsonAsync<List<MoveDto>>();
        Assert.NotNull(possibleMoves);

        // All possible moves should share the base Pokemon's type
        foreach (var move in possibleMoves!)
        {
            Assert.Equal(basePokemon.Type, move.Type);
        }
    }

    [Fact]
    public async Task GetPossibleMoves_WithInvalidBasePokemonId_ReturnsNotFound()
    {
        var possibleMovesResponse = await _client.GetAsync("/api/base-pokemons/99999/possible-moves");

        Assert.Equal(HttpStatusCode.NotFound, possibleMovesResponse.StatusCode);
    }

    [Fact]
    public async Task GetByMove_WithValidMoveId_ReturnsBasePokemonsList()
    {
        // Get a move from seeded data
        var movesResponse = await _client.GetAsync("/api/moves");
        var moves = await movesResponse.Content.ReadFromJsonAsync<List<MoveDto>>();
        Assert.NotNull(moves);
        Assert.NotEmpty(moves);

        var move = moves![0];

        var byMoveResponse = await _client.GetAsync($"/api/base-pokemons/by-move/{move.Id}");

        Assert.Equal(HttpStatusCode.OK, byMoveResponse.StatusCode);

        var basePokemons = await byMoveResponse.Content.ReadFromJsonAsync<List<BasePokemonDto>>();
        Assert.NotNull(basePokemons);

        // All returned Pokemon should share the move's type
        foreach (var pokemon in basePokemons!)
        {
            Assert.Equal(move.Type, pokemon.Type);
        }
    }

    [Fact]
    public async Task GetByMove_WithInvalidMoveId_ReturnsEmptyList()
    {
        var byMoveResponse = await _client.GetAsync("/api/base-pokemons/by-move/99999");

        Assert.Equal(HttpStatusCode.OK, byMoveResponse.StatusCode);

        var basePokemons = await byMoveResponse.Content.ReadFromJsonAsync<List<BasePokemonDto>>();
        Assert.NotNull(basePokemons);
        Assert.Empty(basePokemons!);
    }

    [Fact]
    public async Task GetByMove_ReturnsAllPokemonWithSameType()
    {
        // Get all moves and base Pokemon
        var movesResponse = await _client.GetAsync("/api/moves");
        var moves = await movesResponse.Content.ReadFromJsonAsync<List<MoveDto>>();
        Assert.NotNull(moves);

        var basePokemonsResponse = await _client.GetAsync("/api/base-pokemons");
        var basePokemons = await basePokemonsResponse.Content.ReadFromJsonAsync<List<BasePokemonDto>>();
        Assert.NotNull(basePokemons);

        if (moves!.Count == 0 || basePokemons!.Count == 0)
            return;

        var testMove = moves[0];

        var byMoveResponse = await _client.GetAsync($"/api/base-pokemons/by-move/{testMove.Id}");
        var resultPokemons = await byMoveResponse.Content.ReadFromJsonAsync<List<BasePokemonDto>>();
        Assert.NotNull(resultPokemons);

        // Count how many seeded Pokemon share the move's type
        var expectedPokemonCount = basePokemons.Count(p => p.Type == testMove.Type);

        // Result should have all Pokemon of that type
        Assert.Equal(expectedPokemonCount, resultPokemons!.Count);
    }

    [Fact]
    public async Task PossibleMoves_ConsistentWithTypeSystem()
    {
        // Verify that possible moves are consistent across all Pokemon of same type
        var basePokemonsResponse = await _client.GetAsync("/api/base-pokemons");
        var basePokemons = await basePokemonsResponse.Content.ReadFromJsonAsync<List<BasePokemonDto>>();
        Assert.NotNull(basePokemons);
        Assert.NotEmpty(basePokemons);

        // Get possible moves for first Pokemon
        var pokemon1 = basePokemons![0];
        var moves1Response = await _client.GetAsync($"/api/base-pokemons/{pokemon1.Id}/possible-moves");
        var moves1 = await moves1Response.Content.ReadFromJsonAsync<List<MoveDto>>();
        Assert.NotNull(moves1);

        // Find another Pokemon with same type (if exists)
        var pokemon2 = basePokemons.FirstOrDefault(p => p.Type == pokemon1.Type && p.Id != pokemon1.Id);
        if (pokemon2 != null)
        {
            var moves2Response = await _client.GetAsync($"/api/base-pokemons/{pokemon2.Id}/possible-moves");
            var moves2 = await moves2Response.Content.ReadFromJsonAsync<List<MoveDto>>();
            Assert.NotNull(moves2);

            // Both should have the same possible moves
            Assert.Equal(moves1!.Count, moves2!.Count);
            var moveIds1 = moves1.Select(m => m.Id).OrderBy(id => id).ToList();
            var moveIds2 = moves2.Select(m => m.Id).OrderBy(id => id).ToList();
            Assert.Equal(moveIds1, moveIds2);
        }
    }

    public sealed class MoveDto
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
}
