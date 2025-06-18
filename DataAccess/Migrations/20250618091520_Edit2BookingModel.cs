using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Edit2BookingModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bookings_AspNetUsers_UserID",
                table: "bookings");

            migrationBuilder.RenameColumn(
                name: "PatientName",
                table: "bookings",
                newName: "TestDoctorName");

            migrationBuilder.RenameColumn(
                name: "DoctorName",
                table: "bookings",
                newName: "CreatedByUserName");

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "bookings",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "BookingDoctorName",
                table: "bookings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_bookings_AspNetUsers_UserID",
                table: "bookings",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bookings_AspNetUsers_UserID",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "BookingDoctorName",
                table: "bookings");

            migrationBuilder.RenameColumn(
                name: "TestDoctorName",
                table: "bookings",
                newName: "PatientName");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserName",
                table: "bookings",
                newName: "DoctorName");

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "bookings",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_bookings_AspNetUsers_UserID",
                table: "bookings",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
