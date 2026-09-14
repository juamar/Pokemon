using Microsoft.AspNetCore.Mvc;
using Pokemons.Domain.Entities;
using Pokemons.Domain.Repositories;

namespace Pokedex.API.Controllers;

[ApiController]
[Route("api/my-pokemons")]
public class MyPokemonsController(
    IMyPokemonRepository myPokemonRepository,
    IBasePokemonRepository basePokemonRepository,
    IMoveRepository moveRepository,
    ISaveChanges saveChanges) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<MyPokemon>>> GetAll(CancellationToken cancellationToken)
    {
        var myPokemons = await myPokemonRepository.GetAllAsync(cancellationToken);
        return Ok(myPokemons);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MyPokemon>> GetById(int id, CancellationToken cancellationToken)
    {
        var myPokemon = await myPokemonRepository.GetByIdAsync(id, cancellationToken);

        if (myPokemon is null)
        {
            return NotFound();
        }

        return Ok(myPokemon);
    }

    [HttpPost]
    public async Task<ActionResult<MyPokemon>> Create(CreateMyPokemonRequest request, CancellationToken cancellationToken)
    {
        // Validate that the base Pokemon exists
        var basePokemon = await basePokemonRepository.GetByIdAsync(request.BasePokemonId, cancellationToken);
        if (basePokemon is null)
        {
            return BadRequest("The specified BasePokemon does not exist.");
        }

        var myPokemon = new MyPokemon
        {
            OwnerId = request.OwnerId,
            BasePokemonId = request.BasePokemonId,
            Name = request.Name,
            Type = basePokemon.Type,
            Level = basePokemon.Level,
            CurrentHP = basePokemon.TotalHP,
            TotalHP = basePokemon.TotalHP,
            BaseAttack = basePokemon.BaseAttack,
            BaseDefense = basePokemon.BaseDefense,
            BaseSpecialAttack = basePokemon.BaseSpecialAttack,
            BaseSpecialDefense = basePokemon.BaseSpecialDefense,
            BaseSpeed = basePokemon.BaseSpeed
        };

        myPokemonRepository.Add(myPokemon);
        await saveChanges.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = myPokemon.Id }, myPokemon);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<MyPokemon>> Update(int id, UpdateMyPokemonRequest request, CancellationToken cancellationToken)
    {
        var myPokemon = await myPokemonRepository.GetByIdAsync(id, cancellationToken);

        if (myPokemon is null)
        {
            return NotFound();
        }

        myPokemon.Name = request.Name;
        myPokemon.Level = request.Level;
        myPokemon.CurrentHP = request.CurrentHP;
        myPokemon.TotalHP = request.TotalHP;
        myPokemon.BaseAttack = request.BaseAttack;
        myPokemon.BaseDefense = request.BaseDefense;
        myPokemon.BaseSpecialAttack = request.BaseSpecialAttack;
        myPokemon.BaseSpecialDefense = request.BaseSpecialDefense;
        myPokemon.BaseSpeed = request.BaseSpeed;

        myPokemonRepository.Update(myPokemon);
        await saveChanges.SaveChangesAsync(cancellationToken);

        return Ok(myPokemon);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var myPokemon = await myPokemonRepository.GetByIdAsync(id, cancellationToken);

        if (myPokemon is null)
        {
            return NotFound();
        }

        myPokemonRepository.Remove(myPokemon);
        await saveChanges.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpPost("{id:int}/moves")]
    public async Task<ActionResult<MyPokemonMove>> AssignMove(int id, AssignMoveRequest request, CancellationToken cancellationToken)
    {
        var myPokemon = await myPokemonRepository.GetByIdAsync(id, cancellationToken);

        if (myPokemon is null)
        {
            return NotFound("MyPokemon not found.");
        }

        // Check if move already exists
        var hasMoveKey = await myPokemonRepository.HasMoveAsync(id, request.MoveId, cancellationToken);
        if (hasMoveKey)
        {
            return BadRequest("This move is already assigned to this Pokemon.");
        }

        // Check max 4 moves constraint
        var moveCount = await myPokemonRepository.GetMoveCountAsync(id, cancellationToken);
        if (moveCount >= 4)
        {
            return BadRequest("Cannot assign more than 4 moves to a Pokemon.");
        }

        // Validate that the move exists
        var move = await moveRepository.GetByIdAsync(request.MoveId, cancellationToken);
        if (move is null)
        {
            return BadRequest("The specified Move does not exist.");
        }

        myPokemonRepository.AddMove(id, move.Id, move.Name, move.Power, move.Type);
        await saveChanges.SaveChangesAsync(cancellationToken);

        var myPokemonMove = new MyPokemonMove
        {
            MyPokemonId = id,
            MoveId = move.Id,
            Name = move.Name,
            Type = move.Type,
            Power = move.Power
        };

        return CreatedAtAction(nameof(GetMovesByMyPokemonId), new { id }, myPokemonMove);
    }

    [HttpGet("{id:int}/moves")]
    public async Task<ActionResult<List<MyPokemonMoveSummaryResponse>>> GetMovesByMyPokemonId(int id, CancellationToken cancellationToken)
    {
        var myPokemon = await myPokemonRepository.GetByIdAsync(id, cancellationToken);

        if (myPokemon is null)
        {
            return NotFound("MyPokemon not found.");
        }

        var moves = myPokemon.Moves
            .Select(m => new MyPokemonMoveSummaryResponse
            {
                MoveId = m.MoveId,
                Name = m.Name,
                Type = m.Type,
                Power = m.Power
            })
            .ToList();

        return Ok(moves);
    }

    [HttpDelete("{id:int}/moves/{moveId:int}")]
    public async Task<IActionResult> RemoveMove(int id, int moveId, CancellationToken cancellationToken)
    {
        var myPokemon = await myPokemonRepository.GetByIdAsync(id, cancellationToken);

        if (myPokemon is null)
        {
            return NotFound("MyPokemon not found.");
        }

        var myPokemonMove = myPokemon.Moves.FirstOrDefault(m => m.MoveId == moveId);
        if (myPokemonMove is null)
        {
            return NotFound("Move not found on this Pokemon.");
        }

        myPokemonRepository.RemoveMove(id, moveId);
        await saveChanges.SaveChangesAsync(cancellationToken);

        return NoContent();
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

    public sealed class MyPokemonMoveSummaryResponse
    {
        public int MoveId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Power { get; set; }
    }
}
