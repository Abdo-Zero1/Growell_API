using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class EditVideo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_videoEvents_DevelopmentStatuses_DevelopmentStatusID",
                table: "videoEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_videoEvents_TestResults_TestResultID",
                table: "videoEvents");

            migrationBuilder.DropIndex(
                name: "IX_videoEvents_DevelopmentStatusID",
                table: "videoEvents");

            migrationBuilder.DropIndex(
                name: "IX_videoEvents_TestResultID",
                table: "videoEvents");

            migrationBuilder.DropColumn(
                name: "DevelopmentStatusID",
                table: "videoEvents");

            migrationBuilder.DropColumn(
                name: "EventDateTime",
                table: "videoEvents");

            migrationBuilder.DropColumn(
                name: "TestResultID",
                table: "videoEvents");

            migrationBuilder.DropColumn(
                name: "ChildID",
                table: "TestResults");

            

        

           

           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            

            
            

            migrationBuilder.AddColumn<int>(
                name: "DevelopmentStatusID",
                table: "videoEvents",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EventDateTime",
                table: "videoEvents",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "TestResultID",
                table: "videoEvents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ChildID",
                table: "TestResults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_videoEvents_DevelopmentStatusID",
                table: "videoEvents",
                column: "DevelopmentStatusID");

            migrationBuilder.CreateIndex(
                name: "IX_videoEvents_TestResultID",
                table: "videoEvents",
                column: "TestResultID");

            migrationBuilder.AddForeignKey(
                name: "FK_videoEvents_DevelopmentStatuses_DevelopmentStatusID",
                table: "videoEvents",
                column: "DevelopmentStatusID",
                principalTable: "DevelopmentStatuses",
                principalColumn: "DevelopmentStatusID");

            migrationBuilder.AddForeignKey(
                name: "FK_videoEvents_TestResults_TestResultID",
                table: "videoEvents",
                column: "TestResultID",
                principalTable: "TestResults",
                principalColumn: "TestResultID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
