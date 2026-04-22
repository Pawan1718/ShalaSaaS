using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shala.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentFeeLedger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentFeeLedgers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    StudentAdmissionId = table.Column<int>(type: "int", nullable: false),
                    StudentChargeId = table.Column<int>(type: "int", nullable: true),
                    FeeReceiptId = table.Column<int>(type: "int", nullable: true),
                    FeeHeadId = table.Column<int>(type: "int", nullable: true),
                    EntryType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DebitAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreditAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RunningBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReferenceNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentFeeLedgers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentFeeLedgers_TenantId_BranchId_FeeReceiptId",
                table: "StudentFeeLedgers",
                columns: new[] { "TenantId", "BranchId", "FeeReceiptId" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentFeeLedgers_TenantId_BranchId_StudentAdmissionId_EntryDate_Id",
                table: "StudentFeeLedgers",
                columns: new[] { "TenantId", "BranchId", "StudentAdmissionId", "EntryDate", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentFeeLedgers_TenantId_BranchId_StudentChargeId",
                table: "StudentFeeLedgers",
                columns: new[] { "TenantId", "BranchId", "StudentChargeId" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentFeeLedgers_TenantId_BranchId_StudentId",
                table: "StudentFeeLedgers",
                columns: new[] { "TenantId", "BranchId", "StudentId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentFeeLedgers");
        }
    }
}
