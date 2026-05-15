using Backend.Backend.Model;
using Backend.Backend.DTOs;

namespace Backend.Backend.Interface.ServiceInterface
{
    public interface IScheduleService
    {
        Task<ResponseDTO<IEnumerable<GetScheduleDTO>>> GetAllAsync();
        Task<ResponseDTO<GetScheduleDTO>> GetByIdAsync(int id);
        Task<ResponseDTO<GetScheduleDTO>> AddAsync(AddScheduleDTO schedule);
        Task<ResponseDTO<IEnumerable<GetStudentSchedule>>> GetCurrentStudentAttendance(string uuid, string DayOfWeek);
        Task<ResponseDTO<GetScheduleDTO>> UpdateAsync(int id, AddScheduleDTO schedule);
        Task<bool> DeleteAsync(int id);
    }
}