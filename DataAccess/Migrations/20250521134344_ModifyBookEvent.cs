using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ModifyBookEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bookEvents_TestResults_TestResultID",
                table: "bookEvents");

            migrationBuilder.DropIndex(
                name: "IX_bookEvents_TestResultID",
                table: "bookEvents");

            migrationBuilder.DropColumn(
                name: "EventDateTime",
                table: "bookEvents");

            migrationBuilder.DropColumn(
                name: "TestResultID",
                table: "bookEvents");

            migrationBuilder.RenameColumn(
                name: "BookFilePath",
                table: "bookEvents",
                newName: "Description");

            migrationBuilder.AddColumn<string>(
                name: "BookUrl",
                table: "bookEvents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookUrl",
                table: "bookEvents");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "bookEvents",
                newName: "BookFilePath");

            migrationBuilder.AddColumn<DateTime>(
                name: "EventDateTime",
                table: "bookEvents",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "TestResultID",
                table: "bookEvents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_bookEvents_TestResultID",
                table: "bookEvents",
                column: "TestResultID");

            migrationBuilder.AddForeignKey(
                name: "FK_bookEvents_TestResults_TestResultID",
                table: "bookEvents",
                column: "TestResultID",
                principalTable: "TestResults",
                principalColumn: "TestResultID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
