using Attendnce_Management_System.AttendanceManagementSystem.Model;

namespace Smart_Library.SmartLibraryManagement.Interface
{
    public interface IAttendanceRepository
    {
        Task<IEnumerable<Attendance>> GetAllAsync();
        Task<Attendance?> GetByIdAsync(int id);
        Task AddAsync(Attendance attendance);
        Task UpdateAsync(Attendance attendance);
        Task DeleteAsync(Attendance attendance);
    }
}