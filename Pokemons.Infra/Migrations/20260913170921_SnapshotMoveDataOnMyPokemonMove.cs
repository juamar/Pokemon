using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pokemons.Infra.Migrations
{
    /// <inheritdoc />
    public partial class SnapshotMoveDataOnMyPokemonMove : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "MyPokemonMoves",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Power",
                table: "MyPokemonMoves",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "MyPokemonMoves",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "MyPokemonMoves");

            migrationBuilder.DropColumn(
                name: "Power",
                table: "MyPokemonMoves");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "MyPokemonMoves");
        }
    }
}
