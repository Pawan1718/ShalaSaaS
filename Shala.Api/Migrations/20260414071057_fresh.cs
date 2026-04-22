using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shala.Api.Migrations
{
    /// <inheritdoc />
    public partial class fresh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AcademicClasses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicClasses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AcademicYears",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicYears", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AcademicYearSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    StartMonth = table.Column<int>(type: "int", nullable: false),
                    StartDay = table.Column<int>(type: "int", nullable: false),
                    EndMonth = table.Column<int>(type: "int", nullable: false),
                    EndDay = table.Column<int>(type: "int", nullable: false),
                    AutoCreateNextYear = table.Column<bool>(type: "bit", nullable: false),
                    CreateBeforeDays = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicYearSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BranchDocumentProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LogoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AddressLine = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PrimaryColorHex = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ReceiptTitle = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ReceiptFooterNote = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SignatureLabel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ShowLogo = table.Column<bool>(type: "bit", nullable: false),
                    ShowAddress = table.Column<bool>(type: "bit", nullable: false),
                    ShowContactInfo = table.Column<bool>(type: "bit", nullable: false),
                    ShowStudentDetails = table.Column<bool>(type: "bit", nullable: false),
                    ShowFeeBreakup = table.Column<bool>(type: "bit", nullable: false),
                    ShowAmountInWords = table.Column<bool>(type: "bit", nullable: false),
                    ShowSignature = table.Column<bool>(type: "bit", nullable: false),
                    AllowPrintReceipt = table.Column<bool>(type: "bit", nullable: false),
                    AllowDownloadReceipt = table.Column<bool>(type: "bit", nullable: false),
                    AutoPrintAfterSave = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchDocumentProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    IsAiValidationEnabled = table.Column<bool>(type: "bit", nullable: false),
                    BlockAdmissionOnMismatch = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    AllowedFileTypes = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    MaxFileSizeInKb = table.Column<int>(type: "int", nullable: true),
                    RequiredFieldsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FeeHeads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsRegistrationFee = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeeHeads", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FeeStructures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    AcademicClassId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeeStructures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegistrationFeeConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    RegistrationFeeAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsRegistrationFeeMandatory = table.Column<bool>(type: "bit", nullable: false),
                    RegistrationFeeHeadId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationFeeConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegistrationProspectusConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    IncludeProspectus = table.Column<bool>(type: "bit", nullable: false),
                    ProspectusAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsProspectusMandatory = table.Column<bool>(type: "bit", nullable: false),
                    ProspectusFeeHeadId = table.Column<int>(type: "int", nullable: true),
                    ProspectusDisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ShowProspectusInReceipt = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    AllowPrintReceipt = table.Column<bool>(type: "bit", nullable: false),
                    AllowDownloadReceipt = table.Column<bool>(type: "bit", nullable: false),
                    AutoPrintAfterSave = table.Column<bool>(type: "bit", nullable: false),
                    ReceiptTitle = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ReceiptFooterNote = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ShowStudentDetailsInReceipt = table.Column<bool>(type: "bit", nullable: false),
                    ShowFeeHeadInReceipt = table.Column<bool>(type: "bit", nullable: false),
                    ShowAmountInWords = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationReceiptConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RollNumberSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    AutoGenerate = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    AllowManualOverride = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ResetPerAcademicYear = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ResetPerClass = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ResetPerSection = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    StartFrom = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    NumberPadding = table.Column<int>(type: "int", nullable: false, defaultValue: 3),
                    Prefix = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Format = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RollNumberSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentRegistrations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    RegistrationNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: true),
                    GuardianName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    InterestedClassId = table.Column<int>(type: "int", nullable: false),
                    FeePaid = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: true),
                    StudentAdmissionId = table.Column<int>(type: "int", nullable: true),
                    ConvertedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConvertedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentRegistrations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AadhaarNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BloodGroup = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BusinessCategory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubscriptionPlan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Subdomain = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    AcademicClassId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sections_AcademicClasses_AcademicClassId",
                        column: x => x.AcademicClassId,
                        principalTable: "AcademicClasses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeeStructureItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FeeStructureId = table.Column<int>(type: "int", nullable: false),
                    FeeHeadId = table.Column<int>(type: "int", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FrequencyType = table.Column<int>(type: "int", nullable: false),
                    StartMonth = table.Column<int>(type: "int", nullable: true),
                    EndMonth = table.Column<int>(type: "int", nullable: true),
                    DueDay = table.Column<int>(type: "int", nullable: true),
                    ApplyType = table.Column<int>(type: "int", nullable: false),
                    IsOptional = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FeeHeadId1 = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeeStructureItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeeStructureItems_FeeHeads_FeeHeadId",
                        column: x => x.FeeHeadId,
                        principalTable: "FeeHeads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FeeStructureItems_FeeHeads_FeeHeadId1",
                        column: x => x.FeeHeadId1,
                        principalTable: "FeeHeads",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FeeStructureItems_FeeStructures_FeeStructureId",
                        column: x => x.FeeStructureId,
                        principalTable: "FeeStructures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Guardians",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RelationType = table.Column<int>(type: "int", nullable: false),
                    Mobile = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Occupation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guardians", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Guardians_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    StudentRegistrationId = table.Column<int>(type: "int", nullable: true),
                    StudentAdmissionId = table.Column<int>(type: "int", nullable: true),
                    DocumentModelId = table.Column<int>(type: "int", nullable: true),
                    DocumentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MimeType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentDocuments_DocumentModels_DocumentModelId",
                        column: x => x.DocumentModelId,
                        principalTable: "DocumentModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentDocuments_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressLine1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressLine2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pincode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrincipalName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsMainBranch = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Branches_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentAdmissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    AcademicClassId = table.Column<int>(type: "int", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: true),
                    AdmissionNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RollNo = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AdmissionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAdmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentAdmissions_AcademicClasses_AcademicClassId",
                        column: x => x.AcademicClassId,
                        principalTable: "AcademicClasses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentAdmissions_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentAdmissions_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentAdmissions_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentDocumentAnalyses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    StudentDocumentId = table.Column<int>(type: "int", nullable: false),
                    ExtractedText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExtractedJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DetectedDocumentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OcrConfidence = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    AiConfidence = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    AnalysisStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentDocumentAnalyses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentDocumentAnalyses_StudentDocuments_StudentDocumentId",
                        column: x => x.StudentDocumentId,
                        principalTable: "StudentDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentDocumentSuggestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    StudentDocumentId = table.Column<int>(type: "int", nullable: false),
                    SuggestionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    SuggestedValue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ConfidenceScore = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    IsApplied = table.Column<bool>(type: "bit", nullable: false),
                    IsDismissed = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentDocumentSuggestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentDocumentSuggestions_StudentDocuments_StudentDocumentId",
                        column: x => x.StudentDocumentId,
                        principalTable: "StudentDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    BranchId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FeeReceipts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    StudentAdmissionId = table.Column<int>(type: "int", nullable: true),
                    ReceiptNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ReceiptDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentMode = table.Column<int>(type: "int", nullable: false),
                    TransactionReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsCancelled = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CancelledOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelReason = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeeReceipts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeeReceipts_StudentAdmissions_StudentAdmissionId",
                        column: x => x.StudentAdmissionId,
                        principalTable: "StudentAdmissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FeeReceipts_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RegistrationFeeReceipts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    RegistrationId = table.Column<int>(type: "int", nullable: true),
                    ReceiptNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ReceiptDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentMode = table.Column<int>(type: "int", nullable: false),
                    TransactionReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RegistrationAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsProspectusIncluded = table.Column<bool>(type: "bit", nullable: false),
                    ProspectusAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProspectusLabel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsRegistrationReceipt = table.Column<bool>(type: "bit", nullable: false),
                    StudentAdmissionId = table.Column<int>(type: "int", nullable: true),
                    StudentId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                name: "StudentFeeAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    StudentAdmissionId = table.Column<int>(type: "int", nullable: false),
                    FeeStructureId = table.Column<int>(type: "int", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AdditionalChargeAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FeeStructureId1 = table.Column<int>(type: "int", nullable: true),
                    StudentAdmissionId1 = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentFeeAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentFeeAssignments_FeeStructures_FeeStructureId",
                        column: x => x.FeeStructureId,
                        principalTable: "FeeStructures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentFeeAssignments_FeeStructures_FeeStructureId1",
                        column: x => x.FeeStructureId1,
                        principalTable: "FeeStructures",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentFeeAssignments_StudentAdmissions_StudentAdmissionId",
                        column: x => x.StudentAdmissionId,
                        principalTable: "StudentAdmissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentFeeAssignments_StudentAdmissions_StudentAdmissionId1",
                        column: x => x.StudentAdmissionId1,
                        principalTable: "StudentAdmissions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentFeeAssignments_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StudentDocumentFieldMatches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    StudentDocumentAnalysisId = table.Column<int>(type: "int", nullable: false),
                    FieldName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DocumentValue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FormValue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MatchStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ConfidenceScore = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    IsCritical = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Suggestion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentDocumentFieldMatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentDocumentFieldMatches_StudentDocumentAnalyses_StudentDocumentAnalysisId",
                        column: x => x.StudentDocumentAnalysisId,
                        principalTable: "StudentDocumentAnalyses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserBranchAccesses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBranchAccesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserBranchAccesses_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserBranchAccesses_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentCharges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: true),
                    StudentAdmissionId = table.Column<int>(type: "int", nullable: true),
                    StudentFeeAssignmentId = table.Column<int>(type: "int", nullable: true),
                    FeeHeadId = table.Column<int>(type: "int", nullable: false),
                    ChargeLabel = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PeriodLabel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FineAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsRegistrationCharge = table.Column<bool>(type: "bit", nullable: false),
                    IsSettled = table.Column<bool>(type: "bit", nullable: false),
                    IsCancelled = table.Column<bool>(type: "bit", nullable: false),
                    FeeHeadId1 = table.Column<int>(type: "int", nullable: true),
                    StudentAdmissionId1 = table.Column<int>(type: "int", nullable: true),
                    StudentId1 = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentCharges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentCharges_FeeHeads_FeeHeadId",
                        column: x => x.FeeHeadId,
                        principalTable: "FeeHeads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentCharges_FeeHeads_FeeHeadId1",
                        column: x => x.FeeHeadId1,
                        principalTable: "FeeHeads",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentCharges_StudentAdmissions_StudentAdmissionId",
                        column: x => x.StudentAdmissionId,
                        principalTable: "StudentAdmissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentCharges_StudentAdmissions_StudentAdmissionId1",
                        column: x => x.StudentAdmissionId1,
                        principalTable: "StudentAdmissions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentCharges_StudentFeeAssignments_StudentFeeAssignmentId",
                        column: x => x.StudentFeeAssignmentId,
                        principalTable: "StudentFeeAssignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentCharges_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentCharges_Students_StudentId1",
                        column: x => x.StudentId1,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FeeReceiptAllocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FeeReceiptId = table.Column<int>(type: "int", nullable: false),
                    StudentChargeId = table.Column<int>(type: "int", nullable: false),
                    AllocatedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeeReceiptAllocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeeReceiptAllocations_FeeReceipts_FeeReceiptId",
                        column: x => x.FeeReceiptId,
                        principalTable: "FeeReceipts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FeeReceiptAllocations_StudentCharges_StudentChargeId",
                        column: x => x.StudentChargeId,
                        principalTable: "StudentCharges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcademicYears_TenantId_Name",
                table: "AcademicYears",
                columns: new[] { "TenantId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AcademicYearSettings_TenantId",
                table: "AcademicYearSettings",
                column: "TenantId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_BranchId",
                table: "AspNetUsers",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_TenantId",
                table: "AspNetUsers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_TenantId",
                table: "Branches",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentModels_TenantId_BranchId_Code",
                table: "DocumentModels",
                columns: new[] { "TenantId", "BranchId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentModels_TenantId_BranchId_IsActive",
                table: "DocumentModels",
                columns: new[] { "TenantId", "BranchId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_FeeHeads_TenantId_BranchId_Name",
                table: "FeeHeads",
                columns: new[] { "TenantId", "BranchId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeeReceiptAllocations_FeeReceiptId_StudentChargeId",
                table: "FeeReceiptAllocations",
                columns: new[] { "FeeReceiptId", "StudentChargeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeeReceiptAllocations_StudentChargeId",
                table: "FeeReceiptAllocations",
                column: "StudentChargeId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeReceipts_StudentAdmissionId",
                table: "FeeReceipts",
                column: "StudentAdmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeReceipts_StudentId",
                table: "FeeReceipts",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeReceipts_TenantId_BranchId_ReceiptNo",
                table: "FeeReceipts",
                columns: new[] { "TenantId", "BranchId", "ReceiptNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeeStructureItems_FeeHeadId",
                table: "FeeStructureItems",
                column: "FeeHeadId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeStructureItems_FeeHeadId1",
                table: "FeeStructureItems",
                column: "FeeHeadId1");

            migrationBuilder.CreateIndex(
                name: "IX_FeeStructureItems_FeeStructureId_FeeHeadId_Label",
                table: "FeeStructureItems",
                columns: new[] { "FeeStructureId", "FeeHeadId", "Label" });

            migrationBuilder.CreateIndex(
                name: "IX_FeeStructures_TenantId_BranchId_AcademicYearId_AcademicClassId_Name",
                table: "FeeStructures",
                columns: new[] { "TenantId", "BranchId", "AcademicYearId", "AcademicClassId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Guardians_StudentId",
                table: "Guardians",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationFeeConfigurations_TenantId_BranchId",
                table: "RegistrationFeeConfigurations",
                columns: new[] { "TenantId", "BranchId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationFeeReceipts_StudentAdmissionId",
                table: "RegistrationFeeReceipts",
                column: "StudentAdmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationFeeReceipts_StudentId",
                table: "RegistrationFeeReceipts",
                column: "StudentId");

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
                name: "IX_RollNumberSettings_TenantId",
                table: "RollNumberSettings",
                column: "TenantId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sections_AcademicClassId",
                table: "Sections",
                column: "AcademicClassId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAdmissions_AcademicClassId",
                table: "StudentAdmissions",
                column: "AcademicClassId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAdmissions_AcademicYearId",
                table: "StudentAdmissions",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAdmissions_SectionId",
                table: "StudentAdmissions",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAdmissions_StudentId",
                table: "StudentAdmissions",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAdmissions_TenantId_BranchId_AcademicYearId_AcademicClassId_SectionId",
                table: "StudentAdmissions",
                columns: new[] { "TenantId", "BranchId", "AcademicYearId", "AcademicClassId", "SectionId" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentAdmissions_TenantId_BranchId_AcademicYearId_AcademicClassId_SectionId_RollNo",
                table: "StudentAdmissions",
                columns: new[] { "TenantId", "BranchId", "AcademicYearId", "AcademicClassId", "SectionId", "RollNo" },
                unique: true,
                filter: "[RollNo] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCharges_FeeHeadId",
                table: "StudentCharges",
                column: "FeeHeadId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCharges_FeeHeadId1",
                table: "StudentCharges",
                column: "FeeHeadId1");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCharges_StudentAdmissionId",
                table: "StudentCharges",
                column: "StudentAdmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCharges_StudentAdmissionId1",
                table: "StudentCharges",
                column: "StudentAdmissionId1");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCharges_StudentFeeAssignmentId",
                table: "StudentCharges",
                column: "StudentFeeAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCharges_StudentId",
                table: "StudentCharges",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCharges_StudentId1",
                table: "StudentCharges",
                column: "StudentId1");

            migrationBuilder.CreateIndex(
                name: "IX_StudentDocumentAnalyses_StudentDocumentId",
                table: "StudentDocumentAnalyses",
                column: "StudentDocumentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentDocumentAnalyses_TenantId_BranchId_IsActive",
                table: "StudentDocumentAnalyses",
                columns: new[] { "TenantId", "BranchId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentDocumentFieldMatches_StudentDocumentAnalysisId_FieldName",
                table: "StudentDocumentFieldMatches",
                columns: new[] { "StudentDocumentAnalysisId", "FieldName" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentDocuments_DocumentModelId",
                table: "StudentDocuments",
                column: "DocumentModelId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentDocuments_StudentId",
                table: "StudentDocuments",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentDocuments_TenantId_BranchId_IsActive",
                table: "StudentDocuments",
                columns: new[] { "TenantId", "BranchId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentDocuments_TenantId_BranchId_Status",
                table: "StudentDocuments",
                columns: new[] { "TenantId", "BranchId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentDocuments_TenantId_BranchId_StudentId",
                table: "StudentDocuments",
                columns: new[] { "TenantId", "BranchId", "StudentId" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentDocumentSuggestions_StudentDocumentId",
                table: "StudentDocumentSuggestions",
                column: "StudentDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentDocumentSuggestions_TenantId_BranchId_IsActive",
                table: "StudentDocumentSuggestions",
                columns: new[] { "TenantId", "BranchId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentFeeAssignments_FeeStructureId",
                table: "StudentFeeAssignments",
                column: "FeeStructureId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentFeeAssignments_FeeStructureId1",
                table: "StudentFeeAssignments",
                column: "FeeStructureId1");

            migrationBuilder.CreateIndex(
                name: "IX_StudentFeeAssignments_StudentAdmissionId",
                table: "StudentFeeAssignments",
                column: "StudentAdmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentFeeAssignments_StudentAdmissionId1",
                table: "StudentFeeAssignments",
                column: "StudentAdmissionId1");

            migrationBuilder.CreateIndex(
                name: "IX_StudentFeeAssignments_StudentId",
                table: "StudentFeeAssignments",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentFeeAssignments_TenantId_BranchId_StudentAdmissionId",
                table: "StudentFeeAssignments",
                columns: new[] { "TenantId", "BranchId", "StudentAdmissionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserBranchAccesses_BranchId",
                table: "UserBranchAccesses",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBranchAccesses_UserId_BranchId",
                table: "UserBranchAccesses",
                columns: new[] { "UserId", "BranchId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcademicYearSettings");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BranchDocumentProfiles");

            migrationBuilder.DropTable(
                name: "FeeReceiptAllocations");

            migrationBuilder.DropTable(
                name: "FeeStructureItems");

            migrationBuilder.DropTable(
                name: "Guardians");

            migrationBuilder.DropTable(
                name: "RegistrationFeeConfigurations");

            migrationBuilder.DropTable(
                name: "RegistrationFeeReceipts");

            migrationBuilder.DropTable(
                name: "RegistrationProspectusConfigurations");

            migrationBuilder.DropTable(
                name: "RegistrationReceiptConfigurations");

            migrationBuilder.DropTable(
                name: "RollNumberSettings");

            migrationBuilder.DropTable(
                name: "StudentDocumentFieldMatches");

            migrationBuilder.DropTable(
                name: "StudentDocumentSuggestions");

            migrationBuilder.DropTable(
                name: "StudentRegistrations");

            migrationBuilder.DropTable(
                name: "UserBranchAccesses");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "FeeReceipts");

            migrationBuilder.DropTable(
                name: "StudentCharges");

            migrationBuilder.DropTable(
                name: "StudentDocumentAnalyses");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "FeeHeads");

            migrationBuilder.DropTable(
                name: "StudentFeeAssignments");

            migrationBuilder.DropTable(
                name: "StudentDocuments");

            migrationBuilder.DropTable(
                name: "Branches");

            migrationBuilder.DropTable(
                name: "FeeStructures");

            migrationBuilder.DropTable(
                name: "StudentAdmissions");

            migrationBuilder.DropTable(
                name: "DocumentModels");

            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropTable(
                name: "AcademicYears");

            migrationBuilder.DropTable(
                name: "Sections");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "AcademicClasses");
        }
    }
}
