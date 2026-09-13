using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pokemons.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddSpecialStatsAndSpeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BaseSpecialAttack",
                table: "MyPokemons",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BaseSpecialDefense",
                table: "MyPokemons",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BaseSpeed",
                table: "MyPokemons",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BaseSpecialAttack",
                table: "BasePokemons",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BaseSpecialDefense",
                table: "BasePokemons",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BaseSpeed",
                table: "BasePokemons",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "BasePokemons",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "BaseSpecialAttack", "BaseSpecialDefense", "BaseSpeed" },
                values: new object[] { 60, 50, 65 });

            migrationBuilder.UpdateData(
                table: "BasePokemons",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "BaseSpecialAttack", "BaseSpecialDefense", "BaseSpeed" },
                values: new object[] { 50, 64, 43 });

            migrationBuilder.UpdateData(
                table: "BasePokemons",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "BaseSpecialAttack", "BaseSpecialDefense", "BaseSpeed" },
                values: new object[] { 60, 65, 35 });

            migrationBuilder.UpdateData(
                table: "BasePokemons",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "BaseSpecialAttack", "BaseSpecialDefense", "BaseSpeed" },
                values: new object[] { 25, 35, 72 });

            migrationBuilder.UpdateData(
                table: "BasePokemons",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "BaseSpecialAttack", "BaseSpecialDefense", "BaseSpeed" },
                values: new object[] { 40, 54, 55 });

            migrationBuilder.UpdateData(
                table: "BasePokemons",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "BaseSpecialAttack", "BaseSpecialDefense", "BaseSpeed" },
                values: new object[] { 50, 50, 90 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseSpecialAttack",
                table: "MyPokemons");

            migrationBuilder.DropColumn(
                name: "BaseSpecialDefense",
                table: "MyPokemons");

            migrationBuilder.DropColumn(
                name: "BaseSpeed",
                table: "MyPokemons");

            migrationBuilder.DropColumn(
                name: "BaseSpecialAttack",
                table: "BasePokemons");

            migrationBuilder.DropColumn(
                name: "BaseSpecialDefense",
                table: "BasePokemons");

            migrationBuilder.DropColumn(
                name: "BaseSpeed",
                table: "BasePokemons");
        }
    }
}
