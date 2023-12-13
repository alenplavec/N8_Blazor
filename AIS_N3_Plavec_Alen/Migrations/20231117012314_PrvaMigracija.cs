using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIS_N3_Plavec_Alen.Migrations
{
    /// <inheritdoc />
    public partial class PrvaMigracija : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dobavitelji",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ime = table.Column<string>(type: "TEXT", nullable: false),
                    Lokacija = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dobavitelji", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Izdelki",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Naziv = table.Column<string>(type: "TEXT", nullable: false),
                    Opis = table.Column<string>(type: "TEXT", nullable: false),
                    Cena = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Izdelki", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IzdelekDobavitelji",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IzdelekId = table.Column<int>(type: "INTEGER", nullable: false),
                    DobaviteljId = table.Column<int>(type: "INTEGER", nullable: false),
                    KolicinaNaZalogi = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IzdelekDobavitelji", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IzdelekDobavitelji_Dobavitelji_DobaviteljId",
                        column: x => x.DobaviteljId,
                        principalTable: "Dobavitelji",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IzdelekDobavitelji_Izdelki_IzdelekId",
                        column: x => x.IzdelekId,
                        principalTable: "Izdelki",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IzdelekDobavitelji_DobaviteljId",
                table: "IzdelekDobavitelji",
                column: "DobaviteljId");

            migrationBuilder.CreateIndex(
                name: "IX_IzdelekDobavitelji_IzdelekId",
                table: "IzdelekDobavitelji",
                column: "IzdelekId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IzdelekDobavitelji");

            migrationBuilder.DropTable(
                name: "Dobavitelji");

            migrationBuilder.DropTable(
                name: "Izdelki");
        }
    }
}