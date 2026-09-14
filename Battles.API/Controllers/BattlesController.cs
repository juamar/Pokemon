using Microsoft.AspNetCore.Mvc;
using Pokemons.Domain;
using Pokemons.Domain.Entities;
using Pokemons.Domain.Repositories;

namespace Battles.API.Controllers;

[ApiController]
[Route("api/battles")]
public class BattlesController(
    IBattleRepository battleRepository,
    IMyPokemonRepository myPokemonRepository,
    DamageCalculator damageCalculator,
    ISaveChanges saveChanges) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<BattleCreatedResponse>> Create(CreateBattleRequest request, CancellationToken cancellationToken)
    {
        if (request.Pokemon1Id == request.Pokemon2Id)
        {
            return BadRequest("A battle requires two different MyPokemon.");
        }

        var pokemon1 = await myPokemonRepository.GetByIdAsync(request.Pokemon1Id, cancellationToken);
        if (pokemon1 is null)
        {
            return BadRequest("Pokemon1 does not exist.");
        }

        var pokemon2 = await myPokemonRepository.GetByIdAsync(request.Pokemon2Id, cancellationToken);
        if (pokemon2 is null)
        {
            return BadRequest("Pokemon2 does not exist.");
        }

        if (pokemon1.Moves.Count == 0)
        {
            return BadRequest("Pokemon1 must have at least one assigned move before starting a battle.");
        }

        if (pokemon2.Moves.Count == 0)
        {
            return BadRequest("Pokemon2 must have at least one assigned move before starting a battle.");
        }

        var battle = new Battle
        {
            Pokemon1Id = request.Pokemon1Id,
            Pokemon2Id = request.Pokemon2Id,
            Status = BattleStatus.Started,
            CurrentTurnPokemonId = request.Pokemon1Id,
            StartedAt = DateTime.UtcNow
        };

        battleRepository.Add(battle);
        await saveChanges.SaveChangesAsync(cancellationToken);

        var response = new BattleCreatedResponse
        {
            Id = battle.Id,
            Status = battle.Status,
            Pokemon1Id = battle.Pokemon1Id,
            Pokemon2Id = battle.Pokemon2Id,
            CurrentTurnPokemonId = battle.CurrentTurnPokemonId,
            StartedAt = battle.StartedAt
        };

        return CreatedAtAction(nameof(GetById), new { id = battle.Id }, response);
    }

    [HttpPost("{id:int}/actions")]
    public async Task<ActionResult<BattleActionExecutionResponse>> ExecuteAction(int id, ExecuteBattleActionRequest request, CancellationToken cancellationToken)
    {
        var battle = await battleRepository.GetByIdAsync(id, cancellationToken);
        if (battle is null)
        {
            return NotFound();
        }

        if (battle.Status == BattleStatus.Finished)
        {
            return BadRequest("Battle is already finished.");
        }

        if (request.ActingPokemonId != battle.CurrentTurnPokemonId)
        {
            return BadRequest("It is not this Pokemon's turn.");
        }

        if (request.ActingPokemonId != battle.Pokemon1Id && request.ActingPokemonId != battle.Pokemon2Id)
        {
            return BadRequest("Acting Pokemon is not part of this battle.");
        }

        var defenderPokemonId = request.ActingPokemonId == battle.Pokemon1Id ? battle.Pokemon2Id : battle.Pokemon1Id;

        var actingPokemon = await myPokemonRepository.GetByIdAsync(request.ActingPokemonId, cancellationToken);
        var defendingPokemon = await myPokemonRepository.GetByIdAsync(defenderPokemonId, cancellationToken);

        if (actingPokemon is null || defendingPokemon is null)
        {
            return BadRequest("Battle participants could not be loaded.");
        }

        var selectedMove = actingPokemon.Moves.FirstOrDefault(m => m.MoveId == request.MoveId);
        if (selectedMove is null)
        {
            return BadRequest("Selected move is not assigned to acting Pokemon.");
        }

        var attackerBaseProjection = ToBasePokemon(actingPokemon);
        var defenderBaseProjection = ToBasePokemon(defendingPokemon);

        var damage = await damageCalculator.CalculateAsync(
            attackerBaseProjection,
            new Move
            {
                Id = selectedMove.MoveId,
                Name = selectedMove.Name,
                Type = selectedMove.Type,
                Power = selectedMove.Power
            },
            defenderBaseProjection,
            cancellationToken);

        defendingPokemon.CurrentHP = Math.Max(defendingPokemon.CurrentHP - damage, 0);

        var action = new BattleAction
        {
            BattleId = battle.Id,
            ActingPokemonId = actingPokemon.Id,
            MoveId = selectedMove.MoveId,
            DamageDealt = damage,
            TurnNumber = battle.Actions.Count + 1,
            ExecutedAt = DateTime.UtcNow
        };

        battleRepository.AddAction(action);

        if (defendingPokemon.CurrentHP == 0)
        {
            battle.Status = BattleStatus.Finished;
            battle.WinnerPokemonId = actingPokemon.Id;
            battle.FinishedAt = DateTime.UtcNow;
        }
        else
        {
            battle.CurrentTurnPokemonId = defendingPokemon.Id;
        }

        battleRepository.UpdateStatus(battle);
        await saveChanges.SaveChangesAsync(cancellationToken);

        return Ok(new BattleActionExecutionResponse
        {
            BattleId = battle.Id,
            TurnNumber = action.TurnNumber,
            ActingPokemonId = actingPokemon.Id,
            MoveId = selectedMove.MoveId,
            DamageDealt = damage,
            DefenderPokemonId = defendingPokemon.Id,
            DefenderRemainingHP = defendingPokemon.CurrentHP,
            BattleStatus = battle.Status,
            NextTurnPokemonId = battle.Status == BattleStatus.Started ? battle.CurrentTurnPokemonId : null,
            WinnerPokemonId = battle.WinnerPokemonId,
            FinishedAt = battle.FinishedAt
        });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BattleStateResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var battle = await battleRepository.GetByIdAsync(id, cancellationToken);
        if (battle is null)
        {
            return NotFound();
        }

        return Ok(MapBattleState(battle));
    }

    private static BasePokemon ToBasePokemon(MyPokemon myPokemon)
    {
        return new BasePokemon
        {
            Id = myPokemon.BasePokemonId,
            Name = myPokemon.Name,
            Type = myPokemon.Type,
            Level = myPokemon.Level,
            TotalHP = myPokemon.TotalHP,
            BaseAttack = myPokemon.BaseAttack,
            BaseDefense = myPokemon.BaseDefense,
            BaseSpecialAttack = myPokemon.BaseSpecialAttack,
            BaseSpecialDefense = myPokemon.BaseSpecialDefense,
            BaseSpeed = myPokemon.BaseSpeed
        };
    }

    private static BattleStateResponse MapBattleState(Battle battle)
    {
        return new BattleStateResponse
        {
            Id = battle.Id,
            Status = battle.Status,
            Pokemon1Id = battle.Pokemon1Id,
            Pokemon2Id = battle.Pokemon2Id,
            CurrentTurnPokemonId = battle.CurrentTurnPokemonId,
            WinnerPokemonId = battle.WinnerPokemonId,
            StartedAt = battle.StartedAt,
            FinishedAt = battle.FinishedAt,
            Pokemon1 = battle.Pokemon1 is null ? null : new BattlePokemonResponse
            {
                Id = battle.Pokemon1.Id,
                Name = battle.Pokemon1.Name,
                CurrentHP = battle.Pokemon1.CurrentHP,
                TotalHP = battle.Pokemon1.TotalHP
            },
            Pokemon2 = battle.Pokemon2 is null ? null : new BattlePokemonResponse
            {
                Id = battle.Pokemon2.Id,
                Name = battle.Pokemon2.Name,
                CurrentHP = battle.Pokemon2.CurrentHP,
                TotalHP = battle.Pokemon2.TotalHP
            },
            Actions = battle.Actions
                .OrderBy(a => a.TurnNumber)
                .Select(a => new BattleActionResponse
                {
                    Id = a.Id,
                    TurnNumber = a.TurnNumber,
                    ActingPokemonId = a.ActingPokemonId,
                    MoveId = a.MoveId,
                    DamageDealt = a.DamageDealt,
                    ExecutedAt = a.ExecutedAt
                })
                .ToList()
        };
    }

    public sealed class CreateBattleRequest
    {
        public int Pokemon1Id { get; set; }
        public int Pokemon2Id { get; set; }
    }

    public sealed class ExecuteBattleActionRequest
    {
        public int ActingPokemonId { get; set; }
        public int MoveId { get; set; }
    }

    public sealed class BattleCreatedResponse
    {
        public int Id { get; set; }
        public BattleStatus Status { get; set; }
        public int Pokemon1Id { get; set; }
        public int Pokemon2Id { get; set; }
        public int CurrentTurnPokemonId { get; set; }
        public DateTime StartedAt { get; set; }
    }

    public sealed class BattleActionExecutionResponse
    {
        public int BattleId { get; set; }
        public int TurnNumber { get; set; }
        public int ActingPokemonId { get; set; }
        public int MoveId { get; set; }
        public int DamageDealt { get; set; }
        public int DefenderPokemonId { get; set; }
        public int DefenderRemainingHP { get; set; }
        public BattleStatus BattleStatus { get; set; }
        public int? NextTurnPokemonId { get; set; }
        public int? WinnerPokemonId { get; set; }
        public DateTime? FinishedAt { get; set; }
    }

    public sealed class BattleStateResponse
    {
        public int Id { get; set; }
        public BattleStatus Status { get; set; }
        public int Pokemon1Id { get; set; }
        public int Pokemon2Id { get; set; }
        public int CurrentTurnPokemonId { get; set; }
        public int? WinnerPokemonId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public BattlePokemonResponse? Pokemon1 { get; set; }
        public BattlePokemonResponse? Pokemon2 { get; set; }
        public List<BattleActionResponse> Actions { get; set; } = [];
    }

    public sealed class BattlePokemonResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CurrentHP { get; set; }
        public int TotalHP { get; set; }
    }

    public sealed class BattleActionResponse
    {
        public int Id { get; set; }
        public int TurnNumber { get; set; }
        public int ActingPokemonId { get; set; }
        public int MoveId { get; set; }
        public int DamageDealt { get; set; }
        public DateTime ExecutedAt { get; set; }
    }
}
