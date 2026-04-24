using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shala.Api.Migrations
{
    /// <inheritdoc />
    public partial class registration_receipt_cancel_refund_audit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRefunded",
                table: "RegistrationFeeReceipts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ReceiptStatus",
                table: "RegistrationFeeReceipts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RefundReason",
                table: "RegistrationFeeReceipts",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RefundedAmount",
                table: "RegistrationFeeReceipts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "RefundedBy",
                table: "RegistrationFeeReceipts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefundedOn",
                table: "RegistrationFeeReceipts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RegistrationFeeReceiptAudits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    ReceiptId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PerformedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PerformedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationFeeReceiptAudits", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationFeeReceiptAudits_TenantId_BranchId_ReceiptId",
                table: "RegistrationFeeReceiptAudits",
                columns: new[] { "TenantId", "BranchId", "ReceiptId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistrationFeeReceiptAudits");

            migrationBuilder.DropColumn(
                name: "IsRefunded",
                table: "RegistrationFeeReceipts");

            migrationBuilder.DropColumn(
                name: "ReceiptStatus",
                table: "RegistrationFeeReceipts");

            migrationBuilder.DropColumn(
                name: "RefundReason",
                table: "RegistrationFeeReceipts");

            migrationBuilder.DropColumn(
                name: "RefundedAmount",
                table: "RegistrationFeeReceipts");

            migrationBuilder.DropColumn(
                name: "RefundedBy",
                table: "RegistrationFeeReceipts");

            migrationBuilder.DropColumn(
                name: "RefundedOn",
                table: "RegistrationFeeReceipts");
        }
    }
}
