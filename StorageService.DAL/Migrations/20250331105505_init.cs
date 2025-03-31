using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StorageService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArbitrageDifferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Symbol1 = table.Column<string>(type: "text", nullable: false),
                    Symbol2 = table.Column<string>(type: "text", nullable: false),
                    Price1 = table.Column<decimal>(type: "numeric", nullable: false),
                    Price2 = table.Column<decimal>(type: "numeric", nullable: false),
                    Difference = table.Column<decimal>(type: "numeric", nullable: false),
                    CalculatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArbitrageDifferences", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArbitrageDifferences");
        }
    }
}
