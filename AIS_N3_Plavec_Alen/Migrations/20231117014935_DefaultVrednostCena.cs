using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIS_N3_Plavec_Alen.Migrations
{
    /// <inheritdoc />
    public partial class DefaultVrednostCena : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Cena",
                table: "Izdelki",
                type: "TEXT",
                nullable: false,
                defaultValue: 0.0m,
                oldClrType: typeof(decimal),
                oldType: "TEXT");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Cena",
                table: "Izdelki",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldDefaultValue: 0.0m);
        }
    }
}