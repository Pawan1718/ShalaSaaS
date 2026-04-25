using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shala.Api.Migrations
{
    /// <inheritdoc />
    public partial class initoptimizecode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistrationFeeConfigurations");

            migrationBuilder.DropTable(
                name: "RegistrationFeeReceiptAudits");

            migrationBuilder.DropTable(
                name: "RegistrationFeeReceipts");

            migrationBuilder.DropTable(
                name: "RegistrationProspectusConfigurations");

            migrationBuilder.DropTable(
                name: "RegistrationReceiptConfigurations");

            migrationBuilder.DropTable(
                name: "StudentRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_TenantId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "Mobile",
                table: "Students",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Students",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Students",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Students",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AdmissionNo",
                table: "StudentAdmissions",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Mobile",
                table: "Guardians",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyStockLedgers_TenantId_BranchId_ReferenceType",
                table: "SupplyStockLedgers",
                columns: new[] { "TenantId", "BranchId", "ReferenceType" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplyItems_TenantId_BranchId_CurrentStock",
                table: "SupplyItems",
                columns: new[] { "TenantId", "BranchId", "CurrentStock" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplyItems_TenantId_BranchId_IsActive",
                table: "SupplyItems",
                columns: new[] { "TenantId", "BranchId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentSupplyPayments_TenantId_BranchId_StudentSupplyIssueId",
                table: "StudentSupplyPayments",
                columns: new[] { "TenantId", "BranchId", "StudentSupplyIssueId" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentSupplyIssues_TenantId_BranchId_DueAmount",
                table: "StudentSupplyIssues",
                columns: new[] { "TenantId", "BranchId", "DueAmount" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentSupplyIssues_TenantId_BranchId_IsCancelled",
                table: "StudentSupplyIssues",
                columns: new[] { "TenantId", "BranchId", "IsCancelled" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentSupplyIssues_TenantId_BranchId_IssueDate",
                table: "StudentSupplyIssues",
                columns: new[] { "TenantId", "BranchId", "IssueDate" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentSupplyIssues_TenantId_BranchId_StudentAdmissionId",
                table: "StudentSupplyIssues",
                columns: new[] { "TenantId", "BranchId", "StudentAdmissionId" });

            migrationBuilder.CreateIndex(
                name: "IX_Students_TenantId_BranchId",
                table: "Students",
                columns: new[] { "TenantId", "BranchId" });

            migrationBuilder.CreateIndex(
                name: "IX_Students_TenantId_BranchId_CreatedAt",
                table: "Students",
                columns: new[] { "TenantId", "BranchId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Students_TenantId_BranchId_Email",
                table: "Students",
                columns: new[] { "TenantId", "BranchId", "Email" });

            migrationBuilder.CreateIndex(
                name: "IX_Students_TenantId_BranchId_FirstName",
                table: "Students",
                columns: new[] { "TenantId", "BranchId", "FirstName" });

            migrationBuilder.CreateIndex(
                name: "IX_Students_TenantId_BranchId_LastName",
                table: "Students",
                columns: new[] { "TenantId", "BranchId", "LastName" });

            migrationBuilder.CreateIndex(
                name: "IX_Students_TenantId_BranchId_Mobile",
                table: "Students",
                columns: new[] { "TenantId", "BranchId", "Mobile" });

            migrationBuilder.CreateIndex(
                name: "IX_Students_TenantId_BranchId_Status",
                table: "Students",
                columns: new[] { "TenantId", "BranchId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentFeeLedgers_TenantId_BranchId_EntryDate",
                table: "StudentFeeLedgers",
                columns: new[] { "TenantId", "BranchId", "EntryDate" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentFeeLedgers_TenantId_BranchId_EntryType",
                table: "StudentFeeLedgers",
                columns: new[] { "TenantId", "BranchId", "EntryType" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentFeeAssignments_TenantId_BranchId_FeeStructureId",
                table: "StudentFeeAssignments",
                columns: new[] { "TenantId", "BranchId", "FeeStructureId" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentFeeAssignments_TenantId_BranchId_IsActive",
                table: "StudentFeeAssignments",
                columns: new[] { "TenantId", "BranchId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentFeeAssignments_TenantId_BranchId_StudentId",
                table: "StudentFeeAssignments",
                columns: new[] { "TenantId", "BranchId", "StudentId" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentDocumentSuggestions_TenantId_BranchId_SuggestionType",
                table: "StudentDocumentSuggestions",
                columns: new[] { "TenantId", "BranchId", "SuggestionType" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentDocuments_TenantId_BranchId_CreatedAt",
                table: "StudentDocuments",
                columns: new[] { "TenantId", "BranchId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentDocuments_TenantId_BranchId_DocumentModelId",
                table: "StudentDocuments",
                columns: new[] { "TenantId", "BranchId", "DocumentModelId" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentDocumentFieldMatches_MatchStatus",
                table: "StudentDocumentFieldMatches",
                column: "MatchStatus");

            migrationBuilder.CreateIndex(
                name: "IX_StudentDocumentAnalyses_TenantId_BranchId_AnalysisStatus",
                table: "StudentDocumentAnalyses",
                columns: new[] { "TenantId", "BranchId", "AnalysisStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentCharges_TenantId_BranchId_DueDate",
                table: "StudentCharges",
                columns: new[] { "TenantId", "BranchId", "DueDate" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentCharges_TenantId_BranchId_FeeHeadId",
                table: "StudentCharges",
                columns: new[] { "TenantId", "BranchId", "FeeHeadId" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentCharges_TenantId_BranchId_IsCancelled",
                table: "StudentCharges",
                columns: new[] { "TenantId", "BranchId", "IsCancelled" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentCharges_TenantId_BranchId_StudentAdmissionId",
                table: "StudentCharges",
                columns: new[] { "TenantId", "BranchId", "StudentAdmissionId" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentCharges_TenantId_BranchId_StudentFeeAssignmentId",
                table: "StudentCharges",
                columns: new[] { "TenantId", "BranchId", "StudentFeeAssignmentId" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentCharges_TenantId_BranchId_StudentId",
                table: "StudentCharges",
                columns: new[] { "TenantId", "BranchId", "StudentId" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentAdmissions_TenantId_BranchId_AdmissionNo",
                table: "StudentAdmissions",
                columns: new[] { "TenantId", "BranchId", "AdmissionNo" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentAdmissions_TenantId_BranchId_IsCurrent",
                table: "StudentAdmissions",
                columns: new[] { "TenantId", "BranchId", "IsCurrent" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentAdmissions_TenantId_BranchId_Status",
                table: "StudentAdmissions",
                columns: new[] { "TenantId", "BranchId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentAdmissions_TenantId_BranchId_StudentId",
                table: "StudentAdmissions",
                columns: new[] { "TenantId", "BranchId", "StudentId" });

            migrationBuilder.CreateIndex(
                name: "IX_Sections_TenantId_BranchId_AcademicClassId",
                table: "Sections",
                columns: new[] { "TenantId", "BranchId", "AcademicClassId" });

            migrationBuilder.CreateIndex(
                name: "IX_Sections_TenantId_BranchId_IsActive",
                table: "Sections",
                columns: new[] { "TenantId", "BranchId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Guardians_TenantId_IsPrimary",
                table: "Guardians",
                columns: new[] { "TenantId", "IsPrimary" });

            migrationBuilder.CreateIndex(
                name: "IX_Guardians_TenantId_Mobile",
                table: "Guardians",
                columns: new[] { "TenantId", "Mobile" });

            migrationBuilder.CreateIndex(
                name: "IX_Guardians_TenantId_StudentId",
                table: "Guardians",
                columns: new[] { "TenantId", "StudentId" });

            migrationBuilder.CreateIndex(
                name: "IX_FeeStructures_TenantId_BranchId_AcademicClassId",
                table: "FeeStructures",
                columns: new[] { "TenantId", "BranchId", "AcademicClassId" });

            migrationBuilder.CreateIndex(
                name: "IX_FeeStructures_TenantId_BranchId_AcademicYearId",
                table: "FeeStructures",
                columns: new[] { "TenantId", "BranchId", "AcademicYearId" });

            migrationBuilder.CreateIndex(
                name: "IX_FeeStructures_TenantId_BranchId_IsActive",
                table: "FeeStructures",
                columns: new[] { "TenantId", "BranchId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_FeeStructureItems_IsActive",
                table: "FeeStructureItems",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_FeeReceipts_TenantId_BranchId_IsCancelled",
                table: "FeeReceipts",
                columns: new[] { "TenantId", "BranchId", "IsCancelled" });

            migrationBuilder.CreateIndex(
                name: "IX_FeeReceipts_TenantId_BranchId_ReceiptDate",
                table: "FeeReceipts",
                columns: new[] { "TenantId", "BranchId", "ReceiptDate" });

            migrationBuilder.CreateIndex(
                name: "IX_FeeReceipts_TenantId_BranchId_StudentAdmissionId",
                table: "FeeReceipts",
                columns: new[] { "TenantId", "BranchId", "StudentAdmissionId" });

            migrationBuilder.CreateIndex(
                name: "IX_FeeReceipts_TenantId_BranchId_StudentId",
                table: "FeeReceipts",
                columns: new[] { "TenantId", "BranchId", "StudentId" });

            migrationBuilder.CreateIndex(
                name: "IX_FeeHeads_TenantId_BranchId_Code",
                table: "FeeHeads",
                columns: new[] { "TenantId", "BranchId", "Code" });

            migrationBuilder.CreateIndex(
                name: "IX_FeeHeads_TenantId_BranchId_IsActive",
                table: "FeeHeads",
                columns: new[] { "TenantId", "BranchId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentModels_TenantId_BranchId_Name",
                table: "DocumentModels",
                columns: new[] { "TenantId", "BranchId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_BranchDocumentProfiles_TenantId_BranchId",
                table: "BranchDocumentProfiles",
                columns: new[] { "TenantId", "BranchId" });

            migrationBuilder.CreateIndex(
                name: "IX_BranchDocumentProfiles_TenantId_BranchId_IsActive",
                table: "BranchDocumentProfiles",
                columns: new[] { "TenantId", "BranchId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_TenantId_BranchId",
                table: "AspNetUsers",
                columns: new[] { "TenantId", "BranchId" });

            migrationBuilder.CreateIndex(
                name: "IX_AcademicYears_TenantId_IsActive",
                table: "AcademicYears",
                columns: new[] { "TenantId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_AcademicClasses_TenantId_IsActive",
                table: "AcademicClasses",
                columns: new[] { "TenantId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_AcademicClasses_TenantId_Sequence",
                table: "AcademicClasses",
                columns: new[] { "TenantId", "Sequence" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SupplyStockLedgers_TenantId_BranchId_ReferenceType",
                table: "SupplyStockLedgers");

            migrationBuilder.DropIndex(
                name: "IX_SupplyItems_TenantId_BranchId_CurrentStock",
                table: "SupplyItems");

            migrationBuilder.DropIndex(
                name: "IX_SupplyItems_TenantId_BranchId_IsActive",
                table: "SupplyItems");

            migrationBuilder.DropIndex(
                name: "IX_StudentSupplyPayments_TenantId_BranchId_StudentSupplyIssueId",
                table: "StudentSupplyPayments");

            migrationBuilder.DropIndex(
                name: "IX_StudentSupplyIssues_TenantId_BranchId_DueAmount",
                table: "StudentSupplyIssues");

            migrationBuilder.DropIndex(
                name: "IX_StudentSupplyIssues_TenantId_BranchId_IsCancelled",
                table: "StudentSupplyIssues");

            migrationBuilder.DropIndex(
                name: "IX_StudentSupplyIssues_TenantId_BranchId_IssueDate",
                table: "StudentSupplyIssues");

            migrationBuilder.DropIndex(
                name: "IX_StudentSupplyIssues_TenantId_BranchId_StudentAdmissionId",
                table: "StudentSupplyIssues");

            migrationBuilder.DropIndex(
                name: "IX_Students_TenantId_BranchId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_TenantId_BranchId_CreatedAt",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_TenantId_BranchId_Email",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_TenantId_BranchId_FirstName",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_TenantId_BranchId_LastName",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_TenantId_BranchId_Mobile",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_TenantId_BranchId_Status",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_StudentFeeLedgers_TenantId_BranchId_EntryDate",
                table: "StudentFeeLedgers");

            migrationBuilder.DropIndex(
                name: "IX_StudentFeeLedgers_TenantId_BranchId_EntryType",
                table: "StudentFeeLedgers");

            migrationBuilder.DropIndex(
                name: "IX_StudentFeeAssignments_TenantId_BranchId_FeeStructureId",
                table: "StudentFeeAssignments");

            migrationBuilder.DropIndex(
                name: "IX_StudentFeeAssignments_TenantId_BranchId_IsActive",
                table: "StudentFeeAssignments");

            migrationBuilder.DropIndex(
                name: "IX_StudentFeeAssignments_TenantId_BranchId_StudentId",
                table: "StudentFeeAssignments");

            migrationBuilder.DropIndex(
                name: "IX_StudentDocumentSuggestions_TenantId_BranchId_SuggestionType",
                table: "StudentDocumentSuggestions");

            migrationBuilder.DropIndex(
                name: "IX_StudentDocuments_TenantId_BranchId_CreatedAt",
                table: "StudentDocuments");

            migrationBuilder.DropIndex(
                name: "IX_StudentDocuments_TenantId_BranchId_DocumentModelId",
                table: "StudentDocuments");

            migrationBuilder.DropIndex(
                name: "IX_StudentDocumentFieldMatches_MatchStatus",
                table: "StudentDocumentFieldMatches");

            migrationBuilder.DropIndex(
                name: "IX_StudentDocumentAnalyses_TenantId_BranchId_AnalysisStatus",
                table: "StudentDocumentAnalyses");

            migrationBuilder.DropIndex(
                name: "IX_StudentCharges_TenantId_BranchId_DueDate",
                table: "StudentCharges");

            migrationBuilder.DropIndex(
                name: "IX_StudentCharges_TenantId_BranchId_FeeHeadId",
                table: "StudentCharges");

            migrationBuilder.DropIndex(
                name: "IX_StudentCharges_TenantId_BranchId_IsCancelled",
                table: "StudentCharges");

            migrationBuilder.DropIndex(
                name: "IX_StudentCharges_TenantId_BranchId_StudentAdmissionId",
                table: "StudentCharges");

            migrationBuilder.DropIndex(
                name: "IX_StudentCharges_TenantId_BranchId_StudentFeeAssignmentId",
                table: "StudentCharges");

            migrationBuilder.DropIndex(
                name: "IX_StudentCharges_TenantId_BranchId_StudentId",
                table: "StudentCharges");

            migrationBuilder.DropIndex(
                name: "IX_StudentAdmissions_TenantId_BranchId_AdmissionNo",
                table: "StudentAdmissions");

            migrationBuilder.DropIndex(
                name: "IX_StudentAdmissions_TenantId_BranchId_IsCurrent",
                table: "StudentAdmissions");

            migrationBuilder.DropIndex(
                name: "IX_StudentAdmissions_TenantId_BranchId_Status",
                table: "StudentAdmissions");

            migrationBuilder.DropIndex(
                name: "IX_StudentAdmissions_TenantId_BranchId_StudentId",
                table: "StudentAdmissions");

            migrationBuilder.DropIndex(
                name: "IX_Sections_TenantId_BranchId_AcademicClassId",
                table: "Sections");

            migrationBuilder.DropIndex(
                name: "IX_Sections_TenantId_BranchId_IsActive",
                table: "Sections");

            migrationBuilder.DropIndex(
                name: "IX_Guardians_TenantId_IsPrimary",
                table: "Guardians");

            migrationBuilder.DropIndex(
                name: "IX_Guardians_TenantId_Mobile",
                table: "Guardians");

            migrationBuilder.DropIndex(
                name: "IX_Guardians_TenantId_StudentId",
                table: "Guardians");

            migrationBuilder.DropIndex(
                name: "IX_FeeStructures_TenantId_BranchId_AcademicClassId",
                table: "FeeStructures");

            migrationBuilder.DropIndex(
                name: "IX_FeeStructures_TenantId_BranchId_AcademicYearId",
                table: "FeeStructures");

            migrationBuilder.DropIndex(
                name: "IX_FeeStructures_TenantId_BranchId_IsActive",
                table: "FeeStructures");

            migrationBuilder.DropIndex(
                name: "IX_FeeStructureItems_IsActive",
                table: "FeeStructureItems");

            migrationBuilder.DropIndex(
                name: "IX_FeeReceipts_TenantId_BranchId_IsCancelled",
                table: "FeeReceipts");

            migrationBuilder.DropIndex(
                name: "IX_FeeReceipts_TenantId_BranchId_ReceiptDate",
                table: "FeeReceipts");

            migrationBuilder.DropIndex(
                name: "IX_FeeReceipts_TenantId_BranchId_StudentAdmissionId",
                table: "FeeReceipts");

            migrationBuilder.DropIndex(
                name: "IX_FeeReceipts_TenantId_BranchId_StudentId",
                table: "FeeReceipts");

            migrationBuilder.DropIndex(
                name: "IX_FeeHeads_TenantId_BranchId_Code",
                table: "FeeHeads");

            migrationBuilder.DropIndex(
                name: "IX_FeeHeads_TenantId_BranchId_IsActive",
                table: "FeeHeads");

            migrationBuilder.DropIndex(
                name: "IX_DocumentModels_TenantId_BranchId_Name",
                table: "DocumentModels");

            migrationBuilder.DropIndex(
                name: "IX_BranchDocumentProfiles_TenantId_BranchId",
                table: "BranchDocumentProfiles");

            migrationBuilder.DropIndex(
                name: "IX_BranchDocumentProfiles_TenantId_BranchId_IsActive",
                table: "BranchDocumentProfiles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_TenantId_BranchId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AcademicYears_TenantId_IsActive",
                table: "AcademicYears");

            migrationBuilder.DropIndex(
                name: "IX_AcademicClasses_TenantId_IsActive",
                table: "AcademicClasses");

            migrationBuilder.DropIndex(
                name: "IX_AcademicClasses_TenantId_Sequence",
                table: "AcademicClasses");

            migrationBuilder.AlterColumn<string>(
                name: "Mobile",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AdmissionNo",
                table: "StudentAdmissions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Mobile",
                table: "Guardians",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateTable(
                name: "RegistrationFeeConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsRegistrationFeeMandatory = table.Column<bool>(type: "bit", nullable: false),
                    IsRegistrationModuleEnabled = table.Column<bool>(type: "bit", nullable: false),
                    RegistrationFeeAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RegistrationFeeHeadId = table.Column<int>(type: "int", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationFeeConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegistrationFeeReceiptAudits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PerformedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PerformedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ReceiptId = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationFeeReceiptAudits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegistrationFeeReceipts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    CancelReason = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CancelledBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CancelledOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCancelled = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsProspectusIncluded = table.Column<bool>(type: "bit", nullable: false),
                    IsRefunded = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsRegistrationReceipt = table.Column<bool>(type: "bit", nullable: false),
                    PaymentMode = table.Column<int>(type: "int", nullable: false),
                    ProspectusAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProspectusLabel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReceiptDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceiptNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ReceiptStatus = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    RefundReason = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RefundedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    RefundedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RefundedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RegistrationAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RegistrationId = table.Column<int>(type: "int", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StudentAdmissionId = table.Column<int>(type: "int", nullable: true),
                    StudentId = table.Column<int>(type: "int", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationFeeReceipts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrationFeeReceipts_StudentAdmissions_StudentAdmissionId",
                        column: x => x.StudentAdmissionId,
                        principalTable: "StudentAdmissions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RegistrationFeeReceipts_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RegistrationProspectusConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IncludeProspectus = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsProspectusMandatory = table.Column<bool>(type: "bit", nullable: false),
                    ProspectusAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProspectusDisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProspectusFeeHeadId = table.Column<int>(type: "int", nullable: true),
                    ShowProspectusInReceipt = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationProspectusConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegistrationReceiptConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AllowDownloadReceipt = table.Column<bool>(type: "bit", nullable: false),
                    AllowPrintReceipt = table.Column<bool>(type: "bit", nullable: false),
                    AutoPrintAfterSave = table.Column<bool>(type: "bit", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ReceiptFooterNote = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReceiptTitle = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ShowAmountInWords = table.Column<bool>(type: "bit", nullable: false),
                    ShowFeeHeadInReceipt = table.Column<bool>(type: "bit", nullable: false),
                    ShowStudentDetailsInReceipt = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationReceiptConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentRegistrations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    ConvertedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ConvertedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FeePaid = table.Column<bool>(type: "bit", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: true),
                    GuardianName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    InterestedClassId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PaymentStatus = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RegistrationNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StudentAdmissionId = table.Column<int>(type: "int", nullable: true),
                    StudentId = table.Column<int>(type: "int", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentRegistrations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_TenantId",
                table: "AspNetUsers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationFeeConfigurations_TenantId_BranchId",
                table: "RegistrationFeeConfigurations",
                columns: new[] { "TenantId", "BranchId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationFeeReceiptAudits_TenantId_BranchId_ReceiptId",
                table: "RegistrationFeeReceiptAudits",
                columns: new[] { "TenantId", "BranchId", "ReceiptId" });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationFeeReceipts_StudentAdmissionId",
                table: "RegistrationFeeReceipts",
                column: "StudentAdmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationFeeReceipts_StudentId",
                table: "RegistrationFeeReceipts",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationFeeReceipts_TenantId_BranchId_ReceiptNo",
                table: "RegistrationFeeReceipts",
                columns: new[] { "TenantId", "BranchId", "ReceiptNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationFeeReceipts_TenantId_BranchId_RegistrationId_IsCancelled",
                table: "RegistrationFeeReceipts",
                columns: new[] { "TenantId", "BranchId", "RegistrationId", "IsCancelled" });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationProspectusConfigurations_TenantId_BranchId",
                table: "RegistrationProspectusConfigurations",
                columns: new[] { "TenantId", "BranchId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationReceiptConfigurations_TenantId_BranchId",
                table: "RegistrationReceiptConfigurations",
                columns: new[] { "TenantId", "BranchId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentRegistrations_TenantId_BranchId_RegistrationNo",
                table: "StudentRegistrations",
                columns: new[] { "TenantId", "BranchId", "RegistrationNo" },
                unique: true);
        }
    }
}
