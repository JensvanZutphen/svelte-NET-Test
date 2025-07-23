using Microsoft.AspNetCore.Mvc;
using MySvelteApp.Server.Models;
using System.Text.Json;

namespace MySvelteApp.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class RandomPokemonController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly string _pokemonApiUrl;

    public RandomPokemonController(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _pokemonApiUrl = "https://pokeapi.co/api/v2/pokemon/";
    }

    private async Task<int> GetPokemonCount()
    {
        var response = await _httpClient.GetAsync("https://pokeapi.co/api/v2/pokemon-species/?limit=0");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        return doc.RootElement.GetProperty("count").GetInt32();
    }

    private class PokeApiType
    {
        public TypeInfo type { get; set; } = default!;
        public class TypeInfo { public string name { get; set; } = string.Empty; }
    }
    private class PokeApiResponse
    {
        public string name { get; set; } = string.Empty;
        public List<PokeApiType> types { get; set; } = new();
        public Sprites sprites { get; set; } = new();
        public class Sprites { public string? front_default { get; set; } }
    }

    private async Task<RandomPokemon> GetRandomPokemon()
    {
        int count = await GetPokemonCount();
        var random = new Random();
        var randomPokemon = random.Next(1, count + 1);
        var pokemonUrl = $"{_pokemonApiUrl}{randomPokemon}";
        var pokemonResponse = await _httpClient.GetAsync(pokemonUrl);
        pokemonResponse.EnsureSuccessStatusCode();
        var pokemonContent = await pokemonResponse.Content.ReadAsStringAsync();
        var pokeApi = JsonSerializer.Deserialize<PokeApiResponse>(pokemonContent);
        if (pokeApi == null)
            throw new Exception("Failed to deserialize Pokemon data.");
        return new RandomPokemon
        {
            Name = pokeApi.name,
            Type = string.Join(", ", pokeApi.types.Select(t => t.type.name)),
            Image = pokeApi.sprites.front_default
        };
    }

    [HttpGet]
    public async Task<RandomPokemon> Get()
    {
        return await GetRandomPokemon();
    }
}
