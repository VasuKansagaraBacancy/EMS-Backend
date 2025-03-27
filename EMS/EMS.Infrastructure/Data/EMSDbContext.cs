using EMS.EMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EMS.EMS.Infrastructure.Data
{
    public class EMSDbContext : DbContext
    {
        public EMSDbContext(DbContextOptions<EMSDbContext> options) : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Timesheet> Timesheets { get; set; }
        public DbSet<Leave> Leaves { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
        public DbSet<TokenBlacklist> TokenBlacklists { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.User)
                .WithOne(u => u.Employee)
                .HasForeignKey<Employee>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.Property(e => e.TechStack)
                      .HasColumnType("text");
                entity.Property(e => e.Address)
                      .HasColumnType("text");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email)
                      .IsUnique();
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasIndex(d => d.DepartmentName)
                      .IsUnique();
            });

            modelBuilder.Entity<Timesheet>(entity =>
            {
                entity.Property(t => t.Description)
                      .HasColumnType("text");

                entity.HasOne(t => t.Employee)
                      .WithMany(e => e.Timesheets)
                      .HasForeignKey(t => t.EmployeeId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Leave>(entity =>
            {
                entity.Property(l => l.LeaveType)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(l => l.Status)
                      .HasMaxLength(20)
                      .HasDefaultValue("Pending")
                      .IsRequired();

                entity.Property(l => l.AppliedAt)
                      .HasDefaultValueSql("GETDATE()");

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Leave_LeaveType", "[LeaveType] IN ('Sick', 'Casual', 'Vacation', 'Other')");
                    t.HasCheckConstraint("CK_Leave_Status", "[Status] IN ('Pending', 'Approved', 'Rejected')");
                });

            });

            SeedRoles(modelBuilder);
        }

        // Seed roles data
        private void SeedRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "Admin" },
                new Role { RoleId = 2, RoleName = "Employee" }
            );
        }
    }
}