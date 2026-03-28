using Attendance_Management_System.AttendanceManagementSystem.DTOs;

namespace Attendance_Management_System.AttendanceManagementSystem.Interface.ServiceInterface
{
    public interface IStudentService
    {
        Task<IEnumerable<GetStudentDTO>> GetAllAsync();
        Task<GetStudentDTO?> GetByIdAsync(int id);
        Task<GetStudentDTO> AddAsync(AddStudentDTO dto);
        Task<GetStudentDTO?> UpdateAsync(int id, AddStudentDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}