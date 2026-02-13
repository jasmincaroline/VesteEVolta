using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using VesteEVolta.Models;

namespace VesteEVolta.Models;

public partial class PostgresContext : DbContext
{
    public PostgresContext(DbContextOptions<PostgresContext> options)      //construtor
        : base(options)
    {
    }

    public virtual DbSet<TbCategory> TbCategories { get; set; }        //cada dbset é uma tabela

    public virtual DbSet<TbClothing> TbClothings { get; set; }

    public virtual DbSet<TbCustomer> TbCustomers { get; set; }

    public virtual DbSet<TbOwner> TbOwners { get; set; }

    public virtual DbSet<TbPayment> TbPayments { get; set; }

    public virtual DbSet<TbRating> TbRatings { get; set; }

    public virtual DbSet<TbRental> TbRentals { get; set; }

    public virtual DbSet<TbReport> TbReports { get; set; }

    public virtual DbSet<TbUser> TbUsers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
       
        base.OnConfiguring(optionsBuilder);
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)  
    {
        modelBuilder.Entity<TbCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("tb_category_pkey");

            entity.ToTable("tb_category");

            entity.HasIndex(e => e.Name, "tb_category_name_key").IsUnique();

            entity.Property(e => e.CategoryId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("category_id");
            entity.Property(e => e.Name)
                .HasMaxLength(120)
                .HasColumnName("name");
        });

        modelBuilder.Entity<TbClothing>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tb_clothing_pkey");

            entity.ToTable("tb_clothing");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.AvailabilityStatus)
                .HasMaxLength(30)
                .HasColumnName("availability_status");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");
            entity.Property(e => e.OwnerId).HasColumnName("owner_id");
            entity.Property(e => e.RentPrice)
                .HasPrecision(10, 2)
                .HasColumnName("rent_price");

            entity.HasOne(d => d.Owner).WithMany(p => p.TbClothings)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_clothing_owner");

            entity.HasMany(d => d.Categories).WithMany(p => p.Clothings)
                .UsingEntity<Dictionary<string, object>>(
                    "TbClothingCategory",
                    r => r.HasOne<TbCategory>().WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_clothing_category_category"),
                    l => l.HasOne<TbClothing>().WithMany()
                        .HasForeignKey("ClothingId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_clothing_category_clothing"),
                    j =>
                    {
                        j.HasKey("ClothingId", "CategoryId").HasName("tb_clothing_category_pkey");
                        j.ToTable("tb_clothing_category");
                        j.IndexerProperty<Guid>("ClothingId").HasColumnName("clothing_id");
                        j.IndexerProperty<Guid>("CategoryId").HasColumnName("category_id");
                    });
        });

        modelBuilder.Entity<TbCustomer>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("tb_customer_pkey");

            entity.ToTable("tb_customer");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");

            entity.HasOne(d => d.User).WithOne(p => p.TbCustomer)
                .HasForeignKey<TbCustomer>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_customer_user");
        });

        modelBuilder.Entity<TbOwner>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("tb_owner_pkey");

            entity.ToTable("tb_owner");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");

            entity.HasOne(d => d.User).WithOne(p => p.TbOwner)
                .HasForeignKey<TbOwner>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_owner_user");
        });

        modelBuilder.Entity<TbPayment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tb_payment_pkey");

            entity.ToTable("tb_payment");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasPrecision(10, 2)
                .HasColumnName("amount");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("now()")
                .HasColumnName("payment_date");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(30)
                .HasColumnName("payment_method");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(20)
                .HasDefaultValueSql("'pending'::character varying")
                .HasColumnName("payment_status");
            entity.Property(e => e.RentalId).HasColumnName("rental_id");

            entity.HasOne(d => d.Rental).WithMany(p => p.TbPayments)
                .HasForeignKey(d => d.RentalId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_payment_rental");
        });

        modelBuilder.Entity<TbRating>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tb_rating_pkey");

            entity.ToTable("tb_rating");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.ClothingId).HasColumnName("clothing_id");
            entity.Property(e => e.Comment)
                .HasMaxLength(300)
                .HasColumnName("comment");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.RentId).HasColumnName("rent_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Clothing).WithMany(p => p.TbRatings)
                .HasForeignKey(d => d.ClothingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_rating_clothing");

            entity.HasOne(d => d.Rent).WithMany(p => p.TbRatings)
                .HasForeignKey(d => d.RentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_rating_rental");

            entity.HasOne(d => d.User).WithMany(p => p.TbRatings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_rating_user");
        });

        modelBuilder.Entity<TbRental>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tb_rental_pkey");

            entity.ToTable("tb_rental");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.ClothingId).HasColumnName("clothing_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.TotalValue)
                .HasPrecision(10, 2)
                .HasColumnName("total_value");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Clothing).WithMany(p => p.TbRentals)
                .HasForeignKey(d => d.ClothingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_rental_clothing");

            entity.HasOne(d => d.User).WithMany(p => p.TbRentals)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_rental_user");
        });

        modelBuilder.Entity<TbReport>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("tb_report_pkey");

            entity.ToTable("tb_report");

            entity.Property(e => e.ReportId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("report_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Description)
                .HasMaxLength(800)
                .HasColumnName("description");
            entity.Property(e => e.Reason)
                .HasMaxLength(120)
                .HasColumnName("reason");
            entity.Property(e => e.RentalId).HasColumnName("rental_id");
            entity.Property(e => e.ReportedClothingId).HasColumnName("reported_clothing_id");
            entity.Property(e => e.ReportedId).HasColumnName("reported_id");
            entity.Property(e => e.ReporterId).HasColumnName("reporter_id");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasColumnName("status");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");

            entity.HasOne(d => d.Rental).WithMany(p => p.TbReports)
                .HasForeignKey(d => d.RentalId)
                .HasConstraintName("fk_report_rental");

            entity.HasOne(d => d.ReportedClothing).WithMany(p => p.TbReports)
                .HasForeignKey(d => d.ReportedClothingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_report_clothing");

            entity.HasOne(d => d.Reported).WithMany(p => p.TbReportReporteds)
                .HasForeignKey(d => d.ReportedId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_report_reported");

            entity.HasOne(d => d.Reporter).WithMany(p => p.TbReportReporters)
                .HasForeignKey(d => d.ReporterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_report_reporter");
        });

        modelBuilder.Entity<TbUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tb_user_pkey");

            entity.ToTable("tb_user");

            entity.HasIndex(e => e.Email, "tb_user_email_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(256)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(256)
                .HasColumnName("name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(256)
                .HasColumnName("password");
            entity.Property(e => e.ProfileType)
                .HasMaxLength(20)
                .HasColumnName("profile_type");
            entity.Property(e => e.Reported).HasColumnName("reported");
            entity.Property(e => e.Telephone)
                .HasMaxLength(30)
                .HasColumnName("telephone");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("User_pkey");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "User_email_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .HasColumnName("password");
            entity.Property(e => e.ProfileType)
                .HasMaxLength(10)
                .HasColumnName("profile_type");
            entity.Property(e => e.Reported)
                .HasDefaultValue(false)
                .HasColumnName("reported");
            entity.Property(e => e.Telephone)
                .HasMaxLength(20)
                .HasColumnName("telephone");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
