using Attendance_Management_System.AttendanceManagementSystem.DTOs;

namespace Attendance_Management_System.AttendanceManagementSystem.Interface.ServiceInterface
{
    public interface IProgramService
    {
        Task<IEnumerable<GetProgramDTO>> GetAllAsync();
        Task<GetProgramDTO?> GetByIdAsync(int id);
        Task<GetProgramDTO> AddAsync(AddProgramDTO dto);
        Task<GetProgramDTO?> UpdateAsync(int id, AddProgramDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}