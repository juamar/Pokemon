using Microsoft.AspNetCore.Mvc;
using Pokemons.Domain.Entities;
using Pokemons.Domain.Repositories;

namespace Pokedex.API.Controllers;

[ApiController]
[Route("api/base-pokemons")]
public class BasePokemonsController(IBasePokemonRepository basePokemonRepository, ISaveChanges saveChanges) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<BasePokemon>>> GetAll(CancellationToken cancellationToken)
    {
        var basePokemons = await basePokemonRepository.GetAllAsync(cancellationToken);
        return Ok(basePokemons);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BasePokemon>> GetById(int id, CancellationToken cancellationToken)
    {
        var basePokemon = await basePokemonRepository.GetByIdAsync(id, cancellationToken);

        if (basePokemon is null)
        {
            return NotFound();
        }

        return Ok(basePokemon);
    }

    [HttpPost]
    public async Task<ActionResult<BasePokemon>> Create(CreateBasePokemonRequest request, CancellationToken cancellationToken)
    {
        var basePokemon = new BasePokemon
        {
            Name = request.Name,
            Type = request.Type,
            Level = request.Level,
            TotalHP = request.TotalHP,
            BaseAttack = request.BaseAttack,
            BaseDefense = request.BaseDefense,
            BaseSpecialAttack = request.BaseSpecialAttack,
            BaseSpecialDefense = request.BaseSpecialDefense,
            BaseSpeed = request.BaseSpeed
        };

        basePokemonRepository.Add(basePokemon);
        await saveChanges.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = basePokemon.Id }, basePokemon);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<BasePokemon>> Update(int id, UpdateBasePokemonRequest request, CancellationToken cancellationToken)
    {
        var basePokemon = await basePokemonRepository.GetByIdAsync(id, cancellationToken);

        if (basePokemon is null)
        {
            return NotFound();
        }

        basePokemon.Name = request.Name;
        basePokemon.Type = request.Type;
        basePokemon.Level = request.Level;
        basePokemon.TotalHP = request.TotalHP;
        basePokemon.BaseAttack = request.BaseAttack;
        basePokemon.BaseDefense = request.BaseDefense;
        basePokemon.BaseSpecialAttack = request.BaseSpecialAttack;
        basePokemon.BaseSpecialDefense = request.BaseSpecialDefense;
        basePokemon.BaseSpeed = request.BaseSpeed;

        basePokemonRepository.Update(basePokemon);
        await saveChanges.SaveChangesAsync(cancellationToken);

        return Ok(basePokemon);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var basePokemon = await basePokemonRepository.GetByIdAsync(id, cancellationToken);

        if (basePokemon is null)
        {
            return NotFound();
        }

        basePokemonRepository.Remove(basePokemon);
        await saveChanges.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpGet("{id:int}/possible-moves")]
    public async Task<ActionResult<List<Move>>> GetPossibleMoves(int id, CancellationToken cancellationToken)
    {
        var basePokemon = await basePokemonRepository.GetByIdAsync(id, cancellationToken);

        if (basePokemon is null)
        {
            return NotFound();
        }

        var possibleMoves = await basePokemonRepository.GetPossibleMovesAsync(id, cancellationToken);
        return Ok(possibleMoves);
    }

    [HttpGet("by-move/{moveId:int}")]
    public async Task<ActionResult<List<BasePokemon>>> GetByMove(int moveId, CancellationToken cancellationToken)
    {
        var basePokemons = await basePokemonRepository.GetByMoveAsync(moveId, cancellationToken);
        return Ok(basePokemons);
    }

    public sealed class CreateBasePokemonRequest
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

    public sealed class UpdateBasePokemonRequest
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
}
