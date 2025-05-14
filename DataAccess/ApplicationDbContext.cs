using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Models;
namespace DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }
       // public DbSet<Admin> Admins { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<DevelopmentStatus> DevelopmentStatuses { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<TestResult> TestResults { get; set; }
       // public DbSet<User> Users { get; set; }
        //public DbSet<GameEvent> gameEvents  { get; set; }
        public DbSet<VideoEvent> videoEvents   { get; set; }
        public DbSet<BookEvent> bookEvents    { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Test>()
                .HasOne(t => t.Doctor)
                .WithMany(d => d.Tests)
                .HasForeignKey(t => t.DoctorID)
                .OnDelete(DeleteBehavior.Restrict);  

            modelBuilder.Entity<Doctor>()
                .HasMany(d => d.Tests)
                .WithOne(t => t.Doctor)
                .HasForeignKey(t => t.DoctorID)
                .OnDelete(DeleteBehavior.Restrict);  


            modelBuilder.Entity<Test>()
                .HasMany(t => t.TestResults)
                .WithOne(tr => tr.Test)
                .HasForeignKey(tr => tr.TestID)
                .OnDelete(DeleteBehavior.Cascade);




        }
    }
}
