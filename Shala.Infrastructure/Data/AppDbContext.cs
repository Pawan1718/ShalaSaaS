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

namespace Shala.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<SchoolTenant> Tenants { get; set; }
        public DbSet<Branch> Branches { get; set; }

        public DbSet<AcademicYear> AcademicYears { get; set; }
        public DbSet<AcademicYearSetting> AcademicYearSettings { get; set; }
        public DbSet<RollNumberSetting> RollNumberSettings { get; set; }
        public DbSet<AcademicClass> AcademicClasses { get; set; }
        public DbSet<Section> Sections { get; set; }

        public DbSet<Student> Students { get; set; }
        public DbSet<Guardian> Guardians { get; set; }
        public DbSet<StudentAdmission> StudentAdmissions { get; set; }

        public DbSet<UserBranchAccess> UserBranchAccesses { get; set; }

        public DbSet<StudentRegistration> StudentRegistrations { get; set; }

        // Fees
        public DbSet<FeeHead> FeeHeads { get; set; }
        public DbSet<FeeStructure> FeeStructures { get; set; }
        public DbSet<FeeStructureItem> FeeStructureItems { get; set; }
        public DbSet<StudentFeeAssignment> StudentFeeAssignments { get; set; }
        public DbSet<StudentCharge> StudentCharges { get; set; }
        public DbSet<FeeReceipt> FeeReceipts { get; set; }
        public DbSet<FeeReceiptAllocation> FeeReceiptAllocations { get; set; }

        public DbSet<RegistrationFeeReceipt> RegistrationFeeReceipts { get; set; }
        public DbSet<RegistrationFeeConfiguration> RegistrationFeeConfigurations { get; set; }
        public DbSet<RegistrationReceiptConfiguration> RegistrationReceiptConfigurations { get; set; }
        public DbSet<RegistrationProspectusConfiguration> RegistrationProspectusConfigurations { get; set; }

        public DbSet<BranchDocumentProfile> BranchDocumentProfiles { get; set; }

        public DbSet<DocumentModel> DocumentModels => Set<DocumentModel>();
        public DbSet<StudentDocument> StudentDocuments => Set<StudentDocument>();
        public DbSet<StudentDocumentAnalysis> StudentDocumentAnalyses => Set<StudentDocumentAnalysis>();
        public DbSet<StudentDocumentFieldMatch> StudentDocumentFieldMatches => Set<StudentDocumentFieldMatch>();
        public DbSet<StudentDocumentSuggestion> StudentDocumentSuggestions => Set<StudentDocumentSuggestion>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // =========================================================
            // Identity / Org
            // =========================================================

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

            // =========================================================
            // Academics
            // =========================================================

            builder.Entity<AcademicYear>(entity =>
            {
                entity.HasIndex(x => new { x.TenantId, x.Name }).IsUnique();
            });

            builder.Entity<AcademicYearSetting>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => x.TenantId).IsUnique();
            });

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
            });

            builder.Entity<RollNumberSetting>(entity =>
            {
                entity.ToTable("RollNumberSettings");

                entity.HasKey(x => x.Id);

                entity.HasIndex(x => x.TenantId)
                    .IsUnique();

                entity.Property(x => x.AutoGenerate)
                    .HasDefaultValue(true);

                entity.Property(x => x.AllowManualOverride)
                    .HasDefaultValue(false);

                entity.Property(x => x.ResetPerAcademicYear)
                    .HasDefaultValue(true);

                entity.Property(x => x.ResetPerClass)
                    .HasDefaultValue(true);

                entity.Property(x => x.ResetPerSection)
                    .HasDefaultValue(false);

                entity.Property(x => x.StartFrom)
                    .HasDefaultValue(1);

                entity.Property(x => x.NumberPadding)
                    .HasDefaultValue(3);

                entity.Property(x => x.Prefix)
                    .HasMaxLength(20);

                entity.Property(x => x.Format)
                    .HasMaxLength(100)
                    .IsRequired();
            });

            builder.Entity<Section>()
                .HasOne(x => x.AcademicClass)
                .WithMany(x => x.Sections)
                .HasForeignKey(x => x.AcademicClassId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<StudentAdmission>()
                .HasOne(x => x.Student)
                .WithMany(x => x.Admissions)
                .HasForeignKey(x => x.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<StudentAdmission>()
                .HasOne(x => x.AcademicYear)
                .WithMany(x => x.StudentAdmissions)
                .HasForeignKey(x => x.AcademicYearId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<StudentAdmission>()
                .HasOne(x => x.AcademicClass)
                .WithMany(x => x.StudentAdmissions)
                .HasForeignKey(x => x.AcademicClassId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<StudentAdmission>()
                .HasOne(x => x.Section)
                .WithMany(x => x.StudentAdmissions)
                .HasForeignKey(x => x.SectionId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            // =========================================================
            // Registration
            // =========================================================

            builder.Entity<RegistrationFeeConfiguration>(entity =>
            {
                entity.HasIndex(x => new { x.TenantId, x.BranchId }).IsUnique();

                entity.Property(x => x.RegistrationFeeAmount)
                    .HasColumnType("decimal(18,2)");
            });

            builder.Entity<RegistrationProspectusConfiguration>(entity =>
            {
                entity.HasIndex(x => new { x.TenantId, x.BranchId }).IsUnique();

                entity.Property(x => x.ProspectusAmount)
                    .HasColumnType("decimal(18,2)");

                entity.Property(x => x.ProspectusDisplayName)
                    .HasMaxLength(100);
            });

            builder.Entity<RegistrationReceiptConfiguration>(entity =>
            {
                entity.HasIndex(x => new { x.TenantId, x.BranchId }).IsUnique();

                entity.Property(x => x.ReceiptTitle)
                    .HasMaxLength(150);

                entity.Property(x => x.ReceiptFooterNote)
                    .HasMaxLength(500);
            });

            // =========================================================
            // Fees
            // =========================================================

            builder.Entity<FeeHead>(entity =>
            {
                entity.Property(x => x.Name)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.Code)
                    .HasMaxLength(50);

                entity.Property(x => x.Description)
                    .HasMaxLength(500);

                entity.HasIndex(x => new { x.TenantId, x.BranchId, x.Name }).IsUnique();
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
                entity.HasOne(x => x.Student)
                    .WithMany(x => x.FeeAssignments)
                    .HasForeignKey(x => x.StudentId)
                    .OnDelete(DeleteBehavior.NoAction);

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

            // =========================================================
            // Documents
            // =========================================================

            builder.Entity<DocumentModel>(entity =>
            {
                entity.ToTable("DocumentModels");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(x => x.Code)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.Description)
                    .HasMaxLength(500);

                entity.Property(x => x.AllowedFileTypes)
                    .HasMaxLength(250);

                entity.Property(x => x.RequiredFieldsJson)
                    .HasColumnType("nvarchar(max)");

                entity.Property(x => x.CreatedBy).HasMaxLength(100);
                entity.Property(x => x.UpdatedBy).HasMaxLength(100);

                entity.HasIndex(x => new { x.TenantId, x.BranchId, x.Code }).IsUnique();
                entity.HasIndex(x => new { x.TenantId, x.BranchId, x.IsActive });
            });

            builder.Entity<StudentDocument>(entity =>
            {
                entity.ToTable("StudentDocuments");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.DocumentType)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(x => x.FileName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(x => x.FilePath)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(x => x.MimeType)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.Status)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(x => x.Remarks)
                    .HasMaxLength(1000);

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

                entity.Property(x => x.FieldName)
                    .IsRequired()
                    .HasMaxLength(100);

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
}