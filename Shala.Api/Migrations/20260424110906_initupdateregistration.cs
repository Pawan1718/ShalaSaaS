using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shala.Api.Migrations
{
    /// <inheritdoc />
    public partial class initupdateregistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentStatus",
                table: "StudentRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CancelReason",
                table: "RegistrationFeeReceipts",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelledBy",
                table: "RegistrationFeeReceipts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledOn",
                table: "RegistrationFeeReceipts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCancelled",
                table: "RegistrationFeeReceipts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_StudentRegistrations_TenantId_BranchId_RegistrationNo",
                table: "StudentRegistrations",
                columns: new[] { "TenantId", "BranchId", "RegistrationNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationFeeReceipts_TenantId_BranchId_ReceiptNo",
                table: "RegistrationFeeReceipts",
                columns: new[] { "TenantId", "BranchId", "ReceiptNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationFeeReceipts_TenantId_BranchId_RegistrationId_IsCancelled",
                table: "RegistrationFeeReceipts",
                columns: new[] { "TenantId", "BranchId", "RegistrationId", "IsCancelled" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StudentRegistrations_TenantId_BranchId_RegistrationNo",
                table: "StudentRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_RegistrationFeeReceipts_TenantId_BranchId_ReceiptNo",
                table: "RegistrationFeeReceipts");

            migrationBuilder.DropIndex(
                name: "IX_RegistrationFeeReceipts_TenantId_BranchId_RegistrationId_IsCancelled",
                table: "RegistrationFeeReceipts");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "StudentRegistrations");

            migrationBuilder.DropColumn(
                name: "CancelReason",
                table: "RegistrationFeeReceipts");

            migrationBuilder.DropColumn(
                name: "CancelledBy",
                table: "RegistrationFeeReceipts");

            migrationBuilder.DropColumn(
                name: "CancelledOn",
                table: "RegistrationFeeReceipts");

            migrationBuilder.DropColumn(
                name: "IsCancelled",
                table: "RegistrationFeeReceipts");
        }
    }
}
