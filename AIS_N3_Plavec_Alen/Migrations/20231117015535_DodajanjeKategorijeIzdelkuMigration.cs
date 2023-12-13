using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIS_N3_Plavec_Alen.Migrations
{
    /// <inheritdoc />
    public partial class DodajanjeKategorijeIzdelkuMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Kategorija",
                table: "Izdelki",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Kategorija",
                table: "Izdelki");
        }
    }
}