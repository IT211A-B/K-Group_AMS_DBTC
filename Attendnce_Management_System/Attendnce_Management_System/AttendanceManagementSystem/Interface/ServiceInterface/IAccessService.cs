using Attendance_Management_System.AttendanceManagementSystem.DTOs;

namespace Attendance_Management_System.AttendanceManagementSystem.Interface.ServiceInterface
{
    public interface IAccessService
    {
        Task<IEnumerable<GetAccessDTO>> GetAllAsync();
        Task<GetAccessDTO?> GetByIdAsync(int id);
        Task<GetAccessDTO> AddAsync(AddAccessDTO dto);
        Task<GetAccessDTO?> UpdateAsync(int id, AddAccessDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}