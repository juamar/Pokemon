using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pokemons.Infra.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedDataRealPokemon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "BasePokemons",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "BaseAttack", "BaseDefense", "Name", "TotalHP", "Type" },
                values: new object[] { 45, 48, "Clefairy", 70, "Hada" });

            migrationBuilder.UpdateData(
                table: "BasePokemons",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "BaseAttack", "BaseDefense", "Name", "TotalHP", "Type" },
                values: new object[] { 56, 35, "Rattata", 30, "Normal" });

            migrationBuilder.UpdateData(
                table: "BasePokemons",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "BaseAttack", "BaseDefense", "Name", "TotalHP", "Type" },
                values: new object[] { 60, 44, "Ekans", 35, "Veneno" });

            migrationBuilder.UpdateData(
                table: "BasePokemons",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "BaseAttack", "Name", "TotalHP", "Type" },
                values: new object[] { 55, "Pikachu", 35, "Electrico" });

            migrationBuilder.UpdateData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Name", "Type" },
                values: new object[] { "Scratch", "Normal" });

            migrationBuilder.UpdateData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Name", "Type" },
                values: new object[] { "Ember", "Fuego" });

            migrationBuilder.UpdateData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Name", "Power", "Type" },
                values: new object[] { "Dragon Breath", 60, "Dragon" });

            migrationBuilder.UpdateData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Name", "Power", "Type" },
                values: new object[] { "Fire Fang", 65, "Fuego" });

            migrationBuilder.UpdateData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Name", "Power", "Type" },
                values: new object[] { "Slash", 70, "Normal" });

            migrationBuilder.UpdateData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Name", "Power", "Type" },
                values: new object[] { "Flamethrower", 90, "Fuego" });

            migrationBuilder.UpdateData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Name", "Power", "Type" },
                values: new object[] { "Fire Spin", 35, "Fuego" });

            migrationBuilder.InsertData(
                table: "Moves",
                columns: new[] { "Id", "Name", "Power", "Type" },
                values: new object[,]
                {
                    { 8, "Flare Blitz", 120, "Fuego" },
                    { 9, "Tackle", 40, "Normal" },
                    { 10, "Water Gun", 40, "Agua" },
                    { 11, "Rapid Spin", 50, "Normal" },
                    { 12, "Bite", 60, "Siniestro" },
                    { 13, "Water Pulse", 60, "Agua" },
                    { 14, "Aqua Tail", 90, "Agua" },
                    { 15, "Hydro Pump", 110, "Agua" },
                    { 16, "Quick Attack", 40, "Normal" },
                    { 17, "Take Down", 90, "Normal" },
                    { 18, "Crunch", 80, "Siniestro" },
                    { 19, "Double-Edge", 120, "Normal" },
                    { 20, "Endeavor", 100, "Normal" },
                    { 21, "Wrap", 15, "Normal" },
                    { 22, "Poison Sting", 15, "Veneno" },
                    { 23, "Acid", 40, "Veneno" },
                    { 24, "Acid Spray", 40, "Veneno" },
                    { 25, "Sludge Bomb", 90, "Veneno" },
                    { 26, "Nuzzle", 20, "Electrico" },
                    { 27, "Thunder Shock", 40, "Electrico" },
                    { 28, "Spark", 65, "Electrico" },
                    { 29, "Iron Tail", 100, "Acero" },
                    { 30, "Discharge", 80, "Electrico" },
                    { 31, "Thunderbolt", 90, "Electrico" },
                    { 32, "Fairy Wind", 40, "Hada" },
                    { 33, "Disarming Voice", 40, "Hada" },
                    { 34, "Draining Kiss", 50, "Hada" },
                    { 35, "Dazzling Gleam", 80, "Hada" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.UpdateData(
                table: "BasePokemons",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "BaseAttack", "BaseDefense", "Name", "TotalHP", "Type" },
                values: new object[] { 49, 49, "Bulbasaur", 45, "Planta" });

            migrationBuilder.UpdateData(
                table: "BasePokemons",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "BaseAttack", "BaseDefense", "Name", "TotalHP", "Type" },
                values: new object[] { 55, 40, "Pikachu", 35, "Electrico" });

            migrationBuilder.UpdateData(
                table: "BasePokemons",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "BaseAttack", "BaseDefense", "Name", "TotalHP", "Type" },
                values: new object[] { 80, 100, "Geodude", 40, "Roca" });

            migrationBuilder.UpdateData(
                table: "BasePokemons",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "BaseAttack", "Name", "TotalHP", "Type" },
                values: new object[] { 45, "Pidgey", 40, "Volador" });

            migrationBuilder.UpdateData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Name", "Type" },
                values: new object[] { "Ember", "Fuego" });

            migrationBuilder.UpdateData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Name", "Type" },
                values: new object[] { "Water Gun", "Agua" });

            migrationBuilder.UpdateData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Name", "Power", "Type" },
                values: new object[] { "Vine Whip", 45, "Planta" });

            migrationBuilder.UpdateData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Name", "Power", "Type" },
                values: new object[] { "Thunder Shock", 40, "Electrico" });

            migrationBuilder.UpdateData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Name", "Power", "Type" },
                values: new object[] { "Rock Throw", 50, "Roca" });

            migrationBuilder.UpdateData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Name", "Power", "Type" },
                values: new object[] { "Gust", 40, "Volador" });

            migrationBuilder.UpdateData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Name", "Power", "Type" },
                values: new object[] { "Tackle", 40, "Normal" });
        }
    }
}
