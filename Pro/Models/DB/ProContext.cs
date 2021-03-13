using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Pro.Models.DB
{
    public partial class ProContext : DbContext
    {
        public ProContext()
        {
        }

        public ProContext(DbContextOptions<ProContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ArchivalEmailAddress> ArchivalEmailAddresses { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Polish_CI_AS");

            modelBuilder.Entity<ArchivalEmailAddress>(entity =>
            {
                entity.ToTable("ArchivalEmailAddresses", "Pro");

                entity.Property(e => e.EmailAddress)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsFixedLength(true);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ArchivalEmailAddresses)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ArchivalEmailAddresses_Users");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users", "Pro");

                entity.Property(e => e.ControlHash)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LastPasswordChanged).HasColumnType("date");

                entity.Property(e => e.Login)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Password).IsRequired();

                entity.Property(e => e.UserHash)
                    .IsRequired()
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
