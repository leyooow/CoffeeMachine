using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeMachine.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CoffeeMachineStates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    BrewCount = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoffeeMachineStates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MachineLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Endpoint = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    StatusCode = table.Column<int>(type: "INTEGER", nullable: false),
                    Message = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachineLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MachineLogs_CreatedDate",
                table: "MachineLogs",
                column: "CreatedDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoffeeMachineStates");

            migrationBuilder.DropTable(
                name: "MachineLogs");
        }
    }
}
