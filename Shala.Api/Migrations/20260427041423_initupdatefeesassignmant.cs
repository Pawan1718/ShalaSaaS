using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shala.Api.Migrations
{
    /// <inheritdoc />
    public partial class initupdatefeesassignmant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StudentFeeAssignments_TenantId_BranchId_StudentAdmissionId",
                table: "StudentFeeAssignments");

            migrationBuilder.CreateIndex(
                name: "IX_StudentFeeAssignments_TenantId_BranchId_StudentAdmissionId_FeeStructureId",
                table: "StudentFeeAssignments",
                columns: new[] { "TenantId", "BranchId", "StudentAdmissionId", "FeeStructureId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StudentFeeAssignments_TenantId_BranchId_StudentAdmissionId_FeeStructureId",
                table: "StudentFeeAssignments");

            migrationBuilder.CreateIndex(
                name: "IX_StudentFeeAssignments_TenantId_BranchId_StudentAdmissionId",
                table: "StudentFeeAssignments",
                columns: new[] { "TenantId", "BranchId", "StudentAdmissionId" },
                unique: true);
        }
    }
}
