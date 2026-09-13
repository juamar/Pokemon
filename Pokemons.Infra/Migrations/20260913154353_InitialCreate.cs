using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pokemons.Infra.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BasePokemons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Level = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalHP = table.Column<int>(type: "INTEGER", nullable: false),
                    BaseAttack = table.Column<int>(type: "INTEGER", nullable: false),
                    BaseDefense = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasePokemons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Moves",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Power = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Moves", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TypeEffectivenesses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AttackerType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DefenderType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Multiplier = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeEffectivenesses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MyPokemons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OwnerId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    BasePokemonId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Level = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentHP = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalHP = table.Column<int>(type: "INTEGER", nullable: false),
                    BaseAttack = table.Column<int>(type: "INTEGER", nullable: false),
                    BaseDefense = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyPokemons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MyPokemons_BasePokemons_BasePokemonId",
                        column: x => x.BasePokemonId,
                        principalTable: "BasePokemons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Battles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Pokemon1Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Pokemon2Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentTurnPokemonId = table.Column<int>(type: "INTEGER", nullable: false),
                    WinnerPokemonId = table.Column<int>(type: "INTEGER", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FinishedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Battles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Battles_MyPokemons_Pokemon1Id",
                        column: x => x.Pokemon1Id,
                        principalTable: "MyPokemons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Battles_MyPokemons_Pokemon2Id",
                        column: x => x.Pokemon2Id,
                        principalTable: "MyPokemons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MyPokemonMoves",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MyPokemonId = table.Column<int>(type: "INTEGER", nullable: false),
                    MoveId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyPokemonMoves", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MyPokemonMoves_Moves_MoveId",
                        column: x => x.MoveId,
                        principalTable: "Moves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MyPokemonMoves_MyPokemons_MyPokemonId",
                        column: x => x.MyPokemonId,
                        principalTable: "MyPokemons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BattleActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BattleId = table.Column<int>(type: "INTEGER", nullable: false),
                    ActingPokemonId = table.Column<int>(type: "INTEGER", nullable: false),
                    MoveId = table.Column<int>(type: "INTEGER", nullable: false),
                    DamageDealt = table.Column<int>(type: "INTEGER", nullable: false),
                    TurnNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    ExecutedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BattleActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BattleActions_Battles_BattleId",
                        column: x => x.BattleId,
                        principalTable: "Battles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BattleActions_Moves_MoveId",
                        column: x => x.MoveId,
                        principalTable: "Moves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BattleActions_MyPokemons_ActingPokemonId",
                        column: x => x.ActingPokemonId,
                        principalTable: "MyPokemons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "BasePokemons",
                columns: new[] { "Id", "BaseAttack", "BaseDefense", "Level", "Name", "TotalHP", "Type" },
                values: new object[,]
                {
                    { 1, 52, 43, 5, "Charmander", 39, "Fuego" },
                    { 2, 48, 65, 5, "Squirtle", 44, "Agua" },
                    { 3, 49, 49, 5, "Bulbasaur", 45, "Planta" },
                    { 4, 55, 40, 5, "Pikachu", 35, "Electrico" },
                    { 5, 80, 100, 5, "Geodude", 40, "Roca" },
                    { 6, 45, 40, 5, "Pidgey", 40, "Volador" }
                });

            migrationBuilder.InsertData(
                table: "Moves",
                columns: new[] { "Id", "Name", "Power", "Type" },
                values: new object[,]
                {
                    { 1, "Ember", 40, "Fuego" },
                    { 2, "Water Gun", 40, "Agua" },
                    { 3, "Vine Whip", 45, "Planta" },
                    { 4, "Thunder Shock", 40, "Electrico" },
                    { 5, "Rock Throw", 50, "Roca" },
                    { 6, "Gust", 40, "Volador" },
                    { 7, "Tackle", 40, "Normal" }
                });

            migrationBuilder.InsertData(
                table: "TypeEffectivenesses",
                columns: new[] { "Id", "AttackerType", "DefenderType", "Multiplier" },
                values: new object[,]
                {
                    { 1, "Acero", "Acero", 0.5 },
                    { 2, "Acero", "Agua", 0.5 },
                    { 3, "Acero", "Bicho", 1.0 },
                    { 4, "Acero", "Dragon", 1.0 },
                    { 5, "Acero", "Electrico", 0.5 },
                    { 6, "Acero", "Fantasma", 1.0 },
                    { 7, "Acero", "Fuego", 0.5 },
                    { 8, "Acero", "Hada", 2.0 },
                    { 9, "Acero", "Hielo", 2.0 },
                    { 10, "Acero", "Lucha", 1.0 },
                    { 11, "Acero", "Normal", 1.0 },
                    { 12, "Acero", "Planta", 1.0 },
                    { 13, "Acero", "Psiquico", 1.0 },
                    { 14, "Acero", "Roca", 2.0 },
                    { 15, "Acero", "Siniestro", 1.0 },
                    { 16, "Acero", "Tierra", 1.0 },
                    { 17, "Acero", "Veneno", 1.0 },
                    { 18, "Acero", "Volador", 1.0 },
                    { 19, "Agua", "Acero", 1.0 },
                    { 20, "Agua", "Agua", 0.5 },
                    { 21, "Agua", "Bicho", 1.0 },
                    { 22, "Agua", "Dragon", 0.5 },
                    { 23, "Agua", "Electrico", 1.0 },
                    { 24, "Agua", "Fantasma", 1.0 },
                    { 25, "Agua", "Fuego", 2.0 },
                    { 26, "Agua", "Hada", 1.0 },
                    { 27, "Agua", "Hielo", 1.0 },
                    { 28, "Agua", "Lucha", 1.0 },
                    { 29, "Agua", "Normal", 1.0 },
                    { 30, "Agua", "Planta", 0.5 },
                    { 31, "Agua", "Psiquico", 1.0 },
                    { 32, "Agua", "Roca", 2.0 },
                    { 33, "Agua", "Siniestro", 1.0 },
                    { 34, "Agua", "Tierra", 2.0 },
                    { 35, "Agua", "Veneno", 1.0 },
                    { 36, "Agua", "Volador", 1.0 },
                    { 37, "Bicho", "Acero", 0.5 },
                    { 38, "Bicho", "Agua", 1.0 },
                    { 39, "Bicho", "Bicho", 1.0 },
                    { 40, "Bicho", "Dragon", 1.0 },
                    { 41, "Bicho", "Electrico", 1.0 },
                    { 42, "Bicho", "Fantasma", 0.5 },
                    { 43, "Bicho", "Fuego", 0.5 },
                    { 44, "Bicho", "Hada", 0.5 },
                    { 45, "Bicho", "Hielo", 1.0 },
                    { 46, "Bicho", "Lucha", 0.5 },
                    { 47, "Bicho", "Normal", 1.0 },
                    { 48, "Bicho", "Planta", 2.0 },
                    { 49, "Bicho", "Psiquico", 2.0 },
                    { 50, "Bicho", "Roca", 1.0 },
                    { 51, "Bicho", "Siniestro", 2.0 },
                    { 52, "Bicho", "Tierra", 1.0 },
                    { 53, "Bicho", "Veneno", 0.5 },
                    { 54, "Bicho", "Volador", 0.5 },
                    { 55, "Dragon", "Acero", 0.5 },
                    { 56, "Dragon", "Agua", 1.0 },
                    { 57, "Dragon", "Bicho", 1.0 },
                    { 58, "Dragon", "Dragon", 2.0 },
                    { 59, "Dragon", "Electrico", 1.0 },
                    { 60, "Dragon", "Fantasma", 1.0 },
                    { 61, "Dragon", "Fuego", 1.0 },
                    { 62, "Dragon", "Hada", 0.0 },
                    { 63, "Dragon", "Hielo", 1.0 },
                    { 64, "Dragon", "Lucha", 1.0 },
                    { 65, "Dragon", "Normal", 1.0 },
                    { 66, "Dragon", "Planta", 1.0 },
                    { 67, "Dragon", "Psiquico", 1.0 },
                    { 68, "Dragon", "Roca", 1.0 },
                    { 69, "Dragon", "Siniestro", 1.0 },
                    { 70, "Dragon", "Tierra", 1.0 },
                    { 71, "Dragon", "Veneno", 1.0 },
                    { 72, "Dragon", "Volador", 1.0 },
                    { 73, "Electrico", "Acero", 1.0 },
                    { 74, "Electrico", "Agua", 2.0 },
                    { 75, "Electrico", "Bicho", 1.0 },
                    { 76, "Electrico", "Dragon", 0.5 },
                    { 77, "Electrico", "Electrico", 0.5 },
                    { 78, "Electrico", "Fantasma", 1.0 },
                    { 79, "Electrico", "Fuego", 1.0 },
                    { 80, "Electrico", "Hada", 1.0 },
                    { 81, "Electrico", "Hielo", 1.0 },
                    { 82, "Electrico", "Lucha", 1.0 },
                    { 83, "Electrico", "Normal", 1.0 },
                    { 84, "Electrico", "Planta", 0.5 },
                    { 85, "Electrico", "Psiquico", 1.0 },
                    { 86, "Electrico", "Roca", 1.0 },
                    { 87, "Electrico", "Siniestro", 1.0 },
                    { 88, "Electrico", "Tierra", 0.0 },
                    { 89, "Electrico", "Veneno", 1.0 },
                    { 90, "Electrico", "Volador", 2.0 },
                    { 91, "Fantasma", "Acero", 1.0 },
                    { 92, "Fantasma", "Agua", 1.0 },
                    { 93, "Fantasma", "Bicho", 1.0 },
                    { 94, "Fantasma", "Dragon", 1.0 },
                    { 95, "Fantasma", "Electrico", 1.0 },
                    { 96, "Fantasma", "Fantasma", 2.0 },
                    { 97, "Fantasma", "Fuego", 1.0 },
                    { 98, "Fantasma", "Hada", 1.0 },
                    { 99, "Fantasma", "Hielo", 1.0 },
                    { 100, "Fantasma", "Lucha", 1.0 },
                    { 101, "Fantasma", "Normal", 0.0 },
                    { 102, "Fantasma", "Planta", 1.0 },
                    { 103, "Fantasma", "Psiquico", 2.0 },
                    { 104, "Fantasma", "Roca", 1.0 },
                    { 105, "Fantasma", "Siniestro", 0.5 },
                    { 106, "Fantasma", "Tierra", 1.0 },
                    { 107, "Fantasma", "Veneno", 1.0 },
                    { 108, "Fantasma", "Volador", 1.0 },
                    { 109, "Fuego", "Acero", 2.0 },
                    { 110, "Fuego", "Agua", 0.5 },
                    { 111, "Fuego", "Bicho", 2.0 },
                    { 112, "Fuego", "Dragon", 0.5 },
                    { 113, "Fuego", "Electrico", 1.0 },
                    { 114, "Fuego", "Fantasma", 1.0 },
                    { 115, "Fuego", "Fuego", 0.5 },
                    { 116, "Fuego", "Hada", 1.0 },
                    { 117, "Fuego", "Hielo", 2.0 },
                    { 118, "Fuego", "Lucha", 1.0 },
                    { 119, "Fuego", "Normal", 1.0 },
                    { 120, "Fuego", "Planta", 2.0 },
                    { 121, "Fuego", "Psiquico", 1.0 },
                    { 122, "Fuego", "Roca", 0.5 },
                    { 123, "Fuego", "Siniestro", 1.0 },
                    { 124, "Fuego", "Tierra", 1.0 },
                    { 125, "Fuego", "Veneno", 1.0 },
                    { 126, "Fuego", "Volador", 1.0 },
                    { 127, "Hada", "Acero", 0.5 },
                    { 128, "Hada", "Agua", 1.0 },
                    { 129, "Hada", "Bicho", 1.0 },
                    { 130, "Hada", "Dragon", 2.0 },
                    { 131, "Hada", "Electrico", 1.0 },
                    { 132, "Hada", "Fantasma", 1.0 },
                    { 133, "Hada", "Fuego", 0.5 },
                    { 134, "Hada", "Hada", 1.0 },
                    { 135, "Hada", "Hielo", 1.0 },
                    { 136, "Hada", "Lucha", 2.0 },
                    { 137, "Hada", "Normal", 1.0 },
                    { 138, "Hada", "Planta", 1.0 },
                    { 139, "Hada", "Psiquico", 1.0 },
                    { 140, "Hada", "Roca", 1.0 },
                    { 141, "Hada", "Siniestro", 2.0 },
                    { 142, "Hada", "Tierra", 1.0 },
                    { 143, "Hada", "Veneno", 0.5 },
                    { 144, "Hada", "Volador", 1.0 },
                    { 145, "Hielo", "Acero", 0.5 },
                    { 146, "Hielo", "Agua", 0.5 },
                    { 147, "Hielo", "Bicho", 1.0 },
                    { 148, "Hielo", "Dragon", 2.0 },
                    { 149, "Hielo", "Electrico", 1.0 },
                    { 150, "Hielo", "Fantasma", 1.0 },
                    { 151, "Hielo", "Fuego", 0.5 },
                    { 152, "Hielo", "Hada", 1.0 },
                    { 153, "Hielo", "Hielo", 0.5 },
                    { 154, "Hielo", "Lucha", 1.0 },
                    { 155, "Hielo", "Normal", 1.0 },
                    { 156, "Hielo", "Planta", 2.0 },
                    { 157, "Hielo", "Psiquico", 1.0 },
                    { 158, "Hielo", "Roca", 1.0 },
                    { 159, "Hielo", "Siniestro", 1.0 },
                    { 160, "Hielo", "Tierra", 2.0 },
                    { 161, "Hielo", "Veneno", 1.0 },
                    { 162, "Hielo", "Volador", 2.0 },
                    { 163, "Lucha", "Acero", 2.0 },
                    { 164, "Lucha", "Agua", 1.0 },
                    { 165, "Lucha", "Bicho", 0.5 },
                    { 166, "Lucha", "Dragon", 1.0 },
                    { 167, "Lucha", "Electrico", 1.0 },
                    { 168, "Lucha", "Fantasma", 0.0 },
                    { 169, "Lucha", "Fuego", 1.0 },
                    { 170, "Lucha", "Hada", 0.5 },
                    { 171, "Lucha", "Hielo", 2.0 },
                    { 172, "Lucha", "Lucha", 1.0 },
                    { 173, "Lucha", "Normal", 2.0 },
                    { 174, "Lucha", "Planta", 1.0 },
                    { 175, "Lucha", "Psiquico", 0.5 },
                    { 176, "Lucha", "Roca", 2.0 },
                    { 177, "Lucha", "Siniestro", 2.0 },
                    { 178, "Lucha", "Tierra", 1.0 },
                    { 179, "Lucha", "Veneno", 0.5 },
                    { 180, "Lucha", "Volador", 0.5 },
                    { 181, "Normal", "Acero", 0.5 },
                    { 182, "Normal", "Agua", 1.0 },
                    { 183, "Normal", "Bicho", 1.0 },
                    { 184, "Normal", "Dragon", 1.0 },
                    { 185, "Normal", "Electrico", 1.0 },
                    { 186, "Normal", "Fantasma", 0.0 },
                    { 187, "Normal", "Fuego", 1.0 },
                    { 188, "Normal", "Hada", 1.0 },
                    { 189, "Normal", "Hielo", 1.0 },
                    { 190, "Normal", "Lucha", 1.0 },
                    { 191, "Normal", "Normal", 1.0 },
                    { 192, "Normal", "Planta", 1.0 },
                    { 193, "Normal", "Psiquico", 1.0 },
                    { 194, "Normal", "Roca", 0.5 },
                    { 195, "Normal", "Siniestro", 1.0 },
                    { 196, "Normal", "Tierra", 1.0 },
                    { 197, "Normal", "Veneno", 1.0 },
                    { 198, "Normal", "Volador", 1.0 },
                    { 199, "Planta", "Acero", 0.5 },
                    { 200, "Planta", "Agua", 2.0 },
                    { 201, "Planta", "Bicho", 0.5 },
                    { 202, "Planta", "Dragon", 0.5 },
                    { 203, "Planta", "Electrico", 1.0 },
                    { 204, "Planta", "Fantasma", 1.0 },
                    { 205, "Planta", "Fuego", 0.5 },
                    { 206, "Planta", "Hada", 1.0 },
                    { 207, "Planta", "Hielo", 1.0 },
                    { 208, "Planta", "Lucha", 1.0 },
                    { 209, "Planta", "Normal", 1.0 },
                    { 210, "Planta", "Planta", 0.5 },
                    { 211, "Planta", "Psiquico", 1.0 },
                    { 212, "Planta", "Roca", 2.0 },
                    { 213, "Planta", "Siniestro", 1.0 },
                    { 214, "Planta", "Tierra", 2.0 },
                    { 215, "Planta", "Veneno", 0.5 },
                    { 216, "Planta", "Volador", 0.5 },
                    { 217, "Psiquico", "Acero", 0.5 },
                    { 218, "Psiquico", "Agua", 1.0 },
                    { 219, "Psiquico", "Bicho", 1.0 },
                    { 220, "Psiquico", "Dragon", 1.0 },
                    { 221, "Psiquico", "Electrico", 1.0 },
                    { 222, "Psiquico", "Fantasma", 1.0 },
                    { 223, "Psiquico", "Fuego", 1.0 },
                    { 224, "Psiquico", "Hada", 1.0 },
                    { 225, "Psiquico", "Hielo", 1.0 },
                    { 226, "Psiquico", "Lucha", 2.0 },
                    { 227, "Psiquico", "Normal", 1.0 },
                    { 228, "Psiquico", "Planta", 1.0 },
                    { 229, "Psiquico", "Psiquico", 0.5 },
                    { 230, "Psiquico", "Roca", 1.0 },
                    { 231, "Psiquico", "Siniestro", 0.0 },
                    { 232, "Psiquico", "Tierra", 1.0 },
                    { 233, "Psiquico", "Veneno", 2.0 },
                    { 234, "Psiquico", "Volador", 1.0 },
                    { 235, "Roca", "Acero", 0.5 },
                    { 236, "Roca", "Agua", 1.0 },
                    { 237, "Roca", "Bicho", 2.0 },
                    { 238, "Roca", "Dragon", 1.0 },
                    { 239, "Roca", "Electrico", 1.0 },
                    { 240, "Roca", "Fantasma", 1.0 },
                    { 241, "Roca", "Fuego", 2.0 },
                    { 242, "Roca", "Hada", 1.0 },
                    { 243, "Roca", "Hielo", 2.0 },
                    { 244, "Roca", "Lucha", 0.5 },
                    { 245, "Roca", "Normal", 1.0 },
                    { 246, "Roca", "Planta", 1.0 },
                    { 247, "Roca", "Psiquico", 1.0 },
                    { 248, "Roca", "Roca", 1.0 },
                    { 249, "Roca", "Siniestro", 1.0 },
                    { 250, "Roca", "Tierra", 0.5 },
                    { 251, "Roca", "Veneno", 1.0 },
                    { 252, "Roca", "Volador", 2.0 },
                    { 253, "Siniestro", "Acero", 1.0 },
                    { 254, "Siniestro", "Agua", 1.0 },
                    { 255, "Siniestro", "Bicho", 1.0 },
                    { 256, "Siniestro", "Dragon", 1.0 },
                    { 257, "Siniestro", "Electrico", 1.0 },
                    { 258, "Siniestro", "Fantasma", 2.0 },
                    { 259, "Siniestro", "Fuego", 1.0 },
                    { 260, "Siniestro", "Hada", 0.5 },
                    { 261, "Siniestro", "Hielo", 1.0 },
                    { 262, "Siniestro", "Lucha", 0.5 },
                    { 263, "Siniestro", "Normal", 1.0 },
                    { 264, "Siniestro", "Planta", 1.0 },
                    { 265, "Siniestro", "Psiquico", 2.0 },
                    { 266, "Siniestro", "Roca", 1.0 },
                    { 267, "Siniestro", "Siniestro", 0.5 },
                    { 268, "Siniestro", "Tierra", 1.0 },
                    { 269, "Siniestro", "Veneno", 1.0 },
                    { 270, "Siniestro", "Volador", 1.0 },
                    { 271, "Tierra", "Acero", 2.0 },
                    { 272, "Tierra", "Agua", 1.0 },
                    { 273, "Tierra", "Bicho", 0.5 },
                    { 274, "Tierra", "Dragon", 1.0 },
                    { 275, "Tierra", "Electrico", 2.0 },
                    { 276, "Tierra", "Fantasma", 1.0 },
                    { 277, "Tierra", "Fuego", 2.0 },
                    { 278, "Tierra", "Hada", 1.0 },
                    { 279, "Tierra", "Hielo", 1.0 },
                    { 280, "Tierra", "Lucha", 1.0 },
                    { 281, "Tierra", "Normal", 1.0 },
                    { 282, "Tierra", "Planta", 0.5 },
                    { 283, "Tierra", "Psiquico", 1.0 },
                    { 284, "Tierra", "Roca", 2.0 },
                    { 285, "Tierra", "Siniestro", 1.0 },
                    { 286, "Tierra", "Tierra", 1.0 },
                    { 287, "Tierra", "Veneno", 2.0 },
                    { 288, "Tierra", "Volador", 0.0 },
                    { 289, "Veneno", "Acero", 0.0 },
                    { 290, "Veneno", "Agua", 1.0 },
                    { 291, "Veneno", "Bicho", 1.0 },
                    { 292, "Veneno", "Dragon", 1.0 },
                    { 293, "Veneno", "Electrico", 1.0 },
                    { 294, "Veneno", "Fantasma", 0.5 },
                    { 295, "Veneno", "Fuego", 1.0 },
                    { 296, "Veneno", "Hada", 2.0 },
                    { 297, "Veneno", "Hielo", 1.0 },
                    { 298, "Veneno", "Lucha", 1.0 },
                    { 299, "Veneno", "Normal", 1.0 },
                    { 300, "Veneno", "Planta", 2.0 },
                    { 301, "Veneno", "Psiquico", 1.0 },
                    { 302, "Veneno", "Roca", 0.5 },
                    { 303, "Veneno", "Siniestro", 1.0 },
                    { 304, "Veneno", "Tierra", 0.5 },
                    { 305, "Veneno", "Veneno", 0.5 },
                    { 306, "Veneno", "Volador", 1.0 },
                    { 307, "Volador", "Acero", 0.5 },
                    { 308, "Volador", "Agua", 1.0 },
                    { 309, "Volador", "Bicho", 2.0 },
                    { 310, "Volador", "Dragon", 1.0 },
                    { 311, "Volador", "Electrico", 0.5 },
                    { 312, "Volador", "Fantasma", 1.0 },
                    { 313, "Volador", "Fuego", 1.0 },
                    { 314, "Volador", "Hada", 1.0 },
                    { 315, "Volador", "Hielo", 1.0 },
                    { 316, "Volador", "Lucha", 2.0 },
                    { 317, "Volador", "Normal", 1.0 },
                    { 318, "Volador", "Planta", 2.0 },
                    { 319, "Volador", "Psiquico", 1.0 },
                    { 320, "Volador", "Roca", 0.5 },
                    { 321, "Volador", "Siniestro", 1.0 },
                    { 322, "Volador", "Tierra", 1.0 },
                    { 323, "Volador", "Veneno", 1.0 },
                    { 324, "Volador", "Volador", 1.0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BattleActions_ActingPokemonId",
                table: "BattleActions",
                column: "ActingPokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_BattleActions_BattleId",
                table: "BattleActions",
                column: "BattleId");

            migrationBuilder.CreateIndex(
                name: "IX_BattleActions_MoveId",
                table: "BattleActions",
                column: "MoveId");

            migrationBuilder.CreateIndex(
                name: "IX_Battles_Pokemon1Id",
                table: "Battles",
                column: "Pokemon1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Battles_Pokemon2Id",
                table: "Battles",
                column: "Pokemon2Id");

            migrationBuilder.CreateIndex(
                name: "IX_MyPokemonMoves_MoveId",
                table: "MyPokemonMoves",
                column: "MoveId");

            migrationBuilder.CreateIndex(
                name: "IX_MyPokemonMoves_MyPokemonId",
                table: "MyPokemonMoves",
                column: "MyPokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_MyPokemons_BasePokemonId",
                table: "MyPokemons",
                column: "BasePokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_TypeEffectivenesses_AttackerType_DefenderType",
                table: "TypeEffectivenesses",
                columns: new[] { "AttackerType", "DefenderType" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BattleActions");

            migrationBuilder.DropTable(
                name: "MyPokemonMoves");

            migrationBuilder.DropTable(
                name: "TypeEffectivenesses");

            migrationBuilder.DropTable(
                name: "Battles");

            migrationBuilder.DropTable(
                name: "Moves");

            migrationBuilder.DropTable(
                name: "MyPokemons");

            migrationBuilder.DropTable(
                name: "BasePokemons");
        }
    }
}
