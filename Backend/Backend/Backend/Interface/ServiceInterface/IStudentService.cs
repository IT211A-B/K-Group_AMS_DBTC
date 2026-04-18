using Backend.Backend.DTOs;
using Backend.Backend.Repository;

namespace Backend.Backend.Interface.ServiceInterface
{
    public interface IStudentService
    {
        Task<ResponseDTO<IEnumerable<GetStudentDTO>>> GetAllAsync();
        Task<ResponseDTO<GetStudentDTO>> GetByIdAsync(int id);
        Task<ResponseDTO<GetStudentDTO>> AddAsync(AddStudentDTO dto);
        Task<ResponseDTO<GetStudentDTO>> UpdateAsync(int id, AddStudentDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}