using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AboutOfKidsInDoctor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TargetAudience",
                table: "Doctors",
                newName: "AboutOfKids");

            migrationBuilder.AddColumn<string>(
                name: "TargetAgeGroup",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TargetAgeGroup",
                table: "Doctors");

            migrationBuilder.RenameColumn(
                name: "AboutOfKids",
                table: "Doctors",
                newName: "TargetAudience");
        }
    }
}
