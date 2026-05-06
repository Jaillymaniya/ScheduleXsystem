using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleX.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAcademicYearToSubjectSemester : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AcademicYearId",
                table: "TblSubjectSemester",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TblSubjectSemester_AcademicYearId",
                table: "TblSubjectSemester",
                column: "AcademicYearId");

            migrationBuilder.AddForeignKey(
                name: "FK_TblSubjectSemester_TblAcademicYear_AcademicYearId",
                table: "TblSubjectSemester",
                column: "AcademicYearId",
                principalTable: "TblAcademicYear",
                principalColumn: "AcademicYearId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TblSubjectSemester_TblAcademicYear_AcademicYearId",
                table: "TblSubjectSemester");

            migrationBuilder.DropIndex(
                name: "IX_TblSubjectSemester_AcademicYearId",
                table: "TblSubjectSemester");

            migrationBuilder.DropColumn(
                name: "AcademicYearId",
                table: "TblSubjectSemester");
        }
    }
}
