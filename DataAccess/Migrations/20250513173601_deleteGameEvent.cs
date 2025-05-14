using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class deleteGameEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bookEvents_Tests_TestId",
                table: "bookEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_videoEvents_Tests_TestId",
                table: "videoEvents");

            migrationBuilder.DropTable(
                name: "gameEvents");

            migrationBuilder.DropColumn(
                name: "ChildId",
                table: "videoEvents");

            migrationBuilder.RenameColumn(
                name: "TestId",
                table: "videoEvents",
                newName: "TestResultID");

            migrationBuilder.RenameIndex(
                name: "IX_videoEvents_TestId",
                table: "videoEvents",
                newName: "IX_videoEvents_TestResultID");

            migrationBuilder.RenameColumn(
                name: "TestId",
                table: "bookEvents",
                newName: "TestResultID");

            migrationBuilder.RenameIndex(
                name: "IX_bookEvents_TestId",
                table: "bookEvents",
                newName: "IX_bookEvents_TestResultID");

            migrationBuilder.AddForeignKey(
                name: "FK_bookEvents_TestResults_TestResultID",
                table: "bookEvents",
                column: "TestResultID",
                principalTable: "TestResults",
                principalColumn: "TestResultID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_videoEvents_TestResults_TestResultID",
                table: "videoEvents",
                column: "TestResultID",
                principalTable: "TestResults",
                principalColumn: "TestResultID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bookEvents_TestResults_TestResultID",
                table: "bookEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_videoEvents_TestResults_TestResultID",
                table: "videoEvents");

            migrationBuilder.RenameColumn(
                name: "TestResultID",
                table: "videoEvents",
                newName: "TestId");

            migrationBuilder.RenameIndex(
                name: "IX_videoEvents_TestResultID",
                table: "videoEvents",
                newName: "IX_videoEvents_TestId");

            migrationBuilder.RenameColumn(
                name: "TestResultID",
                table: "bookEvents",
                newName: "TestId");

            migrationBuilder.RenameIndex(
                name: "IX_bookEvents_TestResultID",
                table: "bookEvents",
                newName: "IX_bookEvents_TestId");

            migrationBuilder.AddColumn<int>(
                name: "ChildId",
                table: "videoEvents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "gameEvents",
                columns: table => new
                {
                    GameEventId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DevelopmentStatusID = table.Column<int>(type: "int", nullable: true),
                    TestId = table.Column<int>(type: "int", nullable: false),
                    ChildId = table.Column<int>(type: "int", nullable: false),
                    EventDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GameFilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GameImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GameName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Score = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gameEvents", x => x.GameEventId);
                    table.ForeignKey(
                        name: "FK_gameEvents_DevelopmentStatuses_DevelopmentStatusID",
                        column: x => x.DevelopmentStatusID,
                        principalTable: "DevelopmentStatuses",
                        principalColumn: "DevelopmentStatusID");
                    table.ForeignKey(
                        name: "FK_gameEvents_Tests_TestId",
                        column: x => x.TestId,
                        principalTable: "Tests",
                        principalColumn: "TestID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_gameEvents_DevelopmentStatusID",
                table: "gameEvents",
                column: "DevelopmentStatusID");

            migrationBuilder.CreateIndex(
                name: "IX_gameEvents_TestId",
                table: "gameEvents",
                column: "TestId");

            migrationBuilder.AddForeignKey(
                name: "FK_bookEvents_Tests_TestId",
                table: "bookEvents",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "TestID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_videoEvents_Tests_TestId",
                table: "videoEvents",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "TestID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
