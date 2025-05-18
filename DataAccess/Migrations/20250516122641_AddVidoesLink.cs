using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddVidoesLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VideoFilePath",
                table: "videoEvents",
                newName: "VideoImagePath");

            migrationBuilder.RenameColumn(
                name: "Topic",
                table: "videoEvents",
                newName: "Description");

            migrationBuilder.AddColumn<string>(
                name: "VideoUrl",
                table: "videoEvents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VideoUrl",
                table: "videoEvents");

            migrationBuilder.RenameColumn(
                name: "VideoImagePath",
                table: "videoEvents",
                newName: "VideoFilePath");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "videoEvents",
                newName: "Topic");
        }
    }
}
