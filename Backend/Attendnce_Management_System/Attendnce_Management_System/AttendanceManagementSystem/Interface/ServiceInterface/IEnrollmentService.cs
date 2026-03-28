using Attendance_Management_System.AttendanceManagementSystem.DTOs;

namespace Attendance_Management_System.AttendanceManagementSystem.Interface.ServiceInterface
{
    public interface IEnrollmentService
    {
        Task<IEnumerable<GetEnrollmentDTO>> GetAllAsync();
        Task<GetEnrollmentDTO?> GetByIdAsync(int id);
        Task<GetEnrollmentDTO> AddAsync(AddEnrollmentDTO dto);
        Task<GetEnrollmentDTO?> UpdateAsync(int id, AddEnrollmentDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}