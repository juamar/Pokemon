using Pokemons.Domain.Entities;

namespace Pokemons.Infra.Configurations;

/// <summary>
/// V1 seed data: 6 BasePokemon - National Dex #0004 Charmander, #0007 Squirtle, #0035 Clefairy,
/// #0019 Rattata, #0023 Ekans, #0025 Pikachu (per requirements.md §5 assumption 11), sourced from
/// https://pokemondb.net/pokedex/all. Moves are their "Moves learnt by level up" attacking moves
/// (per-Pokemon page, e.g. https://pokemondb.net/pokedex/charmander#dex-evolution), and the full
/// 18x18 type effectiveness matrix from requirements.md §1.
/// </summary>
public static class SeedData
{
    private static readonly string[] Types =
    [
        "Acero", "Agua", "Bicho", "Dragon", "Electrico", "Fantasma", "Fuego", "Hada", "Hielo",
        "Lucha", "Normal", "Planta", "Psiquico", "Roca", "Siniestro", "Tierra", "Veneno", "Volador"
    ];

    // Only non-default (non x1) cells from the requirements.md §1 matrix. Row = attacker, Column = defender.
    private static readonly Dictionary<(string Attacker, string Defender), double> Overrides = new()
    {
        [("Acero", "Acero")] = 0.5,
        [("Acero", "Agua")] = 0.5,
        [("Acero", "Electrico")] = 0.5,
        [("Acero", "Fuego")] = 0.5,
        [("Acero", "Hada")] = 2,
        [("Acero", "Hielo")] = 2,
        [("Acero", "Roca")] = 2,

        [("Agua", "Agua")] = 0.5,
        [("Agua", "Dragon")] = 0.5,
        [("Agua", "Fuego")] = 2,
        [("Agua", "Planta")] = 0.5,
        [("Agua", "Roca")] = 2,
        [("Agua", "Tierra")] = 2,

        [("Bicho", "Acero")] = 0.5,
        [("Bicho", "Fantasma")] = 0.5,
        [("Bicho", "Fuego")] = 0.5,
        [("Bicho", "Hada")] = 0.5,
        [("Bicho", "Lucha")] = 0.5,
        [("Bicho", "Planta")] = 2,
        [("Bicho", "Psiquico")] = 2,
        [("Bicho", "Siniestro")] = 2,
        [("Bicho", "Veneno")] = 0.5,
        [("Bicho", "Volador")] = 0.5,

        [("Dragon", "Acero")] = 0.5,
        [("Dragon", "Dragon")] = 2,
        [("Dragon", "Hada")] = 0,

        [("Electrico", "Agua")] = 2,
        [("Electrico", "Dragon")] = 0.5,
        [("Electrico", "Electrico")] = 0.5,
        [("Electrico", "Planta")] = 0.5,
        [("Electrico", "Tierra")] = 0,
        [("Electrico", "Volador")] = 2,

        [("Fantasma", "Fantasma")] = 2,
        [("Fantasma", "Normal")] = 0,
        [("Fantasma", "Psiquico")] = 2,
        [("Fantasma", "Siniestro")] = 0.5,

        [("Fuego", "Acero")] = 2,
        [("Fuego", "Agua")] = 0.5,
        [("Fuego", "Bicho")] = 2,
        [("Fuego", "Dragon")] = 0.5,
        [("Fuego", "Fuego")] = 0.5,
        [("Fuego", "Hielo")] = 2,
        [("Fuego", "Planta")] = 2,
        [("Fuego", "Roca")] = 0.5,

        [("Hada", "Acero")] = 0.5,
        [("Hada", "Dragon")] = 2,
        [("Hada", "Fuego")] = 0.5,
        [("Hada", "Lucha")] = 2,
        [("Hada", "Siniestro")] = 2,
        [("Hada", "Veneno")] = 0.5,

        [("Hielo", "Acero")] = 0.5,
        [("Hielo", "Agua")] = 0.5,
        [("Hielo", "Dragon")] = 2,
        [("Hielo", "Fuego")] = 0.5,
        [("Hielo", "Hielo")] = 0.5,
        [("Hielo", "Planta")] = 2,
        [("Hielo", "Tierra")] = 2,
        [("Hielo", "Volador")] = 2,

        [("Lucha", "Acero")] = 2,
        [("Lucha", "Bicho")] = 0.5,
        [("Lucha", "Fantasma")] = 0,
        [("Lucha", "Hada")] = 0.5,
        [("Lucha", "Hielo")] = 2,
        [("Lucha", "Normal")] = 2,
        [("Lucha", "Psiquico")] = 0.5,
        [("Lucha", "Roca")] = 2,
        [("Lucha", "Siniestro")] = 2,
        [("Lucha", "Veneno")] = 0.5,
        [("Lucha", "Volador")] = 0.5,

        [("Normal", "Acero")] = 0.5,
        [("Normal", "Fantasma")] = 0,
        [("Normal", "Roca")] = 0.5,

        [("Planta", "Acero")] = 0.5,
        [("Planta", "Agua")] = 2,
        [("Planta", "Bicho")] = 0.5,
        [("Planta", "Dragon")] = 0.5,
        [("Planta", "Fuego")] = 0.5,
        [("Planta", "Planta")] = 0.5,
        [("Planta", "Roca")] = 2,
        [("Planta", "Tierra")] = 2,
        [("Planta", "Veneno")] = 0.5,
        [("Planta", "Volador")] = 0.5,

        [("Psiquico", "Acero")] = 0.5,
        [("Psiquico", "Lucha")] = 2,
        [("Psiquico", "Psiquico")] = 0.5,
        [("Psiquico", "Siniestro")] = 0,
        [("Psiquico", "Veneno")] = 2,

        [("Roca", "Acero")] = 0.5,
        [("Roca", "Bicho")] = 2,
        [("Roca", "Fuego")] = 2,
        [("Roca", "Hielo")] = 2,
        [("Roca", "Lucha")] = 0.5,
        [("Roca", "Tierra")] = 0.5,
        [("Roca", "Volador")] = 2,

        [("Siniestro", "Fantasma")] = 2,
        [("Siniestro", "Hada")] = 0.5,
        [("Siniestro", "Lucha")] = 0.5,
        [("Siniestro", "Psiquico")] = 2,
        [("Siniestro", "Siniestro")] = 0.5,

        [("Tierra", "Acero")] = 2,
        [("Tierra", "Bicho")] = 0.5,
        [("Tierra", "Electrico")] = 2,
        [("Tierra", "Fuego")] = 2,
        [("Tierra", "Planta")] = 0.5,
        [("Tierra", "Roca")] = 2,
        [("Tierra", "Veneno")] = 2,
        [("Tierra", "Volador")] = 0,

        [("Veneno", "Acero")] = 0,
        [("Veneno", "Fantasma")] = 0.5,
        [("Veneno", "Hada")] = 2,
        [("Veneno", "Planta")] = 2,
        [("Veneno", "Roca")] = 0.5,
        [("Veneno", "Tierra")] = 0.5,
        [("Veneno", "Veneno")] = 0.5,

        [("Volador", "Acero")] = 0.5,
        [("Volador", "Bicho")] = 2,
        [("Volador", "Electrico")] = 0.5,
        [("Volador", "Lucha")] = 2,
        [("Volador", "Planta")] = 2,
        [("Volador", "Roca")] = 0.5,
    };

    public static IEnumerable<TypeEffectiveness> TypeEffectivenesses
    {
        get
        {
            var id = 1;
            foreach (var attacker in Types)
            {
                foreach (var defender in Types)
                {
                    var multiplier = Overrides.TryGetValue((attacker, defender), out var value) ? value : 1;
                    yield return new TypeEffectiveness
                    {
                        Id = id++,
                        AttackerType = attacker,
                        DefenderType = defender,
                        Multiplier = multiplier
                    };
                }
            }
        }
    }

    // Source: https://pokemondb.net/pokedex/all. Caterpie (#0010) was swapped for Clefairy (#0035) - too few
    // attacking moves available for Caterpie to reach the 4-same-type-move minimum requested for the seed, and
    // Ekans already covers the Veneno type so a second Veneno Pokemon (Nidoran-F) would have been redundant.
    public static IEnumerable<BasePokemon> BasePokemons =>
    [
        new() { Id = 1, Name = "Charmander", Type = "Fuego", Level = 5, TotalHP = 39, BaseAttack = 52, BaseDefense = 43, BaseSpecialAttack = 60, BaseSpecialDefense = 50, BaseSpeed = 65 },
        new() { Id = 2, Name = "Squirtle", Type = "Agua", Level = 5, TotalHP = 44, BaseAttack = 48, BaseDefense = 65, BaseSpecialAttack = 50, BaseSpecialDefense = 64, BaseSpeed = 43 },
        new() { Id = 3, Name = "Clefairy", Type = "Hada", Level = 5, TotalHP = 70, BaseAttack = 45, BaseDefense = 48, BaseSpecialAttack = 60, BaseSpecialDefense = 65, BaseSpeed = 35 },
        new() { Id = 4, Name = "Rattata", Type = "Normal", Level = 5, TotalHP = 30, BaseAttack = 56, BaseDefense = 35, BaseSpecialAttack = 25, BaseSpecialDefense = 35, BaseSpeed = 72 },
        new() { Id = 5, Name = "Ekans", Type = "Veneno", Level = 5, TotalHP = 35, BaseAttack = 60, BaseDefense = 44, BaseSpecialAttack = 40, BaseSpecialDefense = 54, BaseSpeed = 55 },
        new() { Id = 6, Name = "Pikachu", Type = "Electrico", Level = 5, TotalHP = 35, BaseAttack = 55, BaseDefense = 40, BaseSpecialAttack = 50, BaseSpecialDefense = 50, BaseSpeed = 90 },
    ];

    // Source: "Moves learnt by level up" table on each Pokemon's pokemondb.net page (e.g. /pokedex/charmander#dex-evolution).
    // Charmander/Squirtle/Ekans/Pikachu use the Scarlet & Violet level-up list (latest generation on the site); Rattata
    // and Clefairy aren't obtainable in Scarlet & Violet so their Brilliant Diamond & Shining Pearl / Legends: Z-A list
    // is used instead. Only damage-dealing moves are seeded (with one documented exception, Endeavor - see below) -
    // V1's damage formula has no accuracy/status-effect support. Each Pokemon with more than 4 same-type attacking
    // moves available has its highest-power one intentionally left out of the seed so it can be added manually via
    // the Moves API as part of the smoke test. Where a Pokemon's level-up list includes a move already seeded for
    // another Pokemon (e.g. Clefairy's Tackle), the existing Move row/id is reused rather than duplicated.
    //
    // Held back (not seeded) for manual smoke-test addition:
    //   Charmander -> Inferno (Fuego, 100, Lv.36, S&V)
    //   Squirtle   -> Wave Crash (Agua, 120, Lv.36, S&V)
    //   Ekans      -> Gunk Shot (Veneno, 120, Lv.49, S&V)
    //   Pikachu    -> Thunder (Electrico, 110, Lv.44, S&V)
    //   Clefairy   -> Moonblast (Hada, 95, Lv.48, Legends: Z-A)
    public static IEnumerable<Move> Moves =>
    [
        // Charmander (Fuego) - at least 4 same-type moves per request
        new() { Id = 1, Name = "Scratch", Type = "Normal", Power = 40 },        // Lv.1
        new() { Id = 2, Name = "Ember", Type = "Fuego", Power = 40 },           // Lv.4
        new() { Id = 3, Name = "Dragon Breath", Type = "Dragon", Power = 60 },  // Lv.12
        new() { Id = 4, Name = "Fire Fang", Type = "Fuego", Power = 65 },       // Lv.17
        new() { Id = 5, Name = "Slash", Type = "Normal", Power = 70 },          // Lv.20
        new() { Id = 6, Name = "Flamethrower", Type = "Fuego", Power = 90 },    // Lv.24
        new() { Id = 7, Name = "Fire Spin", Type = "Fuego", Power = 35 },       // Lv.32
        new() { Id = 8, Name = "Flare Blitz", Type = "Fuego", Power = 120 },    // Lv.40

        // Squirtle (Agua)
        new() { Id = 9, Name = "Tackle", Type = "Normal", Power = 40 },         // Lv.1 (shared w/ Rattata)
        new() { Id = 10, Name = "Water Gun", Type = "Agua", Power = 40 },       // Lv.3
        new() { Id = 11, Name = "Rapid Spin", Type = "Normal", Power = 50 },    // Lv.9
        new() { Id = 12, Name = "Bite", Type = "Siniestro", Power = 60 },       // Lv.12 (shared w/ Rattata, Ekans)
        new() { Id = 13, Name = "Water Pulse", Type = "Agua", Power = 60 },     // Lv.15
        new() { Id = 14, Name = "Aqua Tail", Type = "Agua", Power = 90 },       // Lv.24
        new() { Id = 15, Name = "Hydro Pump", Type = "Agua", Power = 110 },     // Lv.33

        // Rattata (Normal)
        new() { Id = 16, Name = "Quick Attack", Type = "Normal", Power = 40 }, // BDSP Lv.4 (shared w/ Pikachu)
        new() { Id = 17, Name = "Take Down", Type = "Normal", Power = 90 },     // BDSP Lv.16
        new() { Id = 18, Name = "Crunch", Type = "Siniestro", Power = 80 },     // BDSP Lv.22
        new() { Id = 19, Name = "Double-Edge", Type = "Normal", Power = 120 },  // BDSP Lv.31
        // Endeavor has no fixed base power in-game (sets the target's HP to the user's current HP) - approximated
        // here as Power = 100 so it fits V1's fixed-power damage formula.
        new() { Id = 20, Name = "Endeavor", Type = "Normal", Power = 100 },     // BDSP Lv.34

        // Ekans (Veneno)
        new() { Id = 21, Name = "Wrap", Type = "Normal", Power = 15 },          // Lv.1
        new() { Id = 22, Name = "Poison Sting", Type = "Veneno", Power = 15 },  // Lv.4
        new() { Id = 23, Name = "Acid", Type = "Veneno", Power = 40 },          // Lv.20
        new() { Id = 24, Name = "Acid Spray", Type = "Veneno", Power = 40 },    // Lv.28
        new() { Id = 25, Name = "Sludge Bomb", Type = "Veneno", Power = 90 },   // Lv.33

        // Pikachu (Electrico)
        new() { Id = 26, Name = "Nuzzle", Type = "Electrico", Power = 20 },     // Lv.1
        new() { Id = 27, Name = "Thunder Shock", Type = "Electrico", Power = 40 }, // Lv.1
        new() { Id = 28, Name = "Spark", Type = "Electrico", Power = 65 },      // Lv.20
        new() { Id = 29, Name = "Iron Tail", Type = "Acero", Power = 100 },     // Lv.28
        new() { Id = 30, Name = "Discharge", Type = "Electrico", Power = 80 }, // Lv.32
        new() { Id = 31, Name = "Thunderbolt", Type = "Electrico", Power = 90 }, // Lv.36

        // Clefairy (Hada) - at least 4 same-type moves per request; Tackle reuses the Squirtle/Rattata Move row
        // (id 9) since Clefairy also learns it by level up (shared-move example)
        new() { Id = 32, Name = "Fairy Wind", Type = "Hada", Power = 40 },      // Legends: Z-A Lv.5
        new() { Id = 33, Name = "Disarming Voice", Type = "Hada", Power = 40 }, // Legends: Z-A Lv.8
        new() { Id = 34, Name = "Draining Kiss", Type = "Hada", Power = 50 },   // Legends: Z-A Lv.16
        new() { Id = 35, Name = "Dazzling Gleam", Type = "Hada", Power = 80 },  // Legends: Z-A Lv.32
    ];
}
