using Backend.Backend.DTOs;

namespace Backend.Backend.Interface.ServiceInterface
{
    public interface IAttendanceService
    {
        Task<ResponseDTO<IEnumerable<GetAttendanceDTO>>> GetAllAsync();
        Task<ResponseDTO<GetAttendanceDTO>> GetByIdAsync(int id);
        Task<ResponseDTO<GetAttendanceDTO>> AddAsync(AddAttendanceDTO dto);
        Task<ResponseDTO<GetAttendanceDTO>> UpdateAsync(int id, AddAttendanceDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}