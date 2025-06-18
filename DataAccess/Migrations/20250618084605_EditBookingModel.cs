using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class EditBookingModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DoctorName",
                table: "bookings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PatientName",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bookings_TestResults_TestResultID",
                table: "bookings");

            migrationBuilder.DropIndex(
                name: "IX_bookings_TestResultID",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "DoctorName",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "PatientName",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "TestResultID",
                table: "bookings");
        }
    }
}
