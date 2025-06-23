using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class EditBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bookings_TestResults_TestResultID",
                table: "bookings");

            migrationBuilder.DropIndex(
                name: "IX_bookings_TestResultID",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "TestDoctorName",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "TestResultID",
                table: "bookings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TestDoctorName",
                table: "bookings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TestResultID",
                table: "bookings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_bookings_TestResultID",
                table: "bookings",
                column: "TestResultID");

            migrationBuilder.AddForeignKey(
                name: "FK_bookings_TestResults_TestResultID",
                table: "bookings",
                column: "TestResultID",
                principalTable: "TestResults",
                principalColumn: "TestResultID");
        }
    }
}
