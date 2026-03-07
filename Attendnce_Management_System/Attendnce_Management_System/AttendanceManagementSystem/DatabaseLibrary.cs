using Attendnce_Management_System.AttendanceManagementSystem.Model;
using Microsoft.EntityFrameworkCore;

namespace Attendnce_Management_System.AttendanceManagementSystem
{
    public class DatabaseLibrary : DbContext
    {
        public DatabaseLibrary(DbContextOptions<DatabaseLibrary> options) : base(options)
        {
        }
        public DbSet<Attendance> Attendance { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Course> Courses { get; set; }
    }
}
