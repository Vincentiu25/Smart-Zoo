using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MobyLabWebProgramming.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSpeciesRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Species",
                table: "ZooAnimals");

            migrationBuilder.AddColumn<Guid>(
                name: "SpeciesId",
                table: "ZooAnimals",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Species",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CommonName = table.Column<string>(type: "text", nullable: false),
                    ScientificName = table.Column<string>(type: "text", nullable: false),
                    Habitat = table.Column<string>(type: "text", nullable: false),
                    Diet = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Species", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ZooAnimals_SpeciesId",
                table: "ZooAnimals",
                column: "SpeciesId");

            migrationBuilder.AddForeignKey(
                name: "FK_ZooAnimals_Species_SpeciesId",
                table: "ZooAnimals",
                column: "SpeciesId",
                principalTable: "Species",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ZooAnimals_Species_SpeciesId",
                table: "ZooAnimals");

            migrationBuilder.DropTable(
                name: "Species");

            migrationBuilder.DropIndex(
                name: "IX_ZooAnimals_SpeciesId",
                table: "ZooAnimals");

            migrationBuilder.DropColumn(
                name: "SpeciesId",
                table: "ZooAnimals");

            migrationBuilder.AddColumn<string>(
                name: "Species",
                table: "ZooAnimals",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
