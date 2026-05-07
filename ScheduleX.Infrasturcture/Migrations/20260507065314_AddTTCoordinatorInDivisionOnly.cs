using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleX.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTTCoordinatorInDivisionOnly : Migration
    {
        /// <inheritdoc />
        //protected override void Up(MigrationBuilder migrationBuilder)
        //{
        //    migrationBuilder.AddColumn<bool>(
        //        name: "IsActive",
        //        table: "TblTTCoordinatorCourse",
        //        type: "bit",
        //        nullable: false,
        //        defaultValue: false);

        //    migrationBuilder.AddColumn<int>(
        //        name: "TTCoordinatorId",
        //        table: "TblDivision",
        //        type: "int",
        //        nullable: true);

        //    migrationBuilder.CreateIndex(
        //        name: "IX_TblDivision_TTCoordinatorId",
        //        table: "TblDivision",
        //        column: "TTCoordinatorId");

        //    migrationBuilder.AddForeignKey(
        //        name: "FK_TblDivision_TblUser_TTCoordinatorId",
        //        table: "TblDivision",
        //        column: "TTCoordinatorId",
        //        principalTable: "TblUser",
        //        principalColumn: "Id",
        //        onDelete: ReferentialAction.Restrict);
        //}

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TTCoordinatorId",
                table: "TblDivision",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TblDivision_TTCoordinatorId",
                table: "TblDivision",
                column: "TTCoordinatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_TblDivision_TblUser_TTCoordinatorId",
                table: "TblDivision",
                column: "TTCoordinatorId",
                principalTable: "TblUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }


        /// <inheritdoc />
        //protected override void Down(MigrationBuilder migrationBuilder)
        //{
        //    migrationBuilder.DropForeignKey(
        //        name: "FK_TblDivision_TblUser_TTCoordinatorId",
        //        table: "TblDivision");

        //    migrationBuilder.DropIndex(
        //        name: "IX_TblDivision_TTCoordinatorId",
        //        table: "TblDivision");

        //    migrationBuilder.DropColumn(
        //        name: "IsActive",
        //        table: "TblTTCoordinatorCourse");

        //    migrationBuilder.DropColumn(
        //        name: "TTCoordinatorId",
        //        table: "TblDivision");
        //}


        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TblDivision_TblUser_TTCoordinatorId",
                table: "TblDivision");

            migrationBuilder.DropIndex(
                name: "IX_TblDivision_TTCoordinatorId",
                table: "TblDivision");

            migrationBuilder.DropColumn(
                name: "TTCoordinatorId",
                table: "TblDivision");
        }
    }
}
