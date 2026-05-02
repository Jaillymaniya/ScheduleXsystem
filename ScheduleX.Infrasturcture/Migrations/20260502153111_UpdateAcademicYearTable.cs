using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleX.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAcademicYearTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "TblAcademicYear");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "TblAcademicYear");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "EndDate",
                table: "TblAcademicYear",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "StartDate",
                table: "TblAcademicYear",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }
    }
}
