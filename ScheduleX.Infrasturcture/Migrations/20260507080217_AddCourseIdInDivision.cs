using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleX.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseIdInDivision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TblDivision_AcademicYearId_SemesterId_DivisionName_TTCoordinatorId",
                table: "TblDivision");

            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "TblDivision",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TblDivision_AcademicYearId_CourseId_SemesterId_DivisionName_TTCoordinatorId",
                table: "TblDivision",
                columns: new[] { "AcademicYearId", "CourseId", "SemesterId", "DivisionName", "TTCoordinatorId" },
                unique: true,
                filter: "[TTCoordinatorId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TblDivision_CourseId",
                table: "TblDivision",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_TblDivision_TblCourse_CourseId",
                table: "TblDivision",
                column: "CourseId",
                principalTable: "TblCourse",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TblDivision_TblCourse_CourseId",
                table: "TblDivision");

            migrationBuilder.DropIndex(
                name: "IX_TblDivision_AcademicYearId_CourseId_SemesterId_DivisionName_TTCoordinatorId",
                table: "TblDivision");

            migrationBuilder.DropIndex(
                name: "IX_TblDivision_CourseId",
                table: "TblDivision");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "TblDivision");

            migrationBuilder.CreateIndex(
                name: "IX_TblDivision_AcademicYearId_SemesterId_DivisionName_TTCoordinatorId",
                table: "TblDivision",
                columns: new[] { "AcademicYearId", "SemesterId", "DivisionName", "TTCoordinatorId" },
                unique: true,
                filter: "[TTCoordinatorId] IS NOT NULL");
        }
    }
}
