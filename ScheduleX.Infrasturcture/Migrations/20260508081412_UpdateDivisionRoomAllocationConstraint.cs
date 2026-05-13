using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleX.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDivisionRoomAllocationConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TblDivisionRoomAllocation_RoomId",
                table: "TblDivisionRoomAllocation");

            migrationBuilder.CreateIndex(
                name: "IX_TblDivisionRoomAllocation_RoomId",
                table: "TblDivisionRoomAllocation",
                column: "RoomId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TblDivisionRoomAllocation_RoomId",
                table: "TblDivisionRoomAllocation");

            migrationBuilder.CreateIndex(
                name: "IX_TblDivisionRoomAllocation_RoomId",
                table: "TblDivisionRoomAllocation",
                column: "RoomId");
        }
    }
}
