using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ModelsLibrary.Migrations
{
    public partial class AddComplaintModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateTime",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "Flagged",
                table: "Reviews");

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeAdded",
                table: "Reviews",
                nullable: false,
                defaultValueSql: "getdate()");

            migrationBuilder.CreateTable(
                name: "Complaint",
                columns: table => new
                {
                    UserID = table.Column<string>(nullable: false),
                    PlaceID = table.Column<string>(nullable: false),
                    TimeSubmitted = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    Comment = table.Column<string>(nullable: true),
                    TimeLastUpdated = table.Column<DateTime>(nullable: false),
                    Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Complaint", x => new { x.PlaceID, x.UserID, x.TimeSubmitted });
                    table.ForeignKey(
                        name: "FK_Complaint_Locations_PlaceID",
                        column: x => x.PlaceID,
                        principalTable: "Locations",
                        principalColumn: "PlaceID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Complaint_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Locations_PlaceID",
                table: "Locations",
                column: "PlaceID");

            migrationBuilder.CreateIndex(
                name: "IX_Complaint_UserID",
                table: "Complaint",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Complaint");

            migrationBuilder.DropIndex(
                name: "IX_Locations_PlaceID",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "TimeAdded",
                table: "Reviews");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTime",
                table: "Reviews",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Flagged",
                table: "Reviews",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
