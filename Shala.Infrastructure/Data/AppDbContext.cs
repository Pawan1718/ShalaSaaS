using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shala.Domain.Entities.Academics;
using Shala.Domain.Entities.Fees;
using Shala.Domain.Entities.Identity;
using Shala.Domain.Entities.Organization;
using Shala.Domain.Entities.Platform;
using Shala.Domain.Entities.Settings;
using Shala.Domain.Entities.StudentDocuments;
using Shala.Domain.Entities.Students;
using Shala.Domain.Entities.Supplies;

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

    public DbSet<FeeHead> FeeHeads => Set<FeeHead>();
    public DbSet<FeeStructure> FeeStructures => Set<FeeStructure>();
    public DbSet<FeeStructureItem> FeeStructureItems => Set<FeeStructureItem>();
    public DbSet<StudentFeeAssignment> StudentFeeAssignments => Set<StudentFeeAssignment>();
    public DbSet<StudentCharge> StudentCharges => Set<StudentCharge>();
    public DbSet<FeeReceipt> FeeReceipts => Set<FeeReceipt>();
    public DbSet<FeeReceiptAllocation> FeeReceiptAllocations => Set<FeeReceiptAllocation>();
    public DbSet<StudentFeeLedger> StudentFeeLedgers => Set<StudentFeeLedger>();
    public DbSet<FeeReceiptCounter> FeeReceiptCounters => Set<FeeReceiptCounter>();

    public DbSet<BranchDocumentProfile> BranchDocumentProfiles => Set<BranchDocumentProfile>();


    public DbSet<DocumentModel> DocumentModels => Set<DocumentModel>();
    public DbSet<StudentDocumentChecklist> StudentDocumentChecklists => Set<StudentDocumentChecklist>();


    public DbSet<SupplyItem> SupplyItems => Set<SupplyItem>();
    public DbSet<StudentSupplyIssue> StudentSupplyIssues => Set<StudentSupplyIssue>();
    public DbSet<StudentSupplyIssueItem> StudentSupplyIssueItems => Set<StudentSupplyIssueItem>();
    public DbSet<StudentSupplyPayment> StudentSupplyPayments => Set<StudentSupplyPayment>();
    public DbSet<SupplyStockLedger> SupplyStockLedgers => Set<SupplyStockLedger>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        ConfigureIdentityAndOrg(builder);
        ConfigureAcademics(builder);
        ConfigureStudents(builder);
        ConfigureFees(builder);
        ConfigureDocuments(builder);
        ConfigureSupplies(builder);
    }

    private static void ConfigureIdentityAndOrg(ModelBuilder builder)
    {
        builder.Entity<ApplicationUser>()
            .HasIndex(x => new { x.TenantId, x.BranchId });

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
            entity.HasIndex(x => x.BranchId);
        });
    }

    private static void ConfigureAcademics(ModelBuilder builder)
    {
        builder.Entity<AcademicYear>(entity =>
        {
            entity.HasIndex(x => new { x.TenantId, x.Name }).IsUnique();
            entity.HasIndex(x => new { x.TenantId, x.IsActive });
        });

        builder.Entity<AcademicClass>(entity =>
        {
            entity.HasIndex(x => new { x.TenantId, x.IsActive });
            entity.HasIndex(x => new { x.TenantId, x.Sequence });
        });

        builder.Entity<Section>(entity =>
        {
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.AcademicClassId });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.IsActive });

            entity.HasOne(x => x.AcademicClass)
                .WithMany(x => x.Sections)
                .HasForeignKey(x => x.AcademicClassId)
                .OnDelete(DeleteBehavior.NoAction);
        });

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

            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.StudentId });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.AdmissionNo });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.IsCurrent });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.Status });

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

    private static void ConfigureStudents(ModelBuilder builder)
    {
        builder.Entity<Student>(entity =>
        {
            entity.HasIndex(x => new { x.TenantId, x.BranchId });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.Status });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.FirstName });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.LastName });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.Mobile });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.Email });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.CreatedAt });
        });

        builder.Entity<Guardian>(entity =>
        {
            entity.HasIndex(x => new { x.TenantId, x.StudentId });
            entity.HasIndex(x => new { x.TenantId, x.Mobile });
            entity.HasIndex(x => new { x.TenantId, x.IsPrimary });
        });
    }

    private static void ConfigureFees(ModelBuilder builder)
    {
        builder.Entity<FeeHead>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Code).HasMaxLength(50);
            entity.Property(x => x.Description).HasMaxLength(500);

            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.Name }).IsUnique();
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.Code });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.IsActive });
        });

        builder.Entity<FeeStructure>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(150).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(500);

            entity.HasIndex(x => new
            {
                x.TenantId,
                x.BranchId,
                x.AcademicYearId,
                x.AcademicClassId,
                x.Name
            }).IsUnique();

            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.AcademicYearId });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.AcademicClassId });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.IsActive });

            entity.HasMany(x => x.Items)
                .WithOne(x => x.FeeStructure)
                .HasForeignKey(x => x.FeeStructureId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<FeeStructureItem>(entity =>
        {
            entity.Property(x => x.Label).HasMaxLength(150).IsRequired();
            entity.Property(x => x.Amount).HasColumnType("decimal(18,2)");

            entity.HasOne(x => x.FeeHead)
                .WithMany()
                .HasForeignKey(x => x.FeeHeadId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => new { x.FeeStructureId, x.FeeHeadId, x.Label });
            entity.HasIndex(x => new { x.FeeHeadId });
            entity.HasIndex(x => new { x.IsActive });
        });

        builder.Entity<StudentFeeAssignment>(entity =>
        {
            entity.Property(x => x.DiscountAmount).HasColumnType("decimal(18,2)");
            entity.Property(x => x.AdditionalChargeAmount).HasColumnType("decimal(18,2)");

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

            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.StudentAdmissionId }).IsUnique();
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.StudentId });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.FeeStructureId });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.IsActive });
        });

        builder.Entity<StudentCharge>(entity =>
        {
            entity.Property(x => x.ChargeLabel).HasMaxLength(200).IsRequired();
            entity.Property(x => x.PeriodLabel).HasMaxLength(100);
            entity.Property(x => x.Amount).HasColumnType("decimal(18,2)");
            entity.Property(x => x.DiscountAmount).HasColumnType("decimal(18,2)");
            entity.Property(x => x.FineAmount).HasColumnType("decimal(18,2)");
            entity.Property(x => x.PaidAmount).HasColumnType("decimal(18,2)");

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

            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.StudentId });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.StudentAdmissionId });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.StudentFeeAssignmentId });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.FeeHeadId });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.IsCancelled });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.DueDate });
        });

        builder.Entity<FeeReceipt>(entity =>
        {
            entity.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)");
            entity.Property(x => x.ReceiptNo).HasMaxLength(30).IsRequired();
            entity.Property(x => x.TransactionReference).HasMaxLength(100);
            entity.Property(x => x.Remarks).HasMaxLength(500);
            entity.Property(x => x.CancelReason).HasMaxLength(250);
            entity.Property(x => x.IsCancelled).HasDefaultValue(false);

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

            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.ReceiptNo }).IsUnique();
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.StudentId });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.StudentAdmissionId });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.ReceiptDate });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.IsCancelled });
        });

        builder.Entity<FeeReceiptAllocation>(entity =>
        {
            entity.Property(x => x.AllocatedAmount).HasColumnType("decimal(18,2)");

            entity.HasIndex(x => new { x.FeeReceiptId, x.StudentChargeId }).IsUnique();
            entity.HasIndex(x => x.StudentChargeId);

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

            entity.Property(x => x.EntryType).HasMaxLength(50).IsRequired();
            entity.Property(x => x.DebitAmount).HasColumnType("decimal(18,2)");
            entity.Property(x => x.CreditAmount).HasColumnType("decimal(18,2)");
            entity.Property(x => x.RunningBalance).HasColumnType("decimal(18,2)");
            entity.Property(x => x.ReferenceNo).HasMaxLength(100);
            entity.Property(x => x.Remarks).HasMaxLength(500);

            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.StudentAdmissionId, x.EntryDate, x.Id });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.StudentId });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.FeeReceiptId });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.StudentChargeId });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.EntryType });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.EntryDate });
        });

        builder.Entity<FeeReceiptCounter>(entity =>
        {
            entity.ToTable("FeeReceiptCounters");
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.Year }).IsUnique();
            entity.Property(x => x.LastNumber).HasDefaultValue(0);
        });
    }

    private static void ConfigureDocuments(ModelBuilder builder)
    {
        builder.Entity<BranchDocumentProfile>(entity =>
        {
            entity.HasIndex(x => new { x.TenantId, x.BranchId });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.IsActive });
        });

        builder.Entity<DocumentModel>(entity =>
        {
            entity.ToTable("DocumentModels");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Code).HasMaxLength(100);
            entity.Property(x => x.Description).HasMaxLength(1000);

            entity.Property(x => x.CreatedBy).HasMaxLength(100);
            entity.Property(x => x.UpdatedBy).HasMaxLength(100);

            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.IsActive });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.DisplayOrder });
        });

        builder.Entity<StudentDocumentChecklist>(entity =>
        {
            entity.ToTable("StudentDocumentChecklists");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Remark).HasMaxLength(1000);
            entity.Property(x => x.CreatedBy).HasMaxLength(100);
            entity.Property(x => x.UpdatedBy).HasMaxLength(100);

            // IMPORTANT:
            // Checklist admission ke against hai, direct Student ke against nahi.
            entity.HasOne(x => x.StudentAdmission)
                .WithMany(x => x.DocumentChecklists)
                .HasForeignKey(x => x.StudentAdmissionId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            entity.HasOne(x => x.DocumentModel)
                .WithMany()
                .HasForeignKey(x => x.DocumentModelId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            entity.HasIndex(x => new
            {
                x.TenantId,
                x.BranchId,
                x.StudentAdmissionId,
                x.DocumentModelId
            }).IsUnique();

            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.StudentAdmissionId });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.IsReceived });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.IsActive });
        });
    }

    private static void ConfigureSupplies(ModelBuilder builder)
    {
        builder.Entity<SupplyItem>(entity =>
        {
            entity.ToTable("SupplyItems");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name).HasMaxLength(150).IsRequired();
            entity.Property(x => x.Code).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(500);
            entity.Property(x => x.SalePrice).HasColumnType("decimal(18,2)");
            entity.Property(x => x.CurrentStock).HasColumnType("decimal(18,2)");
            entity.Property(x => x.MinimumStock).HasColumnType("decimal(18,2)");

            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.Code }).IsUnique();
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.Name });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.IsActive });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.CurrentStock });
        });

        builder.Entity<StudentSupplyIssue>(entity =>
        {
            entity.ToTable("StudentSupplyIssues");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.IssueNo).HasMaxLength(40).IsRequired();
            entity.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)");
            entity.Property(x => x.PaidAmount).HasColumnType("decimal(18,2)");
            entity.Property(x => x.DueAmount).HasColumnType("decimal(18,2)");
            entity.Property(x => x.Remarks).HasMaxLength(500);
            entity.Property(x => x.CancelReason).HasMaxLength(500);
            entity.Property(x => x.IsCancelled).HasDefaultValue(false);

            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.IssueNo }).IsUnique();
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.AcademicYearId, x.StudentId });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.StudentAdmissionId });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.IssueDate });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.IsCancelled });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.DueAmount });

            entity.HasOne(x => x.Student)
                .WithMany()
                .HasForeignKey(x => x.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.StudentAdmission)
                .WithMany()
                .HasForeignKey(x => x.StudentAdmissionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(x => x.Items)
                .WithOne(x => x.StudentSupplyIssue)
                .HasForeignKey(x => x.StudentSupplyIssueId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(x => x.Payments)
                .WithOne(x => x.StudentSupplyIssue)
                .HasForeignKey(x => x.StudentSupplyIssueId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<StudentSupplyIssueItem>(entity =>
        {
            entity.ToTable("StudentSupplyIssueItems");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.ItemName).HasMaxLength(150).IsRequired();
            entity.Property(x => x.ItemCode).HasMaxLength(50);
            entity.Property(x => x.Quantity).HasColumnType("decimal(18,2)");
            entity.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");
            entity.Property(x => x.LineTotal).HasColumnType("decimal(18,2)");

            entity.HasOne(x => x.SupplyItem)
                .WithMany(x => x.IssueItems)
                .HasForeignKey(x => x.SupplyItemId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => x.StudentSupplyIssueId);
            entity.HasIndex(x => x.SupplyItemId);
        });

        builder.Entity<StudentSupplyPayment>(entity =>
        {
            entity.ToTable("StudentSupplyPayments");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Amount).HasColumnType("decimal(18,2)");
            entity.Property(x => x.ReferenceNo).HasMaxLength(100);
            entity.Property(x => x.Remarks).HasMaxLength(500);

            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.AcademicYearId, x.PaymentDate });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.StudentSupplyIssueId });
        });

        builder.Entity<SupplyStockLedger>(entity =>
        {
            entity.ToTable("SupplyStockLedgers");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Quantity).HasColumnType("decimal(18,2)");
            entity.Property(x => x.BalanceAfter).HasColumnType("decimal(18,2)");
            entity.Property(x => x.ReferenceType).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Remarks).HasMaxLength(500);

            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.SupplyItemId, x.MovementDate });
            entity.HasIndex(x => new { x.TenantId, x.BranchId, x.ReferenceType });

            entity.HasOne(x => x.SupplyItem)
                .WithMany(x => x.StockLedgers)
                .HasForeignKey(x => x.SupplyItemId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}