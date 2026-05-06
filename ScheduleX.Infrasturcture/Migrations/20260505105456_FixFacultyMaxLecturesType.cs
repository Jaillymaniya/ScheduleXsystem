using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleX.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixFacultyMaxLecturesType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "MaxLecturesPerDay",
                table: "TblFaculty",
                type: "int",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "MaxLecturesPerDay",
                table: "TblFaculty",
                type: "tinyint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
