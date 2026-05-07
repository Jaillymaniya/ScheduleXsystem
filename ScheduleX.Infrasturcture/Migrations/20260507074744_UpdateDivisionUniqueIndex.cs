//using Microsoft.EntityFrameworkCore.Migrations;

//#nullable disable

//namespace ScheduleX.Infrastructure.Migrations
//{
//    /// <inheritdoc />
//    public partial class UpdateDivisionUniqueIndex : Migration
//    {
//        /// <inheritdoc />
//        protected override void Up(MigrationBuilder migrationBuilder)
//        {


//            using Microsoft.EntityFrameworkCore.Migrations;

//#nullable disable

//namespace ScheduleX.Infrastructure.Migrations
//    {
//        /// <inheritdoc />
//        public partial class UpdateDivisionUniqueIndex : Migration
//        {
//            /// <inheritdoc />
//            protected override void Up(MigrationBuilder migrationBuilder)
//            {
//                migrationBuilder.CreateIndex(
//        name: "IX_TblDivision_AcademicYearId_SemesterId_DivisionName_TTCoordinatorId",
//        table: "TblDivision",
//        columns: new[]
//        {
//        "AcademicYearId",
//        "SemesterId",
//        "DivisionName",
//        "TTCoordinatorId"
//        },
//        unique: true);

//                migrationBuilder.CreateIndex(
//                    name: "IX_TblDivision_AcademicYearId_SemesterId_DivisionName_TTCoordinatorId",
//                    table: "TblDivision",
//                    columns: new[] { "AcademicYearId", "SemesterId", "DivisionName", "TTCoordinatorId" },
//                    unique: true,
//                    filter: "[TTCoordinatorId] IS NOT NULL");
//            }

//            /// <inheritdoc />
//            protected override void Down(MigrationBuilder migrationBuilder)
//            {
//                migrationBuilder.DropIndex(
//                    name: "IX_TblDivision_AcademicYearId_SemesterId_DivisionName_TTCoordinatorId",
//                    table: "TblDivision");

//                migrationBuilder.CreateIndex(
//                    name: "IX_TblDivision_AcademicYearId",
//                    table: "TblDivision",
//                    column: "AcademicYearId");
//            }
//        }
//    }

//}

///// <inheritdoc />
//protected override void Down(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.DropIndex(
//                name: "IX_TblDivision_AcademicYearId_SemesterId_DivisionName_TTCoordinatorId",
//                table: "TblDivision");

//            migrationBuilder.CreateIndex(
//                name: "IX_TblDivision_AcademicYearId",
//                table: "TblDivision",
//                column: "AcademicYearId");
//        }
//    }
//}



using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleX.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDivisionUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // REMOVE OLD UNIQUE INDEX

            migrationBuilder.DropIndex(
                name: "IX_TblDivision_SemesterId_DivisionName",
                table: "TblDivision");

            // CREATE NEW TT-WISE UNIQUE INDEX

            migrationBuilder.CreateIndex(
                name: "IX_TblDivision_AcademicYearId_SemesterId_DivisionName_TTCoordinatorId",
                table: "TblDivision",
                columns: new[]
                {
                    "AcademicYearId",
                    "SemesterId",
                    "DivisionName",
                    "TTCoordinatorId"
                },
                unique: true,
                filter: "[TTCoordinatorId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // REMOVE NEW INDEX

            migrationBuilder.DropIndex(
                name: "IX_TblDivision_AcademicYearId_SemesterId_DivisionName_TTCoordinatorId",
                table: "TblDivision");

            // RESTORE OLD INDEX

            migrationBuilder.CreateIndex(
                name: "IX_TblDivision_SemesterId_DivisionName",
                table: "TblDivision",
                columns: new[]
                {
                    "SemesterId",
                    "DivisionName"
                },
                unique: true);
        }
    }
}