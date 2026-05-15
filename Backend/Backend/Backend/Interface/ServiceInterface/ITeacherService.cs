using Backend.Backend.DTOs;

namespace Backend.Backend.Interface.ServiceInterface
{
    public interface ITeacherService
    {
        Task<ResponseDTO<IEnumerable<GetTeacherDTO>>> GetAllAsync();
        Task<ResponseDTO<GetTeacherDTO>> GetByIdAsync(int id);
        Task<ResponseDTO<GetTeacherDTO>> AddAsync(AddTeacherDTO dto, string uuid);
        Task<ResponseDTO<GetTeacherDTO>> UpdateAsync(int id, AddTeacherDTO dto, string uuid);
        Task<bool> DeleteAsync(int id);
    }
}