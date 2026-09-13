using Microsoft.AspNetCore.Mvc;
using Pokemons.Domain.Entities;
using Pokemons.Domain.Repositories;

namespace Pokedex.API.Controllers;

[ApiController]
[Route("api/moves")]
public class MovesController(IMoveRepository moveRepository, ISaveChanges saveChanges) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Move>>> GetAll(CancellationToken cancellationToken)
    {
        var moves = await moveRepository.GetAllAsync(cancellationToken);
        return Ok(moves);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Move>> GetById(int id, CancellationToken cancellationToken)
    {
        var move = await moveRepository.GetByIdAsync(id, cancellationToken);

        if (move is null)
        {
            return NotFound();
        }

        return Ok(move);
    }

    [HttpPost]
    public async Task<ActionResult<Move>> Create(CreateMoveRequest request, CancellationToken cancellationToken)
    {
        var move = new Move
        {
            Name = request.Name,
            Type = request.Type,
            Power = request.Power
        };

        moveRepository.Add(move);
        await saveChanges.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = move.Id }, move);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Move>> Update(int id, UpdateMoveRequest request, CancellationToken cancellationToken)
    {
        var move = await moveRepository.GetByIdAsync(id, cancellationToken);

        if (move is null)
        {
            return NotFound();
        }

        move.Name = request.Name;
        move.Type = request.Type;
        move.Power = request.Power;

        moveRepository.Update(move);
        await saveChanges.SaveChangesAsync(cancellationToken);

        return Ok(move);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var move = await moveRepository.GetByIdAsync(id, cancellationToken);

        if (move is null)
        {
            return NotFound();
        }

        moveRepository.Remove(move);
        await saveChanges.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    public sealed class CreateMoveRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Power { get; set; }
    }

    public sealed class UpdateMoveRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Power { get; set; }
    }
}
