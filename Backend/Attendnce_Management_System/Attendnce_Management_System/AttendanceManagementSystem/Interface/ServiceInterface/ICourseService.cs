using Attendance_Management_System.AttendanceManagementSystem.DTOs;

namespace Attendance_Management_System.AttendanceManagementSystem.Interface.ServiceInterface
{
    public interface ICourseService
    {
        Task<IEnumerable<GetCourseDTO>> GetAllAsync();
        Task<GetCourseDTO?> GetByIdAsync(int id);
        Task<GetCourseDTO> AddAsync(AddCourseDTO dto);
        Task<GetCourseDTO?> UpdateAsync(int id, AddCourseDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}