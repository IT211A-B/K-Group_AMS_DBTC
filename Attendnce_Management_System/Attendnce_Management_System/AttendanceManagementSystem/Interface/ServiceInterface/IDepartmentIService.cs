using Attendance_Management_System.AttendanceManagementSystem.DTOs;

namespace Attendance_Management_System.AttendanceManagementSystem.Interface.ServiceInterface
{
    public interface IDepartmentService
    {
        Task<IEnumerable<GetDepartmentDTO>> GetAllAsync();
        Task<GetDepartmentDTO?> GetByIdAsync(int id);
        Task<GetDepartmentDTO> AddAsync(AddDepartmentDTO dto);
        Task<GetDepartmentDTO?> UpdateAsync(int id, AddDepartmentDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}