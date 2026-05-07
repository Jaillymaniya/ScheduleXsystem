using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleX.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRoomUniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TblRoom_DepartmentId",
                table: "TblRoom");

            migrationBuilder.CreateIndex(
                name: "IX_TblRoom_DepartmentId_RoomName",
                table: "TblRoom",
                columns: new[] { "DepartmentId", "RoomName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TblRoom_DepartmentId_RoomName",
                table: "TblRoom");

            migrationBuilder.CreateIndex(
                name: "IX_TblRoom_DepartmentId",
                table: "TblRoom",
                column: "DepartmentId");
        }
    }
}
