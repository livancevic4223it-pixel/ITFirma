using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITFirma.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Klijenti",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Adresa = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Klijenti", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Zaposleni",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ime = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Prezime = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Pozicija = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zaposleni", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projekti",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DatumPocetka = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DatumZavrsetka = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    KlijentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projekti", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projekti_Klijenti_KlijentId",
                        column: x => x.KlijentId,
                        principalTable: "Klijenti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Angazovanja",
                columns: table => new
                {
                    ProjekatId = table.Column<int>(type: "int", nullable: false),
                    ZaposleniId = table.Column<int>(type: "int", nullable: false),
                    Uloga = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Satnica = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    DatumOd = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Angazovanja", x => new { x.ProjekatId, x.ZaposleniId });
                    table.ForeignKey(
                        name: "FK_Angazovanja_Projekti_ProjekatId",
                        column: x => x.ProjekatId,
                        principalTable: "Projekti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Angazovanja_Zaposleni_ZaposleniId",
                        column: x => x.ZaposleniId,
                        principalTable: "Zaposleni",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Zadaci",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Rok = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ProjekatId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zadaci", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Zadaci_Projekti_ProjekatId",
                        column: x => x.ProjekatId,
                        principalTable: "Projekti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Angazovanja_ZaposleniId",
                table: "Angazovanja",
                column: "ZaposleniId");

            migrationBuilder.CreateIndex(
                name: "IX_Projekti_KlijentId",
                table: "Projekti",
                column: "KlijentId");

            migrationBuilder.CreateIndex(
                name: "IX_Zadaci_ProjekatId",
                table: "Zadaci",
                column: "ProjekatId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Angazovanja");

            migrationBuilder.DropTable(
                name: "Zadaci");

            migrationBuilder.DropTable(
                name: "Zaposleni");

            migrationBuilder.DropTable(
                name: "Projekti");

            migrationBuilder.DropTable(
                name: "Klijenti");
        }
    }
}
