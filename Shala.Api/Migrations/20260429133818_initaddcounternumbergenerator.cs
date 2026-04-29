using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shala.Api.Migrations
{
    /// <inheritdoc />
    public partial class initaddcounternumbergenerator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StudentAdmissions_TenantId_BranchId_AdmissionNo",
                table: "StudentAdmissions");

            migrationBuilder.CreateTable(
                name: "AdmissionNumberCounters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    LastNumber = table.Column<int>(type: "int", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmissionNumberCounters", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentAdmissions_TenantId_BranchId_AdmissionNo",
                table: "StudentAdmissions",
                columns: new[] { "TenantId", "BranchId", "AdmissionNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionNumberCounters_TenantId_BranchId_AcademicYearId",
                table: "AdmissionNumberCounters",
                columns: new[] { "TenantId", "BranchId", "AcademicYearId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdmissionNumberCounters");

            migrationBuilder.DropIndex(
                name: "IX_StudentAdmissions_TenantId_BranchId_AdmissionNo",
                table: "StudentAdmissions");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAdmissions_TenantId_BranchId_AdmissionNo",
                table: "StudentAdmissions",
                columns: new[] { "TenantId", "BranchId", "AdmissionNo" });
        }
    }
}
