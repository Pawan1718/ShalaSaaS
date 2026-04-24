using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shala.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentSuppliesModule_v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentSupplyIssues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    StudentAdmissionId = table.Column<int>(type: "int", nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DueAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentStatus = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsCancelled = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CancelReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentSupplyIssues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentSupplyIssues_StudentAdmissions_StudentAdmissionId",
                        column: x => x.StudentAdmissionId,
                        principalTable: "StudentAdmissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentSupplyIssues_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupplyItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SalePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurrentStock = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinimumStock = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplyItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentSupplyPayments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    StudentSupplyIssueId = table.Column<int>(type: "int", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMode = table.Column<int>(type: "int", nullable: false),
                    ReferenceNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentSupplyPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentSupplyPayments_StudentSupplyIssues_StudentSupplyIssueId",
                        column: x => x.StudentSupplyIssueId,
                        principalTable: "StudentSupplyIssues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentSupplyIssueItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentSupplyIssueId = table.Column<int>(type: "int", nullable: false),
                    SupplyItemId = table.Column<int>(type: "int", nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ItemCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LineTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentSupplyIssueItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentSupplyIssueItems_StudentSupplyIssues_StudentSupplyIssueId",
                        column: x => x.StudentSupplyIssueId,
                        principalTable: "StudentSupplyIssues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentSupplyIssueItems_SupplyItems_SupplyItemId",
                        column: x => x.SupplyItemId,
                        principalTable: "SupplyItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupplyStockLedgers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    SupplyItemId = table.Column<int>(type: "int", nullable: false),
                    MovementDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MovementType = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BalanceAfter = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReferenceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReferenceId = table.Column<int>(type: "int", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplyStockLedgers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplyStockLedgers_SupplyItems_SupplyItemId",
                        column: x => x.SupplyItemId,
                        principalTable: "SupplyItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentSupplyIssueItems_StudentSupplyIssueId",
                table: "StudentSupplyIssueItems",
                column: "StudentSupplyIssueId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentSupplyIssueItems_SupplyItemId",
                table: "StudentSupplyIssueItems",
                column: "SupplyItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentSupplyIssues_StudentAdmissionId",
                table: "StudentSupplyIssues",
                column: "StudentAdmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentSupplyIssues_StudentId",
                table: "StudentSupplyIssues",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentSupplyIssues_TenantId_BranchId_AcademicYearId_StudentId",
                table: "StudentSupplyIssues",
                columns: new[] { "TenantId", "BranchId", "AcademicYearId", "StudentId" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentSupplyIssues_TenantId_BranchId_IssueNo",
                table: "StudentSupplyIssues",
                columns: new[] { "TenantId", "BranchId", "IssueNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentSupplyPayments_StudentSupplyIssueId",
                table: "StudentSupplyPayments",
                column: "StudentSupplyIssueId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentSupplyPayments_TenantId_BranchId_AcademicYearId_PaymentDate",
                table: "StudentSupplyPayments",
                columns: new[] { "TenantId", "BranchId", "AcademicYearId", "PaymentDate" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplyItems_TenantId_BranchId_Code",
                table: "SupplyItems",
                columns: new[] { "TenantId", "BranchId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplyItems_TenantId_BranchId_Name",
                table: "SupplyItems",
                columns: new[] { "TenantId", "BranchId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplyStockLedgers_SupplyItemId",
                table: "SupplyStockLedgers",
                column: "SupplyItemId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyStockLedgers_TenantId_BranchId_SupplyItemId_MovementDate",
                table: "SupplyStockLedgers",
                columns: new[] { "TenantId", "BranchId", "SupplyItemId", "MovementDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentSupplyIssueItems");

            migrationBuilder.DropTable(
                name: "StudentSupplyPayments");

            migrationBuilder.DropTable(
                name: "SupplyStockLedgers");

            migrationBuilder.DropTable(
                name: "StudentSupplyIssues");

            migrationBuilder.DropTable(
                name: "SupplyItems");
        }
    }
}
