using Backend.Backend.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NUlid;
using System.Text.RegularExpressions;

namespace Backend.Backend
{
    public class DatabaseLibrary : IdentityDbContext<User>
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
        public DbSet<AttendanceStudent> AttendanceStudents { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<AttendanceToken> AttendanceTokens { get; set; }
        public DbSet<Program_> Programs { get; set; }
        public DbSet<Schedule> Schedules { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Convert Enum into String when save in db 
            modelBuilder.Entity<Attendance>()
                .Property(s => s.TeacherStatus)
                .HasConversion<string>();

            modelBuilder.Entity<AttendanceStudent>()
                .Property(s => s.StudentAttendance)
                .HasConversion<string>();

            /* 
            RolePermission is a Junction Table
            Role_ID and Permission_ID as Composite Key not Superkey
            It Asks both ID to locate uniqueness and considered an Primary Key

            eg. 
            Role : Permission
            SuperUser -> User.Update
            SuperUser -> User.Delete

            Both form uniqueness
            ps. (Worth it Ma'am Joan's Lessons HAHAHAHA)
            */
            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.Role_ID, rp.Permission_ID });

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany()
                .HasForeignKey(rp => rp.Role_ID);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission_Entity)
                .WithMany()
                .HasForeignKey(rp => rp.Permission_ID);

            // Composite Key for Junction Table
            modelBuilder.Entity<AttendanceStudent>()
                .HasKey(x => new { x.Attendance_Id, x.Student_Id });

            //Relationships
            modelBuilder.Entity<Student>() // Entity: Student
                .HasOne(s => s.User) // Navigation: Student -> User
                .WithOne() // 1:1 relationship
                .HasForeignKey<Student>(s => s.User_ID) // FK in Student
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Teacher>() // Entity: Teacher
                .HasOne(t => t.User) // Navigation
                .WithOne() // 1:1
                .HasForeignKey<Teacher>(t => t.User_ID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Course>() // Entity: Course
                .HasOne(c => c.Teacher) // Navigation: Course -> Teacher
                .WithMany(t => t.Courses) // Navigation: Teacher -> Courses
                .HasForeignKey(c => c.Teacher_ID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Schedule>() // Entity: Schedule
                .HasOne(s => s.Course) // Navigation
                .WithMany(c => c.Schedules)
                .HasForeignKey(s => s.Course_ID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Schedule>() // Entity: Schedule
                .HasOne(s => s.Section)
                .WithMany(sec => sec.Schedules)
                .HasForeignKey(s => s.Section_ID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Student>() // Entity: Student
                .HasOne(s => s.Section)
                .WithMany(sec => sec.Students)
                .HasForeignKey(s => s.SectionID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Student>() // Entity: Student
                .HasOne(s => s.Program)
                .WithMany(p => p.Students)
                .HasForeignKey(s => s.Program_ID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Student>() // Entity: Student
                .HasOne(s => s.Department)
                .WithMany(d => d.Students)
                .HasForeignKey(s => s.Department_ID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Attendance>() // Entity: Attendance
                .HasOne(a => a.Schedule)
                .WithMany(s => s.Attendances)
                .HasForeignKey(a => a.Schedule_ID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AttendanceStudent>()
                .HasOne(a => a.Attendance) // Navigation
                .WithMany(att => att.AttendanceStudents)
                .HasForeignKey(a => a.Attendance_Id)
                .OnDelete(DeleteBehavior.Cascade);
                            
            modelBuilder.Entity<AttendanceStudent>()
                .HasOne(a => a.Student)
                .WithMany(s => s.AttendanceStudents)
                .HasForeignKey(a => a.Student_Id)
                .OnDelete(DeleteBehavior.Cascade);

            // Enforce Unique Attendance per Schedule per Date
            modelBuilder.Entity<Attendance>()
                .HasIndex(a => new { a.Schedule_ID, a.Date })
                .IsUnique();

            modelBuilder.Entity<Student>()
                .HasIndex(s => s.DocumentSeries)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(s => s.DocumentSeries)
                .IsUnique();

            modelBuilder.Entity<Teacher>()
                .HasIndex(s => s.DocumentSeries)
                .IsUnique();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<User>())
            {
                // check if new user
                if (entry.State == EntityState.Added &&
                    string.IsNullOrEmpty(entry.Entity.Id))
                {
                    entry.Entity.Id = NUlid.Ulid.NewUlid().ToString();
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }

}
