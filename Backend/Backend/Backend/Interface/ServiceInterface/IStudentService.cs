using Backend.Backend.DTOs;
using Backend.Backend.Repository;

namespace Backend.Backend.Interface.ServiceInterface
{
    public interface IStudentService
    {
        Task<ResponseDTO<IEnumerable<GetStudentDTO>>> GetAllAsync();
        Task<ResponseDTO<GetStudentDTO>> GetByIdAsync(int id);
        Task<ResponseDTO<GetStudentDTO>> AddAsync(AddStudentDTO dto, string uuid);
        Task<ResponseDTO<GetStudentDTO>> UpdateAsync(int id, AddStudentDTO dto, string uuid);
        Task<byte[]?> getQrByCurrentStudent(string uuid);
        Task<ResponseDTO<IEnumerable<GetRecordAttendanceOfCertainStudentServiceDTO>>> GetRecordAttendanceOfOneStudent(string uuid);
        Task<byte[]?> getQrById(int id);
        Task<bool> DeleteAsync(int id);
    }
}