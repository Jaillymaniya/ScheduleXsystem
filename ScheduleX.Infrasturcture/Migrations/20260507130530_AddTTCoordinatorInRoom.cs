using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleX.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTTCoordinatorInRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TTCoordinatorId",
                table: "TblRoom",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TblRoom_TTCoordinatorId",
                table: "TblRoom",
                column: "TTCoordinatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_TblRoom_TblUser_TTCoordinatorId",
                table: "TblRoom",
                column: "TTCoordinatorId",
                principalTable: "TblUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TblRoom_TblUser_TTCoordinatorId",
                table: "TblRoom");

            migrationBuilder.DropIndex(
                name: "IX_TblRoom_TTCoordinatorId",
                table: "TblRoom");

            migrationBuilder.DropColumn(
                name: "TTCoordinatorId",
                table: "TblRoom");
        }
    }
}
