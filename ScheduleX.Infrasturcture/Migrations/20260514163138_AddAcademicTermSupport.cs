using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleX.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAcademicTermSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TblDivisionRoomAllocation_DivisionId",
                table: "TblDivisionRoomAllocation");

            migrationBuilder.AddColumn<int>(
                name: "AcademicTermId",
                table: "TblTimeTableBatch",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "SubjectName",
                table: "TblSubject",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AddColumn<byte>(
                name: "SemesterPattern",
                table: "TblSemester",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "AcademicTermId",
                table: "TblDivisionRoomAllocation",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TblAcademicTerm",
                columns: table => new
                {
                    AcademicTermId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    TermType = table.Column<byte>(type: "tinyint", nullable: false),
                    SemesterPattern = table.Column<byte>(type: "tinyint", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblAcademicTerm", x => x.AcademicTermId);
                    table.ForeignKey(
                        name: "FK_TblAcademicTerm_TblAcademicYear_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "TblAcademicYear",
                        principalColumn: "AcademicYearId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TblAcademicTerm_TblCourse_CourseId",
                        column: x => x.CourseId,
                        principalTable: "TblCourse",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TblTimeTableBatch_AcademicTermId",
                table: "TblTimeTableBatch",
                column: "AcademicTermId");

            migrationBuilder.CreateIndex(
                name: "IX_TblDivisionRoomAllocation_AcademicTermId_DivisionId",
                table: "TblDivisionRoomAllocation",
                columns: new[] { "AcademicTermId", "DivisionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TblDivisionRoomAllocation_DivisionId",
                table: "TblDivisionRoomAllocation",
                column: "DivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_TblAcademicTerm_AcademicYearId_CourseId_IsCurrent",
                table: "TblAcademicTerm",
                columns: new[] { "AcademicYearId", "CourseId", "IsCurrent" },
                unique: true,
                filter: "[IsCurrent] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_TblAcademicTerm_AcademicYearId_CourseId_TermType",
                table: "TblAcademicTerm",
                columns: new[] { "AcademicYearId", "CourseId", "TermType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TblAcademicTerm_CourseId",
                table: "TblAcademicTerm",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_TblDivisionRoomAllocation_TblAcademicTerm_AcademicTermId",
                table: "TblDivisionRoomAllocation",
                column: "AcademicTermId",
                principalTable: "TblAcademicTerm",
                principalColumn: "AcademicTermId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TblTimeTableBatch_TblAcademicTerm_AcademicTermId",
                table: "TblTimeTableBatch",
                column: "AcademicTermId",
                principalTable: "TblAcademicTerm",
                principalColumn: "AcademicTermId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TblDivisionRoomAllocation_TblAcademicTerm_AcademicTermId",
                table: "TblDivisionRoomAllocation");

            migrationBuilder.DropForeignKey(
                name: "FK_TblTimeTableBatch_TblAcademicTerm_AcademicTermId",
                table: "TblTimeTableBatch");

            migrationBuilder.DropTable(
                name: "TblAcademicTerm");

            migrationBuilder.DropIndex(
                name: "IX_TblTimeTableBatch_AcademicTermId",
                table: "TblTimeTableBatch");

            migrationBuilder.DropIndex(
                name: "IX_TblDivisionRoomAllocation_AcademicTermId_DivisionId",
                table: "TblDivisionRoomAllocation");

            migrationBuilder.DropIndex(
                name: "IX_TblDivisionRoomAllocation_DivisionId",
                table: "TblDivisionRoomAllocation");

            migrationBuilder.DropColumn(
                name: "AcademicTermId",
                table: "TblTimeTableBatch");

            migrationBuilder.DropColumn(
                name: "SemesterPattern",
                table: "TblSemester");

            migrationBuilder.DropColumn(
                name: "AcademicTermId",
                table: "TblDivisionRoomAllocation");

            migrationBuilder.AlterColumn<string>(
                name: "SubjectName",
                table: "TblSubject",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300);

            migrationBuilder.CreateIndex(
                name: "IX_TblDivisionRoomAllocation_DivisionId",
                table: "TblDivisionRoomAllocation",
                column: "DivisionId",
                unique: true);
        }
    }
}
