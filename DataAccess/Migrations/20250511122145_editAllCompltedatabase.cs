using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class editAllCompltedatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bookEvents_Children_ChildId",
                table: "bookEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_gameEvents_Children_ChildId",
                table: "gameEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Children_ChildID",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_Children_ChildID",
                table: "TestResults");

            migrationBuilder.DropForeignKey(
                name: "FK_videoEvents_Children_ChildId",
                table: "videoEvents");

            migrationBuilder.DropTable(
                name: "Children");

            migrationBuilder.DropTable(
                name: "Parents");

            migrationBuilder.DropIndex(
                name: "IX_videoEvents_ChildId",
                table: "videoEvents");

            migrationBuilder.DropIndex(
                name: "IX_TestResults_ChildID",
                table: "TestResults");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_ChildID",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_gameEvents_ChildId",
                table: "gameEvents");

            migrationBuilder.DropIndex(
                name: "IX_bookEvents_ChildId",
                table: "bookEvents");

            migrationBuilder.DropColumn(
                name: "AdminID",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "Child",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "ChildID",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "ChildId",
                table: "bookEvents");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Categories");

            migrationBuilder.AddColumn<int>(
                name: "AdminID",
                table: "Tests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Child",
                table: "Sessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ChildID",
                table: "Sessions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChildId",
                table: "bookEvents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Parents",
                columns: table => new
                {
                    ParentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Occupation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecondName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parents", x => x.ParentID);
                });

            migrationBuilder.CreateTable(
                name: "Children",
                columns: table => new
                {
                    ChildID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DevelopmentStatusID = table.Column<int>(type: "int", nullable: false),
                    DoctorID = table.Column<int>(type: "int", nullable: false),
                    ParentID = table.Column<int>(type: "int", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    DOB = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IQScore = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImgUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastTestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Children", x => x.ChildID);
                    table.ForeignKey(
                        name: "FK_Children_DevelopmentStatuses_DevelopmentStatusID",
                        column: x => x.DevelopmentStatusID,
                        principalTable: "DevelopmentStatuses",
                        principalColumn: "DevelopmentStatusID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Children_Doctors_DoctorID",
                        column: x => x.DoctorID,
                        principalTable: "Doctors",
                        principalColumn: "DoctorID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Children_Parents_ParentID",
                        column: x => x.ParentID,
                        principalTable: "Parents",
                        principalColumn: "ParentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_videoEvents_ChildId",
                table: "videoEvents",
                column: "ChildId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_ChildID",
                table: "TestResults",
                column: "ChildID");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_ChildID",
                table: "Sessions",
                column: "ChildID");

            migrationBuilder.CreateIndex(
                name: "IX_gameEvents_ChildId",
                table: "gameEvents",
                column: "ChildId");

            migrationBuilder.CreateIndex(
                name: "IX_bookEvents_ChildId",
                table: "bookEvents",
                column: "ChildId");

            migrationBuilder.CreateIndex(
                name: "IX_Children_DevelopmentStatusID",
                table: "Children",
                column: "DevelopmentStatusID");

            migrationBuilder.CreateIndex(
                name: "IX_Children_DoctorID",
                table: "Children",
                column: "DoctorID");

            migrationBuilder.CreateIndex(
                name: "IX_Children_ParentID",
                table: "Children",
                column: "ParentID");

            migrationBuilder.AddForeignKey(
                name: "FK_bookEvents_Children_ChildId",
                table: "bookEvents",
                column: "ChildId",
                principalTable: "Children",
                principalColumn: "ChildID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_gameEvents_Children_ChildId",
                table: "gameEvents",
                column: "ChildId",
                principalTable: "Children",
                principalColumn: "ChildID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Children_ChildID",
                table: "Sessions",
                column: "ChildID",
                principalTable: "Children",
                principalColumn: "ChildID");

            migrationBuilder.AddForeignKey(
                name: "FK_TestResults_Children_ChildID",
                table: "TestResults",
                column: "ChildID",
                principalTable: "Children",
                principalColumn: "ChildID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_videoEvents_Children_ChildId",
                table: "videoEvents",
                column: "ChildId",
                principalTable: "Children",
                principalColumn: "ChildID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
