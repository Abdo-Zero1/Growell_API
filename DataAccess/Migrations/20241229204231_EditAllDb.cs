using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class EditAllDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Admins_ApprovedByAdminAdminID",
                table: "Doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Users_UserID",
                table: "Doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Users_UserID1",
                table: "Doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_Parents_Users_UserID",
                table: "Parents");

            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_Tests_TestID",
                table: "TestResults");

            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_Tests_TestID1",
                table: "TestResults");

            migrationBuilder.DropForeignKey(
                name: "FK_Tests_Admins_AdminID",
                table: "Tests");

            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Tests_AdminID",
                table: "Tests");

            migrationBuilder.DropIndex(
                name: "IX_TestResults_TestID1",
                table: "TestResults");

            migrationBuilder.DropIndex(
                name: "IX_Parents_UserID",
                table: "Parents");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_ApprovedByAdminAdminID",
                table: "Doctors");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_UserID",
                table: "Doctors");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_UserID1",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "TestID1",
                table: "TestResults");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Parents");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "ApprovedByAdminAdminID",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "UserID1",
                table: "Doctors");

            migrationBuilder.RenameColumn(
                name: "ParentType",
                table: "Parents",
                newName: "SecondName");

            migrationBuilder.AddColumn<int>(
                name: "DoctorID",
                table: "Tests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Parents",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Parents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Parents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Parents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Parents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Parents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Parents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Doctors",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SecondName",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DoctorID",
                table: "Children",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "bookEvents",
                columns: table => new
                {
                    BookEventId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChildId = table.Column<int>(type: "int", nullable: false),
                    TestId = table.Column<int>(type: "int", nullable: false),
                    EventDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BookTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BookImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BookFilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DevelopmentStatusID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bookEvents", x => x.BookEventId);
                    table.ForeignKey(
                        name: "FK_bookEvents_Children_ChildId",
                        column: x => x.ChildId,
                        principalTable: "Children",
                        principalColumn: "ChildID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_bookEvents_DevelopmentStatuses_DevelopmentStatusID",
                        column: x => x.DevelopmentStatusID,
                        principalTable: "DevelopmentStatuses",
                        principalColumn: "DevelopmentStatusID");
                    table.ForeignKey(
                        name: "FK_bookEvents_Tests_TestId",
                        column: x => x.TestId,
                        principalTable: "Tests",
                        principalColumn: "TestID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "gameEvents",
                columns: table => new
                {
                    GameEventId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChildId = table.Column<int>(type: "int", nullable: false),
                    TestId = table.Column<int>(type: "int", nullable: false),
                    EventDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GameName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Score = table.Column<int>(type: "int", nullable: true),
                    GameImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GameFilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DevelopmentStatusID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gameEvents", x => x.GameEventId);
                    table.ForeignKey(
                        name: "FK_gameEvents_Children_ChildId",
                        column: x => x.ChildId,
                        principalTable: "Children",
                        principalColumn: "ChildID",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateTable(
                name: "videoEvents",
                columns: table => new
                {
                    VideoEventId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChildId = table.Column<int>(type: "int", nullable: false),
                    TestId = table.Column<int>(type: "int", nullable: false),
                    EventDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VideoTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Topic = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VideoFilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DevelopmentStatusID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_videoEvents", x => x.VideoEventId);
                    table.ForeignKey(
                        name: "FK_videoEvents_Children_ChildId",
                        column: x => x.ChildId,
                        principalTable: "Children",
                        principalColumn: "ChildID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_videoEvents_DevelopmentStatuses_DevelopmentStatusID",
                        column: x => x.DevelopmentStatusID,
                        principalTable: "DevelopmentStatuses",
                        principalColumn: "DevelopmentStatusID");
                    table.ForeignKey(
                        name: "FK_videoEvents_Tests_TestId",
                        column: x => x.TestId,
                        principalTable: "Tests",
                        principalColumn: "TestID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tests_DoctorID",
                table: "Tests",
                column: "DoctorID");

            migrationBuilder.CreateIndex(
                name: "IX_Children_DoctorID",
                table: "Children",
                column: "DoctorID");

            migrationBuilder.CreateIndex(
                name: "IX_bookEvents_ChildId",
                table: "bookEvents",
                column: "ChildId");

            migrationBuilder.CreateIndex(
                name: "IX_bookEvents_DevelopmentStatusID",
                table: "bookEvents",
                column: "DevelopmentStatusID");

            migrationBuilder.CreateIndex(
                name: "IX_bookEvents_TestId",
                table: "bookEvents",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_gameEvents_ChildId",
                table: "gameEvents",
                column: "ChildId");

            migrationBuilder.CreateIndex(
                name: "IX_gameEvents_DevelopmentStatusID",
                table: "gameEvents",
                column: "DevelopmentStatusID");

            migrationBuilder.CreateIndex(
                name: "IX_gameEvents_TestId",
                table: "gameEvents",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_videoEvents_ChildId",
                table: "videoEvents",
                column: "ChildId");

            migrationBuilder.CreateIndex(
                name: "IX_videoEvents_DevelopmentStatusID",
                table: "videoEvents",
                column: "DevelopmentStatusID");

            migrationBuilder.CreateIndex(
                name: "IX_videoEvents_TestId",
                table: "videoEvents",
                column: "TestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Children_Doctors_DoctorID",
                table: "Children",
                column: "DoctorID",
                principalTable: "Doctors",
                principalColumn: "DoctorID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestResults_Tests_TestID",
                table: "TestResults",
                column: "TestID",
                principalTable: "Tests",
                principalColumn: "TestID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_Doctors_DoctorID",
                table: "Tests",
                column: "DoctorID",
                principalTable: "Doctors",
                principalColumn: "DoctorID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Children_Doctors_DoctorID",
                table: "Children");

            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_Tests_TestID",
                table: "TestResults");

            migrationBuilder.DropForeignKey(
                name: "FK_Tests_Doctors_DoctorID",
                table: "Tests");

            migrationBuilder.DropTable(
                name: "bookEvents");

            migrationBuilder.DropTable(
                name: "gameEvents");

            migrationBuilder.DropTable(
                name: "videoEvents");

            migrationBuilder.DropIndex(
                name: "IX_Tests_DoctorID",
                table: "Tests");

            migrationBuilder.DropIndex(
                name: "IX_Children_DoctorID",
                table: "Children");

            migrationBuilder.DropColumn(
                name: "DoctorID",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Parents");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Parents");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Parents");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Parents");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Parents");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Parents");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Parents");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "SecondName",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "DoctorID",
                table: "Children");

            migrationBuilder.RenameColumn(
                name: "SecondName",
                table: "Parents",
                newName: "ParentType");

            migrationBuilder.AddColumn<int>(
                name: "TestID1",
                table: "TestResults",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserID",
                table: "Parents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ApprovedBy",
                table: "Doctors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ApprovedByAdminAdminID",
                table: "Doctors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserID",
                table: "Doctors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserID1",
                table: "Doctors",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecondName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    AdminID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    AdminRole = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Permissions = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.AdminID);
                    table.ForeignKey(
                        name: "FK_Admins_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tests_AdminID",
                table: "Tests",
                column: "AdminID");

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_TestID1",
                table: "TestResults",
                column: "TestID1");

            migrationBuilder.CreateIndex(
                name: "IX_Parents_UserID",
                table: "Parents",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_ApprovedByAdminAdminID",
                table: "Doctors",
                column: "ApprovedByAdminAdminID");

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_UserID",
                table: "Doctors",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_UserID1",
                table: "Doctors",
                column: "UserID1");

            migrationBuilder.CreateIndex(
                name: "IX_Admins_UserID",
                table: "Admins",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Admins_ApprovedByAdminAdminID",
                table: "Doctors",
                column: "ApprovedByAdminAdminID",
                principalTable: "Admins",
                principalColumn: "AdminID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Users_UserID",
                table: "Doctors",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Users_UserID1",
                table: "Doctors",
                column: "UserID1",
                principalTable: "Users",
                principalColumn: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Parents_Users_UserID",
                table: "Parents",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestResults_Tests_TestID",
                table: "TestResults",
                column: "TestID",
                principalTable: "Tests",
                principalColumn: "TestID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TestResults_Tests_TestID1",
                table: "TestResults",
                column: "TestID1",
                principalTable: "Tests",
                principalColumn: "TestID");

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_Admins_AdminID",
                table: "Tests",
                column: "AdminID",
                principalTable: "Admins",
                principalColumn: "AdminID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
