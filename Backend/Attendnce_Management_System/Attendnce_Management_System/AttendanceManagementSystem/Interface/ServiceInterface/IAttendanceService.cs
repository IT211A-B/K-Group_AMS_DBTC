using Attendance_Management_System.AttendanceManagementSystem.DTOs;

namespace Attendance_Management_System.AttendanceManagementSystem.Interface.ServiceInterface
{
    public interface IAttendanceService
    {
        Task<IEnumerable<GetAttendanceDTO>> GetAllAsync();
        Task<GetAttendanceDTO?> GetByIdAsync(int id);
        Task<GetAttendanceDTO> AddAsync(AddAttendanceDTO dto);
        Task<GetAttendanceDTO?> UpdateAsync(int id, AddAttendanceDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}