using Backend.Backend.DTOs;

namespace Backend.Backend.Interface.ServiceInterface
{
    public interface IAttendanceService
    {
        Task<ResponseDTO<IEnumerable<GetAttendanceDTO>>> GetAllAsync();
        Task<ResponseDTO<GetAttendanceDTO>> GetByIdAsync(int id);
        Task<ResponseDTO<GetAttendanceDTO>> AddAsync(string currentUserId);
        Task<ResponseDTO<IEnumerable<GetTeacherScheduleDTO>>> GetTeacherSchedulesAsync(string currentUserId);
        Task<ResponseDTO<IEnumerable<GetSessionStudentDTO>>> GetSessionStudentsAsync(string currentUserId);
        Task<bool> DeleteAsync(int id);
    }
}