using Backend.Backend.Model;
using Microsoft.EntityFrameworkCore;

namespace Backend.Backend
{
    public class DatabaseLibrary : DbContext
    {
        public DatabaseLibrary(DbContextOptions<DatabaseLibrary> options) : base(options)
        {
        }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Program_> Programs { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Attendance>()
                .Property(s => s.Status)
                .HasConversion<string>();

            /* 
             RolePermission is a Junction Table
             Role_ID and Permission_ID as Composite Key not Superkey
             It Asks both ID to locate uniqueness and considered an Primary Key

            eg. 
            Role : Permission
            SuperUser -> User.Update
            SuperUser -> User.Delete

            Both form made uniqueness
            ps. (Worth it Ma'am Joan's Lessons HAHAHAHA)
             */
            modelBuilder.Entity<RolePermission>()
            .HasKey(rp => new { rp.Role_ID, rp.Permission_ID });

        }
    }

}
