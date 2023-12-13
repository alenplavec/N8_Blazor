using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIS_N3_Plavec_Alen.Migrations
{
    /// <inheritdoc />
    public partial class spreminjanjeImenMigracija : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Ime",
                table: "Dobavitelji",
                newName: "Naziv");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Dobavitelji",
                newName: "Kontakt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Naziv",
                table: "Dobavitelji",
                newName: "Ime");

            migrationBuilder.RenameColumn(
                name: "Kontakt",
                table: "Dobavitelji",
                newName: "Email");
        }
    }
}