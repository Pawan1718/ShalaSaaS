using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shala.Api.Migrations
{
    /// <inheritdoc />
    public partial class initupdateaccessbranch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserBranchAccesses_Branches_BranchId",
                table: "UserBranchAccesses");

            migrationBuilder.DropIndex(
                name: "IX_UserBranchAccesses_UserId_BranchId",
                table: "UserBranchAccesses");

            migrationBuilder.DropIndex(
                name: "IX_StudentAdmissions_TenantId_BranchId_AdmissionNo",
                table: "StudentAdmissions");

            migrationBuilder.AlterColumn<int>(
                name: "BranchId",
                table: "UserBranchAccesses",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "UserBranchAccesses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "HasAllBranchesAccess",
                table: "UserBranchAccesses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "UserBranchAccesses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "UserBranchAccesses",
                type: "datetime2",
                nullable: true);

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
                name: "IX_UserBranchAccesses_TenantId_UserId",
                table: "UserBranchAccesses",
                columns: new[] { "TenantId", "UserId" },
                unique: true,
                filter: "[HasAllBranchesAccess] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_UserBranchAccesses_TenantId_UserId_BranchId",
                table: "UserBranchAccesses",
                columns: new[] { "TenantId", "UserId", "BranchId" },
                unique: true,
                filter: "[BranchId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserBranchAccesses_TenantId_UserId_HasAllBranchesAccess",
                table: "UserBranchAccesses",
                columns: new[] { "TenantId", "UserId", "HasAllBranchesAccess" });

            migrationBuilder.CreateIndex(
                name: "IX_UserBranchAccesses_TenantId_UserId_IsActive",
                table: "UserBranchAccesses",
                columns: new[] { "TenantId", "UserId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_UserBranchAccesses_UserId",
                table: "UserBranchAccesses",
                column: "UserId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_UserBranchAccesses_Branches_BranchId",
                table: "UserBranchAccesses",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserBranchAccesses_Branches_BranchId",
                table: "UserBranchAccesses");

            migrationBuilder.DropTable(
                name: "AdmissionNumberCounters");

            migrationBuilder.DropIndex(
                name: "IX_UserBranchAccesses_TenantId_UserId",
                table: "UserBranchAccesses");

            migrationBuilder.DropIndex(
                name: "IX_UserBranchAccesses_TenantId_UserId_BranchId",
                table: "UserBranchAccesses");

            migrationBuilder.DropIndex(
                name: "IX_UserBranchAccesses_TenantId_UserId_HasAllBranchesAccess",
                table: "UserBranchAccesses");

            migrationBuilder.DropIndex(
                name: "IX_UserBranchAccesses_TenantId_UserId_IsActive",
                table: "UserBranchAccesses");

            migrationBuilder.DropIndex(
                name: "IX_UserBranchAccesses_UserId",
                table: "UserBranchAccesses");

            migrationBuilder.DropIndex(
                name: "IX_StudentAdmissions_TenantId_BranchId_AdmissionNo",
                table: "StudentAdmissions");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "UserBranchAccesses");

            migrationBuilder.DropColumn(
                name: "HasAllBranchesAccess",
                table: "UserBranchAccesses");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "UserBranchAccesses");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "UserBranchAccesses");

            migrationBuilder.AlterColumn<int>(
                name: "BranchId",
                table: "UserBranchAccesses",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserBranchAccesses_UserId_BranchId",
                table: "UserBranchAccesses",
                columns: new[] { "UserId", "BranchId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentAdmissions_TenantId_BranchId_AdmissionNo",
                table: "StudentAdmissions",
                columns: new[] { "TenantId", "BranchId", "AdmissionNo" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserBranchAccesses_Branches_BranchId",
                table: "UserBranchAccesses",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
