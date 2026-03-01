using Microsoft.EntityFrameworkCore;
using StudentApplication.Data.Models;

namespace StudentApplication.Data
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Professor> Professors { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
        public DatabaseContext() { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // USERS
            modelBuilder.Entity<User>(b =>
            {
                b.HasIndex(u => u.Email).IsUnique();
                b.HasIndex(u => u.Username).IsUnique();
                b.Property(u => u.Email).HasMaxLength(254).IsRequired();
                b.Property(u => u.Username).HasMaxLength(100).IsRequired();
                b.Property(u => u.Password).IsRequired();
            });

            // DEPARTMENTS
            modelBuilder.Entity<Department>(b =>
            {
                b.Property(d => d.Name).IsRequired().HasMaxLength(200);
                b.HasIndex(d => d.Name).IsUnique();
            });

            // ADMINS
            modelBuilder.Entity<Admin>(b =>
            {
                b.Property(a => a.Username).IsRequired().HasMaxLength(100);
                b.HasIndex(a => a.Username).IsUnique();
            });

            // STUDENTS
            modelBuilder.Entity<Student>(b =>
            {
                b.Property(s => s.FirstName).IsRequired().HasMaxLength(100);
                b.Property(s => s.LastName).IsRequired().HasMaxLength(100);

                b.HasOne(s => s.Department)
                 .WithMany(d => d.Students)
                 .HasForeignKey(s => s.DepartmentId)
                 .OnDelete(DeleteBehavior.SetNull);

                b.HasOne(s => s.User)
                 .WithOne(u => u.StudentProfile)
                 .HasForeignKey<Student>(s => s.UserId)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            // PROFESSORS
            modelBuilder.Entity<Professor>(b =>
            {
                b.Property(p => p.FirstName).IsRequired().HasMaxLength(100);
                b.Property(p => p.LastName).IsRequired().HasMaxLength(100);
                b.Property(p => p.Title).HasMaxLength(100);

                b.HasOne(p => p.Department)
                 .WithMany(d => d.Professors)
                 .HasForeignKey(p => p.DepartmentId)
                 .OnDelete(DeleteBehavior.SetNull);

                b.HasOne(p => p.User)
                 .WithOne(u => u.ProfessorProfile)
                 .HasForeignKey<Professor>(p => p.UserId)
                 .OnDelete(DeleteBehavior.SetNull);

                b.HasOne(p => p.ApprovedByAdmin)
                 .WithMany(a => a.ApprovedProfessors)
                 .HasForeignKey(p => p.ApprovedByAdminId)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            // SUBJECTS
            modelBuilder.Entity<Subject>(b =>
            {
                b.Property(s => s.Title).IsRequired().HasMaxLength(200);
                b.Property(s => s.Description).HasMaxLength(2000);

                b.HasOne(s => s.Professor)
                 .WithMany(p => p.Subjects)
                 .HasForeignKey(s => s.ProfessorId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ENROLLMENTS
            modelBuilder.Entity<Enrollment>(b =>
            {
                b.Property(e => e.EnrolledAt).HasDefaultValueSql("GETUTCDATE()");
                b.HasOne(e => e.Student)
                 .WithMany(s => s.Enrollments)
                 .HasForeignKey(e => e.StudentId)
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(e => e.Subject)
                 .WithMany(s => s.Enrollments)
                 .HasForeignKey(e => e.SubjectId)
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(e => e.Grade)
                 .WithOne(g => g.Enrollment)
                 .HasForeignKey<Grade>(g => g.EnrollmentId)
                 .OnDelete(DeleteBehavior.Cascade);

                // A student can enroll a given subject only once
                b.HasIndex(e => new { e.StudentId, e.SubjectId }).IsUnique();
            });

            // GRADES (simplified: rely only on EnrollmentId)
            modelBuilder.Entity<Grade>(b =>
            {
                b.Property(g => g.AssignedAt).HasDefaultValueSql("GETUTCDATE()");
                // No direct FKs to Student/Professor/Subject to avoid drift
            });
        }
    }
}