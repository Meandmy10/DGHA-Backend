using Microsoft.EntityFrameworkCore.Migrations;

namespace ModelsLibrary.Migrations
{
    public partial class AddStatusDefaultValue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Complaint_Locations_PlaceID",
                table: "Complaint");

            migrationBuilder.DropForeignKey(
                name: "FK_Complaint_AspNetUsers_UserID",
                table: "Complaint");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Complaint",
                table: "Complaint");

            migrationBuilder.RenameTable(
                name: "Complaint",
                newName: "Complaints");

            migrationBuilder.RenameIndex(
                name: "IX_Complaint_UserID",
                table: "Complaints",
                newName: "IX_Complaints_UserID");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Complaints",
                nullable: true,
                defaultValue: "Newly Created",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Complaints",
                table: "Complaints",
                columns: new[] { "PlaceID", "UserID", "TimeSubmitted" });

            migrationBuilder.AddForeignKey(
                name: "FK_Complaints_Locations_PlaceID",
                table: "Complaints",
                column: "PlaceID",
                principalTable: "Locations",
                principalColumn: "PlaceID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Complaints_AspNetUsers_UserID",
                table: "Complaints",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Complaints_Locations_PlaceID",
                table: "Complaints");

            migrationBuilder.DropForeignKey(
                name: "FK_Complaints_AspNetUsers_UserID",
                table: "Complaints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Complaints",
                table: "Complaints");

            migrationBuilder.RenameTable(
                name: "Complaints",
                newName: "Complaint");

            migrationBuilder.RenameIndex(
                name: "IX_Complaints_UserID",
                table: "Complaint",
                newName: "IX_Complaint_UserID");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Complaint",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true,
                oldDefaultValue: "Newly Created");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Complaint",
                table: "Complaint",
                columns: new[] { "PlaceID", "UserID", "TimeSubmitted" });

            migrationBuilder.AddForeignKey(
                name: "FK_Complaint_Locations_PlaceID",
                table: "Complaint",
                column: "PlaceID",
                principalTable: "Locations",
                principalColumn: "PlaceID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Complaint_AspNetUsers_UserID",
                table: "Complaint",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
