using Backend.Backend.DTOs;

namespace Backend.Backend.Interface.ServiceInterface
{
    public interface IAttendanceStudentService
    {
        Task<ResponseDTO<IEnumerable<GetAttendanceStudentDTO>>> GetAllAsync();
        //Task<ResponseDTO<GetAttendanceStudentDTO>> GetByIdAsync(int id);
        Task<ResponseDTO<GetAttendanceStudentDTO>> AddAsync(ScanRequest request);
        //Task<ResponseDTO<GetAttendanceStudentDTO>> UpdateAsync(int id, AddAttendanceStudentDTO dto);
        //Task<bool> DeleteAsync(int id);
    }
}