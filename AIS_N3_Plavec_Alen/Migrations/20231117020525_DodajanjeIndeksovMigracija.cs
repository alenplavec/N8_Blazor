using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIS_N3_Plavec_Alen.Migrations
{
    /// <inheritdoc />
    public partial class DodajanjeIndeksovMigracija : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Izdelki_Naziv",
                table: "Izdelki",
                column: "Naziv");

            migrationBuilder.CreateIndex(
                name: "IX_Dobavitelji_Naziv",
                table: "Dobavitelji",
                column: "Naziv");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Izdelki_Naziv",
                table: "Izdelki");

            migrationBuilder.DropIndex(
                name: "IX_Dobavitelji_Naziv",
                table: "Dobavitelji");
        }
    }
}