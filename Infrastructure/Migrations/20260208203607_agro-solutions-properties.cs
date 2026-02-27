using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class agrosolutionsproperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Property",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "VARCHAR(200)", nullable: false),
                    AddressStreet = table.Column<string>(type: "VARCHAR(200)", nullable: false),
                    AddressNumber = table.Column<string>(type: "VARCHAR(20)", nullable: false),
                    AddressComplement = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    AddressCity = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    AddressState = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    AddressZipCode = table.Column<string>(type: "VARCHAR(20)", nullable: false),
                    AddressCountry = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    Latitude = table.Column<double>(type: "DOUBLE PRECISION", nullable: false),
                    Longitude = table.Column<double>(type: "DOUBLE PRECISION", nullable: false),
                    TotalArea = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    SoilType = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    UserId = table.Column<int>(type: "INT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Property", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Field",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "VARCHAR(200)", nullable: false),
                    CropType = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    Area = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PlantingDate = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: true),
                    Latitude = table.Column<double>(type: "DOUBLE PRECISION", nullable: false),
                    Longitude = table.Column<double>(type: "DOUBLE PRECISION", nullable: false),
                    Status = table.Column<int>(type: "INT", nullable: false),
                    PropertyId = table.Column<int>(type: "INT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Field", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Field_Property_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Property",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Field_PropertyId",
                table: "Field",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_Property_UserId",
                table: "Property",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Field");

            migrationBuilder.DropTable(
                name: "Property");
        }
    }
}
