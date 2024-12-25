using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;
namespace DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Child> Children { get; set; }
        public DbSet<DevelopmentStatus> DevelopmentStatuses { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Parent> Parents { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<TestResult> TestResults { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TestResult>()
                .HasOne(tr => tr.Test)
                .WithMany()
                .HasForeignKey(tr => tr.TestID)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
