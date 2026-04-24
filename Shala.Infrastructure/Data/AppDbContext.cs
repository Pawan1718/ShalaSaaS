using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shala.Domain.Entities.Academics;
using Shala.Domain.Entities.Fees;
using Shala.Domain.Entities.Identity;
using Shala.Domain.Entities.Organization;
using Shala.Domain.Entities.Platform;
using Shala.Domain.Entities.Registration;
using Shala.Domain.Entities.Settings;
using Shala.Domain.Entities.StudentDocuments;
using Shala.Domain.Entities.Students;

namespace Shala.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<SchoolTenant> Tenants => Set<SchoolTenant>();
    public DbSet<Branch> Branches => Set<Branch>();

    public DbSet<AcademicYear> AcademicYears => Set<AcademicYear>();
    public DbSet<AcademicYearSetting> AcademicYearSettings => Set<AcademicYearSetting>();
    public DbSet<RollNumberSetting> RollNumberSettings => Set<RollNumberSetting>();
    public DbSet<AcademicClass> AcademicClasses => Set<AcademicClass>();
    public DbSet<Section> Sections => Set<Section>();

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Guardian> Guardians => Set<Guardian>();
    public DbSet<StudentAdmission> StudentAdmissions => Set<StudentAdmission>();
    public DbSet<UserBranchAccess> UserBranchAccesses => Set<UserBranchAccess>();
    public DbSet<StudentRegistration> StudentRegistrations => Set<StudentRegistration>();

    public DbSet<FeeHead> FeeHeads => Set<FeeHead>();
    public DbSet<FeeStructure> FeeStructures => Set<FeeStructure>();
    public DbSet<FeeStructureItem> FeeStructureItems => Set<FeeStructureItem>();
    public DbSet<StudentFeeAssignment> StudentFeeAssignments => Set<StudentFeeAssignment>();
    public DbSet<StudentCharge> StudentCharges => Set<StudentCharge>();
    public DbSet<FeeReceipt> FeeReceipts => Set<FeeReceipt>();
    public DbSet<FeeReceiptAllocation> FeeReceiptAllocations => Set<FeeReceiptAllocation>();
    public DbSet<StudentFeeLedger> StudentFeeLedgers => Set<StudentFeeLedger>();
    public DbSet<FeeReceiptCounter> FeeReceiptCounters => Set<FeeReceiptCounter>();

    public DbSet<RegistrationFeeReceipt> RegistrationFeeReceipts => Set<RegistrationFeeReceipt>();
    public DbSet<RegistrationFeeConfiguration> RegistrationFeeConfigurations => Set<RegistrationFeeConfiguration>();
    public DbSet<RegistrationReceiptConfiguration> RegistrationReceiptConfigurations => Set<RegistrationReceiptConfiguration>();
    public DbSet<RegistrationProspectusConfiguration> RegistrationProspectusConfigurations => Set<RegistrationProspectusConfiguration>();

    public DbSet<BranchDocumentProfile> BranchDocumentProfiles => Set<BranchDocumentProfile>();
    public DbSet<DocumentModel> DocumentModels => Set<DocumentModel>();
    public DbSet<StudentDocument> StudentDocuments => Set<StudentDocument>();
    public DbSet<StudentDocumentAnalysis> StudentDocumentAnalyses => Set<StudentDocumentAnalysis>();
    public DbSet<StudentDocumentFieldMatch> StudentDocumentFieldMatches => Set<StudentDocumentFieldMatch>();
    public DbSet<StudentDocumentSuggestion> StudentDocumentSuggestions => Set<StudentDocumentSuggestion>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        ConfigureIdentityAndOrg(builder);
        ConfigureAcademics(builder);
        ConfigureRegistration(builder);
        ConfigureFees(builder);
        ConfigureDocuments(builder);
    }

    private static void ConfigureIdentityAndOrg(ModelBuilder builder)
    {
        builder.Entity<ApplicationUser>()
            .HasOne(x => x.Tenant)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ApplicationUser>()
            .HasOne(x => x.Branch)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<UserBranchAccess>(entity =>
        {
            entity.ToTable("UserBranchAccesses");
            entity.HasKey(x => x.Id);

            entity.HasOne(x => x.User)
                .WithMany(x => x.BranchAccesses)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Branch)
                .WithMany(x => x.UserBranchAccesses)
                .HasForeignKey(x => x.BranchId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => new { x.UserId, x.BranchId }).IsUnique();
        });
    }

    private static void ConfigureAcademics(ModelBuilder builder)
    {
        builder.Entity<AcademicYear>()
            .HasIndex(x => new { x.TenantId, x.Name })
            .IsUnique();

        builder.Entity<AcademicYearSetting>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.TenantId).IsUnique();
        });

        builder.Entity<RollNumberSetting>(entity =>
        {
            entity.ToTable("RollNumberSettings");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.TenantId).IsUnique();

            entity.Property(x => x.AutoGenerate).HasDefaultValue(true);
            entity.Property(x => x.AllowManualOverride).HasDefaultValue(false);
            entity.Property(x => x.ResetPerAcademicYear).HasDefaultValue(true);
            entity.Property(x => x.ResetPerClass).HasDefaultValue(true);
            entity.Property(x => x.ResetPerSection).HasDefaultValue(false);
            entity.Property(x => x.StartFrom).HasDefaultValue(1);
            entity.Property(x => x.NumberPadding).HasDefaultValue(3);
            entity.Property(x => x.Prefix).HasMaxLength(20);
            entity.Property(x => x.Format).HasMaxLength(100).IsRequired();
        });

        builder.Entity<Section>()
            .HasOne(x => x.AcademicClass)
            .WithMany(x => x.Sections)
            .HasForeignKey(x => x.AcademicClassId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<StudentAdmission>(entity =>
        {
            entity.HasIndex(x => new
            {
                x.TenantId,
                x.BranchId,
                x.AcademicYearId,
                x.AcademicClassId,
                x.SectionId,
                x.RollNo
            })
            .IsUnique()
            .HasFilter("[RollNo] IS NOT NULL");

            entity.HasIndex(x => new
            {
                x.TenantId,
                x.BranchId,
                x.AcademicYearId,
                x.AcademicClassId,
                x.SectionId
            });

            entity.HasOne(x => x.Student)
                .WithMany(x => x.Admissions)
                .HasForeignKey(x => x.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.AcademicYear)
                .WithMany(x => x.StudentAdmissions)
                .HasForeignKey(x => x.AcademicYearId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(x => x.AcademicClass)
                .WithMany(x => x.StudentAdmissions)
                .HasForeignKey(x => x.AcademicClassId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(x => x.Section)
                .WithMany(x => x.StudentAdmissions)
                .HasForeignKey(x => x.SectionId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);
        });
    }

    private static void ConfigureRegistration(ModelBuilder builder)
    {
        builder.Entity<RegistrationFeeConfiguration>(entity =>
        {
            entity.HasIndex(x => new { x.TenantId, x.BranchId }).IsUnique();
            entity.Property(x => x.RegistrationFeeAmount).HasColumnType("decimal(18,2)");
        });

        builder.Entity<RegistrationProspectusConfiguration>(entity =>
        {
            entity.HasIndex(x => new { x.TenantId, x.BranchId }).IsUnique();
            entity.Property(x => x.ProspectusAmount).HasColumnType("decimal(18,2)");
            entity.Property(x => x.ProspectusDisplayName).HasMaxLength(100);
        });

        builder.Entity<RegistrationReceiptConfiguration>(entity =>
        {
            entity.HasIndex(x => new { x.TenantId, x.BranchId }).IsUnique();
            entity.Property(x => x.ReceiptTitle).HasMaxLength(150);
            entity.Property(x => x.ReceiptFooterNote).HasMaxLength(500);
        });
    }

    private static void ConfigureFees(ModelBuilder builder)
    {
        builder.Entity<FeeHead>(entity =>
        {
            entity.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.Code)
                .HasMaxLength(50);

            entity.Property(x => x.Description)
                .HasMaxLength(500);

            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.Name })
                .IsUnique();
        });

        builder.Entity<FeeStructure>(entity =>
        {
            entity.Property(x => x.Name)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.Description)
                .HasMaxLength(500);

            entity.HasIndex(x => new
            {
                x.TenantId,
                x.BranchId,
                x.AcademicYearId,
                x.AcademicClassId,
                x.Name
            }).IsUnique();

            entity.HasMany(x => x.Items)
                .WithOne(x => x.FeeStructure)
                .HasForeignKey(x => x.FeeStructureId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<FeeStructureItem>(entity =>
        {
            entity.Property(x => x.Label)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.Amount)
                .HasColumnType("decimal(18,2)");

            entity.HasOne(x => x.FeeHead)
                .WithMany()
                .HasForeignKey(x => x.FeeHeadId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => new { x.FeeStructureId, x.FeeHeadId, x.Label });
        });

        builder.Entity<StudentFeeAssignment>(entity =>
        {
            entity.Property(x => x.DiscountAmount)
                .HasColumnType("decimal(18,2)");

            entity.Property(x => x.AdditionalChargeAmount)
                .HasColumnType("decimal(18,2)");

            entity.HasOne(x => x.Student)
                .WithMany()
                .HasForeignKey(x => x.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.StudentAdmission)
                .WithMany()
                .HasForeignKey(x => x.StudentAdmissionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.FeeStructure)
                .WithMany()
                .HasForeignKey(x => x.FeeStructureId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.StudentAdmissionId })
                .IsUnique();
        });

        builder.Entity<StudentCharge>(entity =>
        {
            entity.Property(x => x.ChargeLabel)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.PeriodLabel)
                .HasMaxLength(100);

            entity.Property(x => x.Amount)
                .HasColumnType("decimal(18,2)");

            entity.Property(x => x.DiscountAmount)
                .HasColumnType("decimal(18,2)");

            entity.Property(x => x.FineAmount)
                .HasColumnType("decimal(18,2)");

            entity.Property(x => x.PaidAmount)
                .HasColumnType("decimal(18,2)");

            entity.HasOne(x => x.Student)
                .WithMany()
                .HasForeignKey(x => x.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.StudentAdmission)
                .WithMany()
                .HasForeignKey(x => x.StudentAdmissionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.StudentFeeAssignment)
                .WithMany(x => x.Charges)
                .HasForeignKey(x => x.StudentFeeAssignmentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.FeeHead)
                .WithMany()
                .HasForeignKey(x => x.FeeHeadId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(x => x.Allocations)
                .WithOne(x => x.StudentCharge)
                .HasForeignKey(x => x.StudentChargeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<FeeReceipt>(entity =>
        {
            entity.Property(x => x.TotalAmount)
                .HasColumnType("decimal(18,2)");

            entity.Property(x => x.ReceiptNo)
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(x => x.TransactionReference)
                .HasMaxLength(100);

            entity.Property(x => x.Remarks)
                .HasMaxLength(500);

            entity.Property(x => x.CancelReason)
                .HasMaxLength(250);

            entity.Property(x => x.IsCancelled)
                .HasDefaultValue(false);

            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.ReceiptNo })
                .IsUnique();

            entity.HasOne(x => x.Student)
                .WithMany()
                .HasForeignKey(x => x.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.StudentAdmission)
                .WithMany()
                .HasForeignKey(x => x.StudentAdmissionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(x => x.Allocations)
                .WithOne(x => x.FeeReceipt)
                .HasForeignKey(x => x.FeeReceiptId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<FeeReceiptAllocation>(entity =>
        {
            entity.Property(x => x.AllocatedAmount)
                .HasColumnType("decimal(18,2)");

            entity.HasIndex(x => new { x.FeeReceiptId, x.StudentChargeId })
                .IsUnique();

            entity.HasOne(x => x.FeeReceipt)
                .WithMany(x => x.Allocations)
                .HasForeignKey(x => x.FeeReceiptId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.StudentCharge)
                .WithMany(x => x.Allocations)
                .HasForeignKey(x => x.StudentChargeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<StudentFeeLedger>(entity =>
        {
            entity.ToTable("StudentFeeLedgers");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.EntryType)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.DebitAmount)
                .HasColumnType("decimal(18,2)");

            entity.Property(x => x.CreditAmount)
                .HasColumnType("decimal(18,2)");

            entity.Property(x => x.RunningBalance)
                .HasColumnType("decimal(18,2)");

            entity.Property(x => x.ReferenceNo)
                .HasMaxLength(100);

            entity.Property(x => x.Remarks)
                .HasMaxLength(500);

            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.StudentAdmissionId, x.EntryDate, x.Id });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.StudentId });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.FeeReceiptId });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.StudentChargeId });
        });

        builder.Entity<FeeReceiptCounter>(entity =>
        {
            entity.ToTable("FeeReceiptCounters");

            entity.HasKey(x => x.Id);

            entity.HasIndex(x => new
            {
                x.TenantId,
                x.BranchId,
                x.Year
            }).IsUnique();

            entity.Property(x => x.LastNumber)
                .HasDefaultValue(0);
        });
    }

    private static void ConfigureDocuments(ModelBuilder builder)
    {
        builder.Entity<DocumentModel>(entity =>
        {
            entity.ToTable("DocumentModels");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name).IsRequired().HasMaxLength(150);
            entity.Property(x => x.Code).IsRequired().HasMaxLength(100);
            entity.Property(x => x.Description).HasMaxLength(500);
            entity.Property(x => x.AllowedFileTypes).HasMaxLength(250);
            entity.Property(x => x.RequiredFieldsJson).HasColumnType("nvarchar(max)");
            entity.Property(x => x.CreatedBy).HasMaxLength(100);
            entity.Property(x => x.UpdatedBy).HasMaxLength(100);

            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.Code }).IsUnique();
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.IsActive });
        });

        builder.Entity<StudentDocument>(entity =>
        {
            entity.ToTable("StudentDocuments");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.DocumentType).IsRequired().HasMaxLength(100);
            entity.Property(x => x.Title).IsRequired().HasMaxLength(200);
            entity.Property(x => x.FileName).IsRequired().HasMaxLength(255);
            entity.Property(x => x.FilePath).IsRequired().HasMaxLength(500);
            entity.Property(x => x.MimeType).IsRequired().HasMaxLength(100);
            entity.Property(x => x.Status).IsRequired().HasMaxLength(50);
            entity.Property(x => x.Remarks).HasMaxLength(1000);
            entity.Property(x => x.CreatedBy).HasMaxLength(100);
            entity.Property(x => x.UpdatedBy).HasMaxLength(100);

            entity.HasOne(x => x.Student)
                .WithMany(x => x.Documents)
                .HasForeignKey(x => x.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.DocumentModel)
                .WithMany()
                .HasForeignKey(x => x.DocumentModelId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Analysis)
                .WithOne(x => x.StudentDocuments)
                .HasForeignKey<StudentDocumentAnalysis>(x => x.StudentDocumentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(x => x.Suggestions)
                .WithOne(x => x.StudentDocument)
                .HasForeignKey(x => x.StudentDocumentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.StudentId });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.IsActive });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.Status });
        });

        builder.Entity<StudentDocumentAnalysis>(entity =>
        {
            entity.ToTable("StudentDocumentAnalyses");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.ExtractedText).HasColumnType("nvarchar(max)");
            entity.Property(x => x.ExtractedJson).HasColumnType("nvarchar(max)");
            entity.Property(x => x.DetectedDocumentType).HasMaxLength(100);
            entity.Property(x => x.AnalysisStatus).HasMaxLength(50);
            entity.Property(x => x.OcrConfidence).HasColumnType("decimal(5,2)");
            entity.Property(x => x.AiConfidence).HasColumnType("decimal(5,2)");
            entity.Property(x => x.CreatedBy).HasMaxLength(100);
            entity.Property(x => x.UpdatedBy).HasMaxLength(100);

            entity.HasMany(x => x.FieldMatches)
                .WithOne(x => x.StudentDocumentAnalysis)
                .HasForeignKey(x => x.StudentDocumentAnalysisId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.StudentDocumentId).IsUnique();
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.IsActive });
        });

        builder.Entity<StudentDocumentFieldMatch>(entity =>
        {
            entity.ToTable("StudentDocumentFieldMatches");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.FieldName).IsRequired().HasMaxLength(100);
            entity.Property(x => x.DocumentValue).HasMaxLength(500);
            entity.Property(x => x.FormValue).HasMaxLength(500);
            entity.Property(x => x.MatchStatus).HasMaxLength(50);
            entity.Property(x => x.Suggestion).HasMaxLength(1000);
            entity.Property(x => x.ConfidenceScore).HasColumnType("decimal(5,2)");
            entity.Property(x => x.CreatedBy).HasMaxLength(100);
            entity.Property(x => x.UpdatedBy).HasMaxLength(100);

            entity.HasIndex(x => new { x.StudentDocumentAnalysisId, x.FieldName });
        });

        builder.Entity<StudentDocumentSuggestion>(entity =>
        {
            entity.ToTable("StudentDocumentSuggestions");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.SuggestionType).HasMaxLength(50);
            entity.Property(x => x.Message).IsRequired().HasMaxLength(1000);
            entity.Property(x => x.SuggestedValue).HasMaxLength(500);
            entity.Property(x => x.ConfidenceScore).HasColumnType("decimal(5,2)");
            entity.Property(x => x.CreatedBy).HasMaxLength(100);
            entity.Property(x => x.UpdatedBy).HasMaxLength(100);

            entity.HasIndex(x => x.StudentDocumentId);
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.IsActive });
        });




    }
}