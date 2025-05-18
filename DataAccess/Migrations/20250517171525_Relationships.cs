using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Relationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "TestResults",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "DoctorID",
                table: "TestResults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_DoctorID",
                table: "TestResults",
                column: "DoctorID");

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_UserID",
                table: "TestResults",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_TestResults_AspNetUsers_UserID",
                table: "TestResults",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TestResults_Doctors_DoctorID",
                table: "TestResults",
                column: "DoctorID",
                principalTable: "Doctors",
                principalColumn: "DoctorID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_AspNetUsers_UserID",
                table: "TestResults");

            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_Doctors_DoctorID",
                table: "TestResults");

            migrationBuilder.DropIndex(
                name: "IX_TestResults_DoctorID",
                table: "TestResults");

            migrationBuilder.DropIndex(
                name: "IX_TestResults_UserID",
                table: "TestResults");

            migrationBuilder.DropColumn(
                name: "DoctorID",
                table: "TestResults");

            migrationBuilder.AlterColumn<int>(
                name: "UserID",
                table: "TestResults",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
