using Microsoft.EntityFrameworkCore;

namespace VesteEVolta.Models;

public partial class VesteEVoltaContext : DbContext
{
    public VesteEVoltaContext(DbContextOptions<VesteEVoltaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TbUser> TbUsers { get; set; }
    public virtual DbSet<TbCategory> TbCategories { get; set; }
    public virtual DbSet<TbClothing> TbClothings { get; set; }
    public virtual DbSet<TbCustomer> TbCustomers { get; set; }
    public virtual DbSet<TbOwner> TbOwners { get; set; }
    public virtual DbSet<TbPayment> TbPayments { get; set; }
    public virtual DbSet<TbRating> TbRatings { get; set; }
    public virtual DbSet<TbRental> TbRentals { get; set; }
    public virtual DbSet<TbReport> TbReports { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Configuration is handled in Program.cs via Dependency Injection
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TbUser>(entity =>
        {
            entity.ToTable("tb_user");
            entity.HasKey(e => e.Id);

            entity.Ignore(e => e.TbReportReporteds);
            entity.Ignore(e => e.TbReportReporters);
        });

        modelBuilder.Entity<TbCategory>(entity =>
        {
            entity.ToTable("tb_category");
            entity.HasKey(e => e.CategoryId);
        });

        modelBuilder.Entity<TbClothing>(entity =>
        {
            entity.ToTable("tb_clothing");
            entity.HasKey(e => e.Id);

            entity.HasMany(c => c.Categories)
                  .WithMany(c => c.Clothings)
                  .UsingEntity(j => j.ToTable("tb_clothing_category"));
        });

        modelBuilder.Entity<TbCustomer>(entity =>
        {
            entity.ToTable("tb_customer");
            entity.HasKey(e => e.UserId);
        });

        modelBuilder.Entity<TbOwner>(entity =>
        {
            entity.ToTable("tb_owner");
            entity.HasKey(e => e.UserId);
        });

        modelBuilder.Entity<TbPayment>(entity =>
        {
            entity.ToTable("tb_payment");
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<TbRating>(entity =>
        {
            entity.ToTable("tb_rating");
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<TbRental>(entity =>
        {
            entity.ToTable("tb_rental");
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<TbReport>(entity =>
        {
            entity.ToTable("tb_report");
            entity.HasKey(e => e.ReportId);

            entity.Ignore(e => e.ReporterId);
            entity.Ignore(e => e.ReportedId);

            entity.HasOne(r => r.Reported)
                  .WithMany(u => u.ReportsReceived)
                  .HasForeignKey("ReportedUserId")
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.Reporter)
                  .WithMany(u => u.ReportsMade)
                  .HasForeignKey("ReporterUserId")
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
